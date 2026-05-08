using System;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;
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
        [SerializeField] private float airAcceleration = 0.1f;
        [SerializeField] private float maxAirMomentum = 0.2f;

        [Header("Dash")]
        [SerializeField] private int dashMaxCharges;
        [SerializeField] private float dashTime;
        [SerializeField] private float dashImpulse;
        [SerializeField] private float airDecay = 2f;
        [SerializeField] private float groundDecay = 5f;
        private float dashTimer;

        [Header("Ground Detection")]
        [SerializeField] private Transform feet;
        [SerializeField] private float detectionRadius;
        [SerializeField] private LayerMask whatIsGround;

        private int dashCharges;

        private Vector3 movementMomentum;
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

        if (!isGrounded)
        {
            Vector3 airContribution = movement * airAcceleration;
            if (airContribution.magnitude > maxAirMomentum)
                airContribution = airContribution.normalized * maxAirMomentum;
            movementMomentum += airContribution;
        }
        
        float currentDecay = isGrounded ? groundDecay : airDecay;
        if (movementMomentum.magnitude > maxSpeed)
            movementMomentum = movementMomentum.normalized * maxSpeed;
        movementMomentum = Vector3.Lerp(movementMomentum, Vector3.zero, currentDecay * Time.deltaTime);
        if (movementMomentum.magnitude < 0.01f) movementMomentum = Vector3.zero;
        main.Controller.Move((movement + verticalMovement + movementMomentum) * Time.deltaTime);
    }

    private void ApplyGravity()
    {
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
        movementMomentum += direction.normalized * dashImpulse;
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
