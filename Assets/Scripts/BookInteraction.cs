using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BookInteraction : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactLayer;

    public GameObject interactionUI; // UI prompt Object 

    private bool lookingAtInteractable = false;

    void Update()
    {
        CheckRaycast();

        if (lookingAtInteractable && Input.GetKeyDown(KeyCode.E))
        {
            interactionUI.SetActive(false); // Hide the UI

            // Load scene
            SceneManager.LoadScene("Main Menu");

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Time.timeScale = 1f;
        }
    }

    void CheckRaycast()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, interactDistance, interactLayer))
        {
            if (hit.collider.GetComponent<Interactable>() != null)
            {
                ShowPrompt();
                return;
            }
        }

        HidePrompt();
    }

    void ShowPrompt()
    {
        if (!lookingAtInteractable)
        {
            lookingAtInteractable = true;
            interactionUI.SetActive(true);
        }
    }

    void HidePrompt()
    {
        if (lookingAtInteractable)
        {
            lookingAtInteractable = false;
            interactionUI.SetActive(false);
        }
    }
}
