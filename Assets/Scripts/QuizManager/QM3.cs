using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QM3 : MonoBehaviour
{
    public TMP_Text questionText;     // Your question UI
    public Button[] answerButtons;    // 4 buttons
    public int correctButtonIndex;    // 0-3 for A-D
    public TextMeshProUGUI Checker;

    public GameObject Multiple1;          // The puzzle UI panel inside PadlockQ
    public GameObject Padlock;             // Padlock object in the scene
    public GameObject PadlockUnlockText;   // The "Press [E] to Unlock" UI
    public MonoBehaviour PlayerMovement;   // Player movement script
    public Padlock3 padlockScript;         // Reference to PadlockQ
    public GameObject DoorUI;

    void Start()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; // capture index for listener
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
    }

    void OnAnswerSelected(int index)
    {
        if (index == correctButtonIndex)
        {
            CloseUI();

            Debug.Log("Correct!");
        }
        else
        {
            Checker.text = "Wrong!.";
            Debug.Log("Wrong!");
        }

    }
    public void CloseUI()
    {
        // Hide Padlock puzzle UI
        if (Multiple1 != null)
            Multiple1.SetActive(false);

        // Hide Padlock unlock hint
        if (PadlockUnlockText != null)
            PadlockUnlockText.SetActive(false);

        // Hide Padlock object
        if (Padlock != null)
            Padlock.SetActive(false);

        // Enable player movement
        if (PlayerMovement != null)
            PlayerMovement.enabled = true;

        // Unlock the padlock so the door can now be interacted with
        if (padlockScript != null)
            padlockScript.UnlockPadlock();

        // Show the Door UI
        if (DoorUI != null)
            DoorUI.SetActive(true);

        // Restore cursor state
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
