using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickItems : MonoBehaviour
{
    public GameObject Flashlight;
    public Transform ItemParent;
    public GameObject PickupText;


    void Start()
    {
        PickupText.SetActive(false);
        Flashlight.GetComponent<Rigidbody>().isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            Drop();
        }
    }

    void Drop()
    {
        GetComponent<Collider>().enabled = true;

        ItemParent.DetachChildren();
        Flashlight.transform.eulerAngles = new Vector3(Flashlight.transform.position.x, Flashlight.transform.position.z, Flashlight.transform.position.y);
        Flashlight.GetComponent<Rigidbody>().isKinematic = false;
        Flashlight.GetComponent<MeshCollider>().enabled = true;
    }

    void Equip()
    {
        GetComponent<Collider>().enabled = false;
        PickupText.SetActive(false);

        Flashlight.GetComponent<Rigidbody>().isKinematic = true;

        Flashlight.transform.position = ItemParent.position;
        Flashlight.transform.rotation = ItemParent.rotation;

        Flashlight.GetComponent<MeshCollider>().enabled = false;

        Flashlight.transform.SetParent(ItemParent);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PickupText.SetActive(true);

            if (Input.GetKey(KeyCode.E))
            {
                Equip();
                PickupText.SetActive(false);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        PickupText.SetActive(false);
    }
}
