using UnityEngine;
using TMPro;

public class ChestInteraction : MonoBehaviour
{
    [Header("References")]
    public Animator chestAnimator;
    public Animator bookAnimator;
    //public AudioSource chestSound;
    public TextMeshProUGUI interactionText;

    [Header("Settings")]
    public float interactDistance = 3f;
    public LayerMask interactLayer;

    bool chestOpened = false;
    bool lookingAtChest = false;

    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        // Raycast
        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            if (hit.collider.gameObject == gameObject && !chestOpened)
            {
                // Show UI
                if (!lookingAtChest)
                {
                    lookingAtChest = true;
                    interactionText.gameObject.SetActive(true);
                }

                // Press E to open
                if (Input.GetKeyDown(KeyCode.E))
                {
                    chestOpened = true;

                    // Play animations
                    chestAnimator.SetTrigger("OpenTrigger");
                    bookAnimator.SetTrigger("PlayTrigger");

                    // Play sound
                   /* if (chestSound != null)
                        chestSound.Play();*/

                    // Hide UI
                    interactionText.gameObject.SetActive(false);
                }

                return;
            }
        }

        // If looking away, hide UI
        if (lookingAtChest)
        {
            lookingAtChest = false;
            interactionText.gameObject.SetActive(false);
        }
    }
}
