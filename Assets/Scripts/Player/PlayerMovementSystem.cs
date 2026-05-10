using System;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerMovementSystem : PlayerSystem
    {
        [Header("ScriptableObjects")] 
        [SerializeField] private InputReaderSO inputReader;

        [SerializeField] private TMP_Text speedText;
        
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
        [SerializeField] private float dashTime;
        [SerializeField] private float dashImpulse;
        [SerializeField] private float dashDuration;
        private bool isDashing;
        private float dashDurationTimer;
        private float dashTimer;

        [Header("Ground Detection")]
        [SerializeField] private Transform feet;
        [SerializeField] private float detectionRadius;
        [SerializeField] private LayerMask whatIsGround;

        private int dashCharges;

        private Vector3 movementMomentum;
        private Vector3 dashMomentum;
        private bool isGrounded;
        private Vector2 inputVector;
        private Vector3 verticalMovement;

        protected override void Awake()
        {
            base.Awake();

            dashCharges = dashMaxCharges;
            Cursor.lockState = CursorLockMode.Locked;
        }

    private void OnEnable()
    {
        inputReader.OnJumpStarted += Jump;
        inputReader.OnMoveEvent += UpdateMovement;
        inputReader.OnDashStarted += Dash;
        
    }

    private void OnDisable()
    {
        inputReader.OnJumpStarted -= Jump;
        inputReader.OnMoveEvent -= UpdateMovement;
        inputReader.OnDashStarted -= Dash;
    }

    private void UpdateMovement(Vector2 input)
    {
        inputVector = input;
    }
    
    private void Jump()
    {
        if (isGrounded)
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
        DashDecay();
        
        if (isDashing)
        {
            dashDurationTimer -= Time.deltaTime;
            if (dashDurationTimer <= 0) isDashing = false;
        }
    }

    private void MoveAndRotate()
    {
        transform.rotation = Quaternion.Euler(0, main.Cam.transform.eulerAngles.y, 0);
        Vector3 movement = Vector3.zero;
        
        if (inputVector.sqrMagnitude > 0)
        {
            float angle = Mathf.Atan2(inputVector.x, inputVector.y) * Mathf.Rad2Deg + main.Cam.transform.eulerAngles.y;
            float speed = movementSpeed * inputVector.magnitude;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            
            movement = direction * speed;
        }

        if (isGrounded)
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

        Vector3 totalMomentum = movementMomentum + dashMomentum;
        if (totalMomentum.magnitude > maxSpeed)
            totalMomentum = totalMomentum.normalized * maxSpeed;
        main.Controller.Move((totalMomentum + verticalMovement) * Time.deltaTime);
        speedText.SetText($"Speed: {Mathf.RoundToInt(totalMomentum.magnitude)}");
    }

    private void ApplyGravity()
    {
        if (isDashing) return;
        
        if (isGrounded && verticalMovement.y < 0)
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
        
        isDashing = true;
        dashDurationTimer = dashDuration;
        
        Vector3 direction;
        if (inputVector.sqrMagnitude > 0)
        {
            float angle = Mathf.Atan2(inputVector.x, inputVector.y) * Mathf.Rad2Deg + main.Cam.transform.eulerAngles.y;
            direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        }
        else
        {
            direction = main.Cam.transform.forward;
        }
        
        main.CameraSystem.ApplyDashFovEffect(direction);
        //dashCharges--;
        dashMomentum += direction.normalized * dashImpulse;
    }

    private void DashDecay()
    {
        if (isGrounded)
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

        if (!(dashTimer >= dashTime)) return;
        
        dashCharges++;
        dashTimer = 0;
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(feet.position, detectionRadius, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        if (feet != null)
        {
            Gizmos.DrawSphere(feet.position, detectionRadius);
        }
    }
    }
}
