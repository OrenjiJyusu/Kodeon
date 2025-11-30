using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door3 : MonoBehaviour
{
    public GameObject OpenDoor;
    public GameObject CloseDoor;
    public GameObject AnimeObject;
    public GameObject TriggerDoorOpen;

    private bool doorIsOpen = false;
    private bool Action = false;

    public float rayDistance = 3f;     // distance to interact
    public LayerMask doorLayer;        // set this layer to your door

    [HideInInspector]
    public bool isLocked = false;      // controlled by padlock

    void Start()
    {
        OpenDoor.SetActive(false);
        CloseDoor.SetActive(false);
    }

    void Update()
    {
        ReticleCheck();

        // Only allow opening if the door is unlocked
        if (Input.GetKeyDown(KeyCode.E) && Action && !isLocked)
        {
            ToggleDoor();
            ShowCorrectUI();
        }
    }

    void ReticleCheck()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, doorLayer))
        {
            // Only activate this door if THIS door was hit
            if (hit.transform.gameObject == this.gameObject)
            {
                Action = true;
                ShowCorrectUI();
            }
            else
            {
                Action = false;
                OpenDoor.SetActive(false);
                CloseDoor.SetActive(false);
            }
        }
        else
        {
            Action = false;
            OpenDoor.SetActive(false);
            CloseDoor.SetActive(false);
        }
    }


    void ToggleDoor()
    {
        if (!doorIsOpen)
        {
            AnimeObject.GetComponent<Animator>().Play("DoorOpen3");
            doorIsOpen = true;
        }
        else
        {
            AnimeObject.GetComponent<Animator>().Play("DoorClose3");
            doorIsOpen = false;
        }
    }

    void ShowCorrectUI()
    {
        if (!Action) return;

        if (!doorIsOpen)
        {
            OpenDoor.SetActive(true);
            CloseDoor.SetActive(false);
        }
        else
        {
            OpenDoor.SetActive(false);
            CloseDoor.SetActive(true);
        }
    }
}
