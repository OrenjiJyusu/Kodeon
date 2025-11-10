using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickItems : MonoBehaviour
{
    public GameObject Lamp;
    public Transform ItemParent;
    public GameObject PickupText;


    void Start()
    {
        PickupText.SetActive(false);
        Lamp.GetComponent<Rigidbody>().isKinematic = true;
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
        ItemParent.DetachChildren();
        Lamp.transform.eulerAngles = new Vector3(Lamp.transform.position.x, Lamp.transform.position.z, Lamp.transform.position.y);
        Lamp.GetComponent<Rigidbody>().isKinematic = false;
        Lamp.GetComponent<MeshCollider>().enabled = true;
    }

    void Equip()
    {
        Lamp.GetComponent<Rigidbody>().isKinematic = true;

        Lamp.transform.position = ItemParent.position;
        Lamp.transform.rotation = ItemParent.rotation;

        Lamp.GetComponent<MeshCollider>().enabled = false;

        Lamp.transform.SetParent(ItemParent);
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
