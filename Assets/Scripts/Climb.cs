using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climb : MonoBehaviour
{
    public Rigidbody rb;
    public Camera cam;

    [Header("Climbing")]
    public float climbSpeed = 3f;
    public float maxClimbDistance = 2f;
    public LayerMask climbableLayer;

    [Header("UI")]
    public GameObject climbPrompt;  // UI popup GameObject

    private bool touchingMountain = false;
    private bool isSticking = false;
    private Vector3 climbDirection = Vector3.up;

    void Start()
    {
        climbPrompt.SetActive(false);  // start hidden
    }

    void Update()
    {
        CheckClimbableSurface();

        if (touchingMountain && Input.GetMouseButton(0))
            StartSticking();
        else
            StopSticking();

        if (isSticking && Input.GetKey(KeyCode.W))
            ClimbSlope();

        UpdateClimbPrompt();
    }

    void CheckClimbableSurface()
    {
        touchingMountain = false;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward,
            out RaycastHit hit, maxClimbDistance, climbableLayer))
        {
            if (hit.collider.CompareTag("Climbable"))
            {
                touchingMountain = true;

                Vector3 slopeUp = Vector3.Cross(hit.normal, cam.transform.right);
                climbDirection = slopeUp.normalized;
            }
        }
    }

    void StartSticking()
    {
        isSticking = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.linearDamping = 5f;
    }

    void StopSticking()
    {
        isSticking = false;
        rb.useGravity = true;
        rb.linearDamping = 0f;
    }

    void ClimbSlope()
    {
        rb.linearVelocity = climbDirection * climbSpeed;
    }

    void UpdateClimbPrompt()
    {
        if (climbPrompt == null) return;

        // EXACTLY like PickupItem logic:
        if (touchingMountain)
            climbPrompt.SetActive(true);   // show popup
        else
            climbPrompt.SetActive(false);  // hide popup
    }
}
