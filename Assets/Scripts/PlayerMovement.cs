using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
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

    // ----------- NEW: sprint state & lock variables -----------
    private bool isSprinting = false;                      // persistent sprint state used by MovePlayer() and HandleFOV()
    private bool sprintLocked = false;                     // locked after depletion; prevents auto re-sprint
    private bool sprintReleasedAfterDepletion = true;     // becomes true only after user releases sprint after depletion
    private float lastExpectedFOV = 0f;                   // used to detect external FOV changes
    // ----------------------------------------------------------

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

        // initialize
        if (playerCamera != null) lastExpectedFOV = playerCamera.fieldOfView;
        sprintReleasedAfterDepletion = true;
        sprintLocked = false;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        HandleJumpTiming();
        HandleFOV();

        // Stop gravity if currently climbing
        if (!climbMechanics.IsClimbing)
        {
            MovePlayer();
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }

    // LateUpdate enforces FOV (helps if another script tries to override it)
    void LateUpdate()
    {
        if (playerCamera == null) return;

        // If something else changed FOV unexpectedly, force it back and warn in console.
        float diff = Mathf.Abs(playerCamera.fieldOfView - lastExpectedFOV);
        if (diff > 0.05f) // small tolerance
        {
            Debug.LogWarning($"[PlayerMovement] External FOV change detected (diff {diff:F2}). Forcing expected FOV.");
            playerCamera.fieldOfView = lastExpectedFOV;
        }
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
        // Use shared isSprinting variable (not recalculating from input here)
        float targetFOV = isSprinting ? sprintFOV : normalFOV;

        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            targetFOV,
            fovChangeSpeed * Time.deltaTime);

        // remember expected FOV so LateUpdate can detect overrides
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

        // read sprint inputs
        bool wantsToSprint = Keyboard.current[sprintKey].isPressed && Keyboard.current[Key.W].isPressed;
        bool sprintPressedThisFrame = Keyboard.current[sprintKey].wasPressedThisFrame;
        bool sprintReleasedThisFrame = Keyboard.current[sprintKey].wasReleasedThisFrame;

        // ---------- SPRINT LOCK + RELEASE LOGIC ----------
        // If stamina depleted -> immediately stop sprint and lock sprint until user releases and re-presses
        if (currentStamina <= 0f)
        {
            isSprinting = false;
            sprintLocked = true;
            sprintReleasedAfterDepletion = false; // require a release
        }
        else
        {
            if (sprintLocked)
            {
                // detect release after depletion
                if (sprintReleasedThisFrame)
                    sprintReleasedAfterDepletion = true;

                // only allow re-sprint if player has released and then pressed again while holding W
                if (sprintReleasedAfterDepletion && sprintPressedThisFrame && Keyboard.current[Key.W].isPressed)
                {
                    sprintLocked = false;
                    sprintReleasedAfterDepletion = false;
                    isSprinting = true;
                }
                else
                {
                    // locked -> no sprint allowed
                    isSprinting = false;
                }
            }
            else
            {
                // normal behavior if not locked
                if (wantsToSprint)
                    isSprinting = true;
                else
                    isSprinting = false;
            }
        }
        // -------------------------------------------------

        // --- Movement speed ---
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

        // --- Handle jumping ---
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

        // --- Stamina drain / regen ---
        if (isSprinting)
        {
            currentStamina -= staminaDrain * Time.deltaTime;
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                isSprinting = false; // instantly stop sprinting
                sprintLocked = true;  // lock sprint until release+press
                sprintReleasedAfterDepletion = false;
            }
        }
        else
        {
            currentStamina += staminaRegen * Time.deltaTime;
            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
        }

        // --- Update stamina bar ---
        if (staminaBar != null)
            staminaBar.SetStamina(currentStamina);
    }

    void DoJump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        lastJumpTime = Time.time;
        coyoteTimeCounter = 0f;
    }
}
