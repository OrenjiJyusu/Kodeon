using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door6: MonoBehaviour
{
    [Header("UI")]
    public GameObject OpenDoorUI;
    public GameObject CloseDoorUI;

    [Header("Door Objects")]
    public GameObject AnimeObject;       // Animator object for the door
    public GameObject TriggerDoorOpen;   // Collider for interaction (could be trigger or mesh)

    [Header("Settings")]
    public float interactDistance = 3f;
    public LayerMask doorLayer;

    private bool doorIsOpen = false;
    private bool canInteract = false;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        if (OpenDoorUI != null) OpenDoorUI.SetActive(false);
        if (CloseDoorUI != null) CloseDoorUI.SetActive(false);
    }

    void Update()
    {
        CheckReticle();

        if (Input.GetKeyDown(KeyCode.E) && canInteract)
        {
            ToggleDoor();
            ShowCorrectUI();
        }
    }

    void CheckReticle()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, doorLayer))
        {
            // Accept either the trigger collider or any child of this door
            if (hit.transform.IsChildOf(transform) || hit.transform == TriggerDoorOpen.transform)
            {
                canInteract = true;
                ShowCorrectUI();
                return;
            }
        }

        canInteract = false;

        if (OpenDoorUI != null) OpenDoorUI.SetActive(false);
        if (CloseDoorUI != null) CloseDoorUI.SetActive(false);
    }

    void ToggleDoor()
    {
        if (doorIsOpen)
        {
            AnimeObject.GetComponent<Animator>().Play("DoorClose6");
            doorIsOpen = false;
        }
        else
        {
            AnimeObject.GetComponent<Animator>().Play("DoorOpen6");
            doorIsOpen = true;
        }
    }

    void ShowCorrectUI()
    {
        if (!canInteract) return;

        if (doorIsOpen)
        {
            if (OpenDoorUI != null) OpenDoorUI.SetActive(false);
            if (CloseDoorUI != null) CloseDoorUI.SetActive(true);
        }
        else
        {
            if (OpenDoorUI != null) OpenDoorUI.SetActive(true);
            if (CloseDoorUI != null) CloseDoorUI.SetActive(false);
        }
    }
}
