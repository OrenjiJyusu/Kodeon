using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multiple1: MonoBehaviour
{
    public GameObject uiPanel;        // Puzzle UI panel
    public GameObject padlockText;    // Hint text
    public float interactDistance = 3f;
    public LayerMask interactLayer;
    public PlayerMovement playerMovement;

    public Door2 door;     // Reference to door script

    Camera cam;
    bool action = false;
    private bool padlockUnlocked = false; // keep private

    void Start()
    {
        cam = Camera.main;
        padlockText.SetActive(false);
        uiPanel.SetActive(false);

        if (door != null)
            door.isLocked = true; // lock the door initially
    }

    void Update()
    {
        ReticleCheck();

        if (Input.GetKeyDown(KeyCode.E) && action)
        {
            OpenPanel();
        }
    }

    void ReticleCheck()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            if (hit.collider.CompareTag("Padlock2") && !padlockUnlocked)
            {
                action = true;
                padlockText.SetActive(true);
                return;
            }
        }

        action = false;
        padlockText.SetActive(false);
    }

    void OpenPanel()
    {
        uiPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        playerMovement.enabled = false;
        padlockText.SetActive(false);
        padlockUnlocked = true;
    }

    // --- NEW METHOD: Close the panel safely ---
    public void ClosePanel()
    {
        uiPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerMovement.enabled = true;

        padlockUnlocked = true;

        // Hide the hint text completely
        padlockText.SetActive(false);
        // Unlock the door
        UnlockPadlock();

        // Hide the padlock object itself
        gameObject.SetActive(false);
    }

    // Public method to unlock padlock safely
    public void UnlockPadlock()
    {
        if (door != null)
        {
            door.isLocked = false;

            // Enable the collider on the child object
            if (door.TriggerDoorOpen != null)
            {
                door.TriggerDoorOpen.GetComponent<Collider>().enabled = true;
            }
        }
    }
}
