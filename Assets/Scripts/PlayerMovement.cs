using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Animator animator;
    int isRunningHash, isSprintingHash, isLeftHash, isRightHash, isBackwardHash,
        isJumpUpHash, isJumpDownHash;

    [Header("Stamina Settings")]
    [SerializeField] private float staminaDrain = 20f; // per second while sprinting
    [SerializeField] private float staminaRegen = 10f; // per second while resting
    public StaminaBar staminaBar;
    public float maxStamina = 100f;
    public float currentStamina;

    [Header("Sprint Settings")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    private bool isSprinting = false;
    private bool sprintLocked = false;
    private bool sprintReleasedAfterDepletion = true;
    public float walkSpeed = 3;       // normal walking speed (modifiable in Inspector)
    public float sprintSpeed = 5;     // sprinting speed (modifiable in Inspector)
    private float moveSpeed;
    public float maxSpeed = 10f;
    private bool sprintJumping = false;
    [SerializeField] private float sprintJumpBoostDuration = 1f; // how long boost lasts
    private float sprintJumpBoostTimer = 0f;
    [SerializeField] private float sprintJumpBoost = 5f; // amount of forward boost

    [Header("Camera FOV Sprint Effect")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float sprintFOV = 68f;
    [SerializeField] private float fovChangeSpeed = 6f;

    // Jumping
    [Header("Jump Settings")]
    private bool readyToJump = true;
    [SerializeField] private float jumpCooldown = 0.25f;
    public float jumpForce = 6;

    private bool fovKickActive = false;

    // Assignables
    public Transform playerCam;
    public Transform orientation;

    // Other
    private Rigidbody rb;

    // Rotation and look
    private float xRotation;
    private float sensitivity = 50f;
    private float sensMultiplier = 1f;

    // Ground
    public bool grounded;
    public LayerMask whatIsGround;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    // Crouch
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float crouchSpeedMultiplier = 0.5f;

    // Input
    float x, y;
    bool jumping, crouching;

    // Ground normals
    private Vector3 normalVector = Vector3.up;
    private bool cancellingGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        staminaBar.SetMaxStamina(maxStamina);
        currentStamina = maxStamina;
        if (staminaBar != null)
            staminaBar.SetMaxStamina(maxStamina);

        animator = GetComponent<Animator>();
        isRunningHash = Animator.StringToHash("isRunning");
        isSprintingHash = Animator.StringToHash("isSprinting");
        isRightHash = Animator.StringToHash("isRight");
        isLeftHash = Animator.StringToHash("isLeft");
        isBackwardHash = Animator.StringToHash("isBack");
        isJumpUpHash = Animator.StringToHash("isJumpUp");
        isJumpDownHash = Animator.StringToHash("isJumpDown");
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        MyInput();
        Look();
        HandleFOV();

        // --- Animator Movements ---
        bool forwardPressed = Keyboard.current[Key.W].isPressed;
        bool rightPressed = Keyboard.current[Key.D].isPressed;
        bool leftPressed = Keyboard.current[Key.A].isPressed;
        bool backwardPressed = Keyboard.current[Key.S].isPressed;

        // ?? FIXED: Removed "jumpPressed" check — now based on actual movement
        animator.SetBool(isJumpUpHash, !grounded && rb.velocity.y > 0f);
        animator.SetBool(isJumpDownHash, !grounded && rb.velocity.y < 0f);

        if (grounded)
        {
            animator.SetBool(isJumpUpHash, false);
            animator.SetBool(isJumpDownHash, false);
        }
        animator.SetBool(isRunningHash, forwardPressed);
        animator.SetBool(isBackwardHash, backwardPressed);
        animator.SetBool(isSprintingHash, forwardPressed && isSprinting);
        animator.SetBool(isRightHash, rightPressed && !leftPressed);
        animator.SetBool(isLeftHash, leftPressed && !rightPressed);
    }

    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);

        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
    }

    private void StartCrouch()
    {
        if (playerCam != null)
        {
            Vector3 camPos = playerCam.transform.localPosition;
            camPos.y -= 0.5f; // move camera down slightly
            playerCam.transform.localPosition = camPos;
        }
    }

    private void StopCrouch()
    {
        if (playerCam != null)
        {
            Vector3 camPos = playerCam.transform.localPosition;
            camPos.y += 0.5f; // move camera back up
            playerCam.transform.localPosition = camPos;
        }
    }

    private void HandleFOV()
    {
        if (!playerCamera) return;

        float targetFOV;

        if (sprintJumping)
            targetFOV = sprintFOV + 5f; // keep FOV kick in air
        else
            targetFOV = isSprinting ? sprintFOV : normalFOV;

        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
    }

    private void Movement()
    {
        // --- Apply downward force for consistent grounding ---
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        // --- Get input ---
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        bool forwardPressed = y > 0;
        bool backwardPressed = y < 0;

        // --- Counter movement to prevent sliding ---
        Vector2 mag = FindVelRelativeToLook();
        CounterMovement(x, y, mag);

        // --- Jump ---
        if (readyToJump && jumping) Jump();

        // --- Sprint handling ---
        bool sprintPressed = Input.GetKey(sprintKey);
        bool sprintReleased = Input.GetKeyUp(sprintKey);

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
                if (sprintReleased) sprintReleasedAfterDepletion = true;
                if (sprintReleasedAfterDepletion && sprintPressed && grounded && forwardPressed)
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
                isSprinting = sprintPressed && grounded && !crouching && forwardPressed;
            }
        }

        // --- Determine target speed ---
        float targetSpeed = isSprinting ? sprintSpeed : (crouching ? walkSpeed * crouchSpeedMultiplier : walkSpeed);

        // --- Air control multiplier ---
        float controlMultiplier = 1f;
        if (!grounded)
        {
            controlMultiplier = forwardPressed ? 0.8f : (backwardPressed ? 0.5f : 0.8f);
        }

        // --- Calculate movement direction relative to player orientation ---
        Vector3 forwardDir = orientation.forward;
        Vector3 rightDir = orientation.right;
        forwardDir.y = 0f;
        rightDir.y = 0f;
        forwardDir.Normalize();
        rightDir.Normalize();

        Vector3 moveDir = forwardDir * y + rightDir * x;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        // --- Apply horizontal velocity with air control ---
        Vector3 horizontalVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vector3 desiredVel = moveDir * targetSpeed * controlMultiplier;

        // Apply sprint jump boost if active or if sprintJumping
        if (sprintJumping && sprintJumpBoostTimer > 0f)
        {
            desiredVel += orientation.forward * sprintJumpBoost;
            sprintJumpBoostTimer -= Time.fixedDeltaTime;
        }

        // Apply velocity
        Vector3 velocityChange = desiredVel - horizontalVel;
        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        // Reset boost when grounded
        if (grounded && sprintJumping)
        {
            sprintJumping = false;
            sprintJumpBoostTimer = 0f;
            sprintJumpBoost = 0f; // reset the boost
        }


        // --- Apply gravity ---
        if (!grounded)
            rb.AddForce(Vector3.up * Physics.gravity.y * Time.deltaTime);

        // --- Stamina drain / regen ---
        if (isSprinting)
        {
            currentStamina -= staminaDrain * Time.fixedDeltaTime;
            if (currentStamina < 0f) currentStamina = 0f;
        }
        else
        {
            currentStamina += staminaRegen * Time.fixedDeltaTime;
            if (currentStamina > maxStamina) currentStamina = maxStamina;
        }

        if (staminaBar != null)
            staminaBar.SetStamina(currentStamina);

        // --- Limit speed ---
        float maxVel = targetSpeed;
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > maxVel)
        {
            Vector3 limitedVel = flatVel.normalized * maxVel;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }


    private void Jump()
    {
        if (grounded && readyToJump)
        {
            readyToJump = false;

            // Apply upward jump force
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // Trigger sprint jump boost
            if (isSprinting && y > 0)
            {
                sprintJumping = true;
                sprintJumpBoost = 3f; // increased from 2f to 3f for a stronger boost
                sprintJumpBoostTimer = sprintJumpBoostDuration;

                StartCoroutine(FOVKick(sprintFOV + 5f, sprintJumpBoostDuration));
            }


            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private IEnumerator FOVKick(float targetFOV, float duration)
    {
        fovKickActive = true;
        float startFOV = playerCamera.fieldOfView;
        float time = 0f;

        // Ramp up to target FOV
        while (time < duration)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        playerCamera.fieldOfView = targetFOV;

        // Wait a tiny bit to make the kick noticeable
        yield return new WaitForSeconds(0.1f);

        // Smoothly reset FOV to normal/sprint
        float resetFOV = isSprinting ? sprintFOV : normalFOV;
        float currentFOV = playerCamera.fieldOfView;
        float resetDuration = 0.4f; // slower reset for smoother feel
        time = 0f;

        while (time < resetDuration)
        {
            playerCamera.fieldOfView = Mathf.Lerp(currentFOV, resetFOV, time / resetDuration);
            time += Time.deltaTime;
            yield return null;
        }

        playerCamera.fieldOfView = resetFOV;
        fovKickActive = false;
    }



    private void ResetJump() => readyToJump = true;

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        float desiredX = rot.y + mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded) return;

        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
            rb.AddForce(moveSpeed * orientation.right * Time.deltaTime * -mag.x * counterMovement);
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
            rb.AddForce(moveSpeed * orientation.forward * Time.deltaTime * -mag.y * counterMovement);

        if (Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitude = rb.velocity.magnitude;
        float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private void OnCollisionStay(Collision other)
    {
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            if (IsFloor(normal))
            {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * 3f);
        }
    }

    private void StopGrounded() => grounded = false;
}
