using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject PickupText;
    public GameObject LampOnPlayer;

    void Start()
    {
        LampOnPlayer.SetActive(false);
        PickupText.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PickupText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                this.gameObject.SetActive(false);
                LampOnPlayer.SetActive(true);

                PickupText.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PickupText.SetActive(false);
    }
}
