using UnityEngine;

public class FlashLightBlocker : MonoBehaviour
{
    public Light flashlight;          // The actual Light component
    public float maxRange = 20f;      // Normal flashlight range
    public float smoothSpeed = 15f;   // How fast it adjusts to walls

    void Update()
    {
        if (!flashlight.enabled)
            return; // Don’t block light when flashlight is off

        RaycastHit hit;
        float targetRange = maxRange;

        // Start raycast slightly forward so it doesn’t hit player collider
        Vector3 origin = transform.position + transform.forward * 0.05f;

        if (Physics.Raycast(origin, transform.forward, out hit, maxRange))
        {
            targetRange = hit.distance;
        }

        flashlight.range = Mathf.Lerp(flashlight.range, targetRange, Time.deltaTime * smoothSpeed);
    }
}
