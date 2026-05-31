using System;
using System.Collections;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerMovementSystem : PlayerSystem
    {
        [Header("ScriptableObjects")] 
        [SerializeField] private InputReaderSO inputReader;
        
        [Header("Movement")] 
        [SerializeField] private float movementSpeed;
        [SerializeField] private float jumpHeight;
        [SerializeField] private float gravityScale;
        [SerializeField] private float maxSpeed;
        
        [Header("Acceleration")]
        [SerializeField] private float airAcceleration;
        [SerializeField] private float maxAirMomentum;
        [SerializeField] private float groundAcceleration;
        [SerializeField] private float airControl;
        
        [Header("Decay")]
        [SerializeField] private float airDecay;
        [SerializeField] private float groundDecay;

        [Header("Dash")]
        [SerializeField] private int dashMaxCharges;
        [SerializeField] private float dashRechargeTime;
        [SerializeField] private float dashImpulse;
        [SerializeField] private float dashDuration;
        [SerializeField] private AudioClip dashSfx;
        private float dashDurationTimer;
        private float dashTimer;

        [Header("Ground Detection")]
        [SerializeField] private Transform feet;
        [SerializeField] private float detectionRadius;
        [SerializeField] private LayerMask whatIsGround;

        [Header("SlowMotion")] 
        [SerializeField] private int maxSlowMotion;
        [SerializeField] private AudioClip addSlowMotionSfx;

        private int currentSlowMotion;

        public bool IsSlowMotion { get; private set; }
        public bool IsDashing { get; private set; }
        public bool IsGrounded { get; private set; }
        public bool IsMoving { get; private set; }
        
        private int dashCharges;

        private Vector3 movementMomentum;
        private Vector3 dashMomentum;
        private Vector2 inputVector;
        private Vector3 verticalMovement;

        private float currentTimeScale;
        private float targetTimeScale = 1f;

        public static event Action<int, float> OnDashUpdate;
        public static event Action<float> OnSlowMotionChange; 
        
        protected override void Awake()
        {
            base.Awake();

            dashCharges = dashMaxCharges;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnEnable()
        {
            main.InputReader.OnJumpStarted += Jump;
            main.InputReader.OnMoveEvent += UpdateMovement;
            main.InputReader.OnDashStarted += Dash;
            main.InputReader.OnAimStarted += StartSlowMotion;
            main.InputReader.OnAimCancel += StopSlowMotion;
            SpawnerManager.OnEnemyDeath += AddSlowMotion;
        }
        private void OnDisable()
        {
            main.InputReader.OnJumpStarted -= Jump;
            main.InputReader.OnMoveEvent -= UpdateMovement;
            main.InputReader.OnDashStarted -= Dash;
            main.InputReader.OnAimStarted -= StartSlowMotion;
            main.InputReader.OnAimCancel -= StopSlowMotion;
            SpawnerManager.OnEnemyDeath += AddSlowMotion;
        }

        private void UpdateMovement(Vector2 input)
        {
            inputVector = input;
        }
        
        private void Jump()
        {
            if (IsGrounded)
            {
                verticalMovement.y = Mathf.Sqrt(-2 * gravityScale * jumpHeight);
            }
        }

        void Update()
        {
            GroundCheck();
            ApplyGravity();
            MoveAndRotate();
            HandleDashCooldown();

            currentTimeScale = Mathf.Lerp(currentTimeScale, targetTimeScale, 10 * Time.unscaledDeltaTime);
            Time.timeScale = currentTimeScale;
            
            if (IsDashing)
            {
                dashDurationTimer -= Time.deltaTime;
                if (dashDurationTimer <= 0) IsDashing = false;
            }
            else
            {
                DashDecay();
            }
        }

        private void MoveAndRotate()
        {
            transform.rotation = Quaternion.Euler(0, main.Cam.transform.eulerAngles.y, 0);
            Vector3 movement = Vector3.zero;
            
            if (inputVector.sqrMagnitude > 0)
            {
                float angle = Mathf.Atan2(inputVector.x, inputVector.y) * Mathf.Rad2Deg + main.Cam.transform.eulerAngles.y;
                float speedMultiplier = Mathf.Lerp(1f, 1f / Time.timeScale, 0.5f);
                float speed = movementSpeed * inputVector.magnitude * speedMultiplier;
                Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                movement = direction * speed;
            }
            main.CameraSystem.UpdateTilt(movement.normalized);

            if (!IsDashing)
            {
                if (IsGrounded)
                {
                    movementMomentum = Vector3.Lerp(movementMomentum, movement, groundAcceleration * Time.deltaTime);
                }
                else
                {
                    if (inputVector.sqrMagnitude > 0)
                        movementMomentum = Vector3.Lerp(movementMomentum, movement, airControl * Time.deltaTime);
                    else
                        movementMomentum = Vector3.Lerp(movementMomentum, Vector3.zero, airDecay * Time.deltaTime);
                }

                if (movementMomentum.magnitude < 0.01f) movementMomentum = Vector3.zero;
            }
            else
                movementMomentum = Vector3.zero;

            Vector3 totalMovement = movementMomentum + dashMomentum;
            if (totalMovement.magnitude >= maxSpeed)
                totalMovement = totalMovement.normalized * maxSpeed;
            
            main.Controller.Move((totalMovement + verticalMovement) * Time.deltaTime);

            if (totalMovement.sqrMagnitude <= 0.2) IsMoving = false;
            else IsMoving = true;
        }

        private void ApplyGravity()
        {
            if (IsDashing)
            {
                verticalMovement.y = 0;
            }
            
            if (IsGrounded && verticalMovement.y < 0)
            {
                verticalMovement.y = -2f;
            }
            else
            {
                verticalMovement.y += gravityScale * Time.deltaTime;
            }
        }

        private void Dash()
        {
            if (dashCharges <= 0) return;
            
            IsDashing = true;
            dashDurationTimer = dashDuration;
            
            Vector3 direction;
            
            float angle = Mathf.Atan2(inputVector.x, inputVector.y) * Mathf.Rad2Deg + main.Cam.transform.eulerAngles.y;
            direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            
            main.CameraSystem.ApplyDashFovEffect(direction);
            dashCharges--;
            dashMomentum = direction.normalized * dashImpulse;
            AudioManager.Instance.PlaySfx(dashSfx, 0.2f);
        }

        private void DashDecay()
        {
            if (IsGrounded)
            {
                dashMomentum = Vector3.Lerp(dashMomentum, Vector3.zero, groundDecay * Time.deltaTime);
            }
            else
            {
                dashMomentum = Vector3.Lerp(dashMomentum, Vector3.zero, airDecay * Time.deltaTime);
            }
            if (dashMomentum.magnitude < 0.01f) dashMomentum = Vector3.zero;
        }

        private void HandleDashCooldown()
        {
            if (dashCharges >= dashMaxCharges) return;
            
            dashTimer += Time.deltaTime;

            OnDashUpdate?.Invoke(dashCharges, dashTimer / dashRechargeTime);
            
            if (!(dashTimer >= dashRechargeTime)) return;
            
            dashCharges++;
            dashTimer = 0;
            
        }

        private void GroundCheck()
        {
            IsGrounded = Physics.CheckSphere(feet.position, detectionRadius, whatIsGround);
        }

        private void StartSlowMotion()
        {
            if (currentSlowMotion <= 0) return;
            IsSlowMotion = true;
            targetTimeScale = 0.1f;
            StartCoroutine(RemoveSlowMotion());
        }

        private void StopSlowMotion()
        {
            IsSlowMotion = false;
            targetTimeScale = 1f;
        }

        private void OnDrawGizmos()
        {
            if (feet != null)
            {
                Gizmos.DrawSphere(feet.position, detectionRadius);
            }
        }
        
        private void AddSlowMotion()
        {
            currentSlowMotion += 15;
            if (currentSlowMotion >= 100) currentSlowMotion = 100;
            OnSlowMotionChange?.Invoke((float)currentSlowMotion/maxSlowMotion);
            AudioManager.Instance.PlaySfx(addSlowMotionSfx);
        }

        private IEnumerator RemoveSlowMotion()
        {
            while (IsSlowMotion)
            {
                currentSlowMotion -= 3;
                OnSlowMotionChange?.Invoke((float)currentSlowMotion/maxSlowMotion);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
