using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Image staminaFill; // assign your green Image here

    private float maxStamina;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetMaxStamina(float value)
    {
        maxStamina = value;
        staminaFill.fillAmount = 1f; // full bar
    }

    public void SetStamina(float currentStamina)
    {
        staminaFill.fillAmount = currentStamina / maxStamina;
    }
}

