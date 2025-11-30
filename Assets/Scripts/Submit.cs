using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submit : MonoBehaviour
{
    public GameObject QandAUI;          // The puzzle UI panel inside PadlockQ
    public GameObject Padlock;             // Padlock object in the scene
    public GameObject PadlockUnlockText;   // The "Press [E] to Unlock" UI
    public MonoBehaviour PlayerMovement;   // Player movement script
    public PadlockQ padlockScript;         // Reference to PadlockQ
    public GameObject DoorUI;              // Door UI panel

    public void CloseUI()
    {
        // Hide Padlock puzzle UI
        if (QandAUI != null)
            QandAUI.SetActive(false);

        // Hide Padlock unlock hint
        if (PadlockUnlockText != null)
            PadlockUnlockText.SetActive(false);

        // Hide Padlock object
        if (Padlock != null)
            Padlock.SetActive(false);

        // Enable player movement
        if (PlayerMovement != null)
            PlayerMovement.enabled = true;

        // Unlock the padlock so the door can now be interacted with
        if (padlockScript != null)
            padlockScript.UnlockPadlock();

        // Show the Door UI
        if (DoorUI != null)
            DoorUI.SetActive(true);

        // Restore cursor state
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
