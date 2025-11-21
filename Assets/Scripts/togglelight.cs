using UnityEngine;

public class ToggleLight : MonoBehaviour
{
    public GameObject LightSource;   // The actual light object
    public bool isEquipped = false;  // Only allow toggle when equipped

    private bool isOn = false;

    void Update()
    {
        if (!isEquipped)
            return;  // Ignore input if not equipped

        if (Input.GetMouseButtonDown(1))
        {
            if (isOn)
                LightOff();
            else
                LightOn();
        }
    }

    void LightOn()
    {
        LightSource.SetActive(true);
        isOn = true;
    }

    void LightOff()
    {
        LightSource.SetActive(false);
        isOn = false;
    }
}
