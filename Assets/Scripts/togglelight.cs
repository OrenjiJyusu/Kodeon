using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class togglelight : MonoBehaviour
{
    public GameObject LightSource;
    private bool isOn = false;

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isOn) LightOn();
            else LightOff();
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
