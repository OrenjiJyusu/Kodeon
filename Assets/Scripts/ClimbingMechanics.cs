using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ClimbingMechanics : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private cameracode cameraCode;

    [Header("Climb Settings")]
    [SerializeField] private LayerMask climbableMask;
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float climbCheckDistance = 0.8f;
    [Tooltip("Small offset to keep player stuck to wall; use a small value like 0.05 - 0.2")]
    [SerializeField] private float wallStickForce = 0.12f;

    private CharacterController controller;
    private bool isClimbingActive = false;
    private bool isGrabbingWall = false;
    private bool climbToggle = false;

    public bool IsClimbing => isGrabbingWall;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Auto-assign cameraTransform / cameraCode if they aren't set in Inspector
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraCode == null && cameraTransform != null)
            cameraCode = cameraTransform.GetComponent<cameracode>();

       /* if (cameraTransform == null)
            Debug.LogWarning("ClimbingMechanics: cameraTransform is not assigned and Camera.main is null.");
        if (cameraCode == null)
            Debug.LogWarning("ClimbingMechanics: cameraCode not found on cameraTransform.");*/
    }

    void Update()
    {
        HandleClimbing();
    }

    void HandleClimbing()
    {
        if (cameraTransform == null)
            return;

        // Detect click to toggle climb
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            climbToggle = !climbToggle;
        }

        bool moveUp = Keyboard.current[Key.W].isPressed;
        bool moveDown = Keyboard.current[Key.S].isPressed;
        bool moveLeft = Keyboard.current[Key.A].isPressed;
        bool moveRight = Keyboard.current[Key.D].isPressed;

        Vector3 camForward = cameraTransform.forward;
        Vector3 flatForward = camForward;
        flatForward.y = 0f;
        flatForward.Normalize();

        // Blend between flat and camera directions
        float lookBlend = 0.3f; // tweak 0.0–0.5 for feel
        Vector3 climbRayDir = Vector3.Lerp(flatForward, camForward, lookBlend);

        // Optional: visualize in Scene view (remove later)
        Debug.DrawRay(cameraTransform.position, climbRayDir * climbCheckDistance, Color.green);

        bool canClimb = Physics.Raycast(cameraTransform.position, climbRayDir,
            out RaycastHit hit, climbCheckDistance, climbableMask);

        // Climb while toggled ON and facing a wall
        isClimbingActive = climbToggle && canClimb;

        if (isClimbingActive)
        {
            isGrabbingWall = true;

            controller.Move(-hit.normal * wallStickForce * Time.deltaTime);

            Vector3 climbDirection = Vector3.zero;
            if (moveUp) climbDirection += Vector3.up;
            if (moveDown) climbDirection += Vector3.down;

            Vector3 cameraRightFlat = cameraTransform.right;
            cameraRightFlat.y = 0f;
            cameraRightFlat.Normalize();

            if (moveRight) climbDirection += cameraRightFlat;
            if (moveLeft) climbDirection -= cameraRightFlat;

            if (climbDirection != Vector3.zero)
            {
                climbDirection.Normalize();
                controller.Move(climbDirection * climbSpeed * Time.deltaTime);
            }
        }
        else
        {
            isGrabbingWall = false;
        }

        if (cameraCode != null)
            cameraCode.isClimbing = isGrabbingWall;
    }

}
