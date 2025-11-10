using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Hotbar : MonoBehaviour
{
    [Header("Assign your hotbar slot Animators here")]
    public Animator[] hotbarAnimators;

    private int currentIndex = -1; // track which slot is selected

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= hotbarAnimators.Length) return;

        // If pressing the currently selected slot → deselect it
        if (index == currentIndex)
        {
            Animator anim = hotbarAnimators[index];
            if (anim != null)
                anim.SetTrigger("Deselect");

            currentIndex = -1; // no slot selected now
            return;
        }

        // Deselect previously selected slot
        if (currentIndex != -1)
        {
            Animator prevAnim = hotbarAnimators[currentIndex];
            if (prevAnim != null)
                prevAnim.SetTrigger("Deselect");
        }

        // Select the new slot
        Animator newAnim = hotbarAnimators[index];
        if (newAnim != null)
            newAnim.SetTrigger("Select");

        currentIndex = index;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
    }
}


