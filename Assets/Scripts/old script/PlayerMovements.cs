using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    Animator animator;
    int isRunningHash, isSprintingHash, isLeftHash, isRightHash, isBackwardHash,
        isJumpUpHash, isJumpDownHash;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private CharacterController controller;
    private cameracode cameraCode;
    private ClimbingMechanics climbMechanics;

    [Header("Movement Settings")]
    [SerializeField] float walkSpeed = 10f;
    [SerializeField] float sprintSpeed = 20f;
    [SerializeField] float gravity = -22f;
    [SerializeField] float jumpHeight = 2.0f;
    [SerializeField] Transform cameraTransform;

    [Header("FOV Sprint Effect")]
    [SerializeField] Camera playerCamera;
    [SerializeField] float normalFOV = 60f;
    [SerializeField] float sprintFOV = 65f;
    [SerializeField] float fovChangeSpeed = 8f;

    [Header("Key Bindings")]
    [SerializeField] Key sprintKey = Key.LeftShift;
    [SerializeField] Key jumpKey = Key.Space;

    [Header("Jump Settings")]
    [SerializeField] bool useJumpCooldown = true;
    [SerializeField] float jumpCooldown = 0.3f;
    private float lastJumpTime = -Mathf.Infinity;

    [Header("Ground Checker")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.15f;
    [SerializeField] LayerMask groundMask;

    [Header("Air Control")]
    [SerializeField] float airControlMultiplier = 0.8f;
    [SerializeField] float backwardMultiplier = 0.5f;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrain = 20f; // per second while sprinting
    [SerializeField] private float staminaRegen = 10f; // per second while resting
    [SerializeField] private StaminaBar staminaBar;
    private float currentStamina;

    [Header("Coyote & Buffer Jump")]
    [SerializeField] float coyoteTime = 0.15f;
    [SerializeField] float jumpBufferTime = 0.12f;

    private Vector3 velocity;
    private bool isGrounded;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private bool isSprinting = false;
    private bool sprintLocked = false;
    private bool sprintReleasedAfterDepletion = true;
    private float lastExpectedFOV = 0f;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
        controller = GetComponent<CharacterController>();
        cameraCode = playerCamera.GetComponent<cameracode>();
        climbMechanics = GetComponent<ClimbingMechanics>();

        currentStamina = maxStamina;
        if (staminaBar != null)
            staminaBar.SetMaxStamina(maxStamina);

        if (playerCamera != null) lastExpectedFOV = playerCamera.fieldOfView;

        animator = GetComponent<Animator>();
        isRunningHash = Animator.StringToHash("isRunning");
        isSprintingHash = Animator.StringToHash("isSprinting");
        isRightHash = Animator.StringToHash("isRight");
        isLeftHash = Animator.StringToHash("isLeft");
        isBackwardHash = Animator.StringToHash("isBack");
        isJumpUpHash = Animator.StringToHash("isJumpUp");
        isJumpDownHash = Animator.StringToHash("isJumpDown");
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        HandleJumpTiming();

        if (!climbMechanics.IsClimbing)
        {
            MovePlayer();
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        // --- Animator Movements ---
        bool forwardPressed = Keyboard.current[Key.W].isPressed;
        bool rightPressed = Keyboard.current[Key.D].isPressed;
        bool leftPressed = Keyboard.current[Key.A].isPressed;
        bool backwardPressed = Keyboard.current[Key.S].isPressed;

        // 🔹 FIXED: Removed "jumpPressed" check — now based on actual movement
        animator.SetBool(isJumpUpHash, !isGrounded && velocity.y > 0f);
        animator.SetBool(isJumpDownHash, !isGrounded && velocity.y < 0f);

        // Reset jump booleans when grounded
        if (isGrounded)
        {
            animator.SetBool(isJumpUpHash, false);
            animator.SetBool(isJumpDownHash, false);
        }

        animator.SetBool(isRunningHash, forwardPressed);
        animator.SetBool(isBackwardHash, backwardPressed);
        animator.SetBool(isSprintingHash, forwardPressed && isSprinting);
        animator.SetBool(isRightHash, rightPressed && !leftPressed);
        animator.SetBool(isLeftHash, leftPressed && !rightPressed);

        HandleFOV();
    }

    void HandleJumpTiming()
    {
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        if (Keyboard.current[jumpKey].wasPressedThisFrame)
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;
    }

    void HandleFOV()
    {
        float targetFOV = isSprinting ? sprintFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
        lastExpectedFOV = playerCamera.fieldOfView;
    }

    void MovePlayer()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * input.y + right * input.x;
        if (direction.sqrMagnitude > 1f) direction.Normalize();

        bool wantsToSprint = Keyboard.current[sprintKey].isPressed && Keyboard.current[Key.W].isPressed;
        bool sprintPressedThisFrame = Keyboard.current[sprintKey].wasPressedThisFrame;
        bool sprintReleasedThisFrame = Keyboard.current[sprintKey].wasReleasedThisFrame;

        // Sprint logic
        if (currentStamina <= 0f)
        {
            currentStamina = 0f;
            isSprinting = false;
            sprintLocked = true;
            sprintReleasedAfterDepletion = false;
        }
        else
        {
            if (sprintLocked)
            {
                if (sprintReleasedThisFrame) sprintReleasedAfterDepletion = true;
                if (sprintReleasedAfterDepletion && sprintPressedThisFrame && Keyboard.current[Key.W].isPressed)
                {
                    sprintLocked = false;
                    sprintReleasedAfterDepletion = false;
                    isSprinting = true;
                }
                else
                {
                    isSprinting = false;
                }
            }
            else
            {
                isSprinting = wantsToSprint;
            }
        }

        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;
        float control = (!isGrounded)
            ? (input.y < 0f ? backwardMultiplier : airControlMultiplier)
            : 1f;

        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;
        float smoothSpeed = Mathf.Lerp(currentSpeed, targetSpeed * control, Time.deltaTime * 10f);
        Vector3 move = direction * smoothSpeed;

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        // Jump input
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            if (!useJumpCooldown || Time.time >= lastJumpTime + jumpCooldown)
            {
                DoJump();
                jumpBufferCounter = 0f;
            }
        }

        velocity.y += gravity * Time.deltaTime;
        Vector3 totalMotion = move + Vector3.up * velocity.y;
        controller.Move(totalMotion * Time.deltaTime);

        // Stamina handling
        if (isSprinting)
        {
            currentStamina -= staminaDrain * Time.deltaTime;
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                isSprinting = false;
                sprintLocked = true;
                sprintReleasedAfterDepletion = false;
            }
        }
        else
        {
            currentStamina += staminaRegen * Time.deltaTime;
            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
        }

        if (staminaBar != null)
            staminaBar.SetStamina(currentStamina);
    }

    void DoJump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        lastJumpTime = Time.time;
        coyoteTimeCounter = 0f;

        // 🔥 FIXED: Trigger jump animation instantly
        animator.SetBool(isJumpUpHash, true);
        animator.SetBool(isJumpDownHash, false);
    }
}
