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

    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    // Crouch
    private Vector3 playerScale;

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

        // Removed crouch camera lowering, so no StartCrouch / StopCrouch calls
    }

    private void HandleFOV()
    {
        if (!playerCamera) return;

        float targetFOV;

        targetFOV = isSprinting ? sprintFOV : normalFOV;

        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
    }

    private void Movement()
    {
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        bool forwardPressed = y > 0;
        bool backwardPressed = y < 0;

        // Removed CounterMovement call

        if (readyToJump && jumping) Jump();

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

        float targetSpeed = isSprinting ? sprintSpeed : (crouching ? walkSpeed : walkSpeed);

        float controlMultiplier = 1f;
        if (!grounded)
        {
            controlMultiplier = forwardPressed ? 0.8f : (backwardPressed ? 0.5f : 0.8f);
        }

        Vector3 forwardDir = orientation.forward;
        Vector3 rightDir = orientation.right;
        forwardDir.y = 0f;
        rightDir.y = 0f;
        forwardDir.Normalize();
        rightDir.Normalize();

        Vector3 moveDir = forwardDir * y + rightDir * x;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        Vector3 horizontalVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vector3 desiredVel = moveDir * targetSpeed * controlMultiplier;

        Vector3 velocityChange = desiredVel - horizontalVel;
        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        if (!grounded)
            rb.AddForce(Vector3.up * Physics.gravity.y * Time.deltaTime);

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

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private IEnumerator FOVKick(float targetFOV, float duration)
    {
        fovKickActive = true;
        float startFOV = playerCamera.fieldOfView;
        float time = 0f;

        while (time < duration)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        playerCamera.fieldOfView = targetFOV;

        yield return new WaitForSeconds(0.1f);

        float resetFOV = isSprinting ? sprintFOV : normalFOV;
        float currentFOV = playerCamera.fieldOfView;
        float resetDuration = 0.4f;
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
