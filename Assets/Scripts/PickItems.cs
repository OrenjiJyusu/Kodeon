using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickItems : MonoBehaviour
{
    public Transform ItemParent;     // Item hold position (on camera)
    public GameObject PickupText;    // UI text
    public float pickupDistance = 3f;

    private GameObject currentItem = null;
    private GameObject hoveredItem = null;

    private Vector3 originalScale;

    void Start()
    {
        PickupText.SetActive(false);
    }

    void Update()
    {
        ReticlePickup();

        if (Input.GetKeyDown(KeyCode.G))
            Drop();
    }

    void ReticlePickup()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupDistance)
            && hit.collider.CompareTag("Pickup"))
        {
            hoveredItem = hit.collider.gameObject;
            PickupText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                Equip(hoveredItem);
                PickupText.SetActive(false);
            }
        }
        else
        {
            hoveredItem = null;
            PickupText.SetActive(false);
        }
    }

    void Equip(GameObject item)
    {
        currentItem = item;
        originalScale = item.transform.localScale;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        Collider[] cols = item.GetComponentsInChildren<Collider>();

        // Disable physics while held
        foreach (Collider c in cols)
            c.enabled = false;

        rb.isKinematic = true;

        // Attach to hand
        item.transform.SetParent(ItemParent);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        // Enable flashlight if it has one
        ToggleLight lightScript = item.GetComponent<ToggleLight>();
        if (lightScript != null)
            lightScript.isEquipped = true;
    }

    void Drop()
    {
        if (currentItem == null)
            return;

        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        Collider[] cols = currentItem.GetComponentsInChildren<Collider>();

        // Detach first
        currentItem.transform.SetParent(null);

        // Restore physics
        foreach (Collider c in cols)
            c.enabled = true;

        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Drop position in front of camera
        Transform cam = Camera.main.transform;
        Vector3 dropPos = cam.position + cam.forward * 0.7f + Vector3.up * 0.1f;
        currentItem.transform.position = dropPos;

        currentItem.transform.localScale = originalScale;

        // Small push and random torque
        rb.AddForce(cam.forward * 1.2f, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 1f, ForceMode.Impulse);

        // Disable flashlight if it has one
        ToggleLight lightScript = currentItem.GetComponent<ToggleLight>();
        if (lightScript != null)
            lightScript.isEquipped = false;

        currentItem = null;
    }
}
