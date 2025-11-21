using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressKeyOpenDoor : MonoBehaviour
{

    public GameObject OpenDoor;
    public GameObject CloseDoor;
    public GameObject AnimeObject;
    public GameObject ThisTrigger;
    //public AudioSource DoorOpenSound;

    private bool doorIsOpen = false;
    public bool Action = false;

    void Start()
    {
        OpenDoor.SetActive(false);
        CloseDoor.SetActive(false);

    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Show appropriate instruction
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

            Action = true;
        }
    }

    void OnTriggerExit(Collider collision)
    {
        OpenDoor.SetActive(false);
        CloseDoor.SetActive(false);
        Action = false;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Action)
        {
            // Toggle door animation
            if (!doorIsOpen)
            {
                AnimeObject.GetComponent<Animator>().Play("DoorOpen");
                doorIsOpen = true;
            }
            else
            {
                AnimeObject.GetComponent<Animator>().Play("DoorClose");
                doorIsOpen = false;
            }

            // Update the instruction text while still in trigger
            if (Action)
            {
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

    }
}
