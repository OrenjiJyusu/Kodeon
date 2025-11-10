using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameracode : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 2f;

    public bool isClimbing = false;

    [Header("Climb Camera Settings")]
    public float climbYawLimit = 50f;   // how far left/right you can look
    public float climbPitchLimit = 45f; // how far up/down you can look
    public float cameraFollowSpeed = 10f;


    private float cameraVerticalRotation = 0f;
    private float cameraHorizontalOffset = 0f;
    private Quaternion climbTargetRotation;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

       
    }

    void Update()
    {
        ClimbingCamera();
    }
    void ClimbingCamera()
    {
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;


        if (isClimbing)
        {
            // Horizontal camera offset (limited side look)
            cameraHorizontalOffset += inputX;
            cameraHorizontalOffset = Mathf.Clamp(cameraHorizontalOffset, -climbYawLimit, climbYawLimit);

            // Vertical camera rotation (limited up/down)
            cameraVerticalRotation -= inputY * 0.6f;
            cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -climbPitchLimit, climbPitchLimit);

            // Build camera rotation relative to player’s facing direction
            Quaternion baseRotation = Quaternion.Euler(0f, player.eulerAngles.y, 0f);
            Quaternion localOffset = Quaternion.Euler(cameraVerticalRotation, cameraHorizontalOffset, 0f);

            climbTargetRotation = baseRotation * localOffset;

            // Smooth transition for stability
            transform.rotation = Quaternion.Slerp(transform.rotation, climbTargetRotation, Time.deltaTime * cameraFollowSpeed);
        }
        else
        {
            // Normal FPS camera behavior
            cameraVerticalRotation -= inputY;
            cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -80f, 80f);

            // smoothly rotate camera back to normal alignment
            Quaternion normalRotation = Quaternion.Euler(cameraVerticalRotation, player.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, normalRotation, Time.deltaTime * cameraFollowSpeed);

            player.Rotate(Vector3.up * inputX);

            // Gradually reset the horizontal offset instead of instantly snapping
            cameraHorizontalOffset = Mathf.Lerp(cameraHorizontalOffset, 0f, Time.deltaTime * 5f);
        }

    }
}
