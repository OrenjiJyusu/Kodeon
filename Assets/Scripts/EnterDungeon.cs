using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class EnterDungeon : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f;
    public float holdTime = 1.5f;

    [Header("UI Elements")]
    public Image reticle;
    public Image holdIndicator;
    public GameObject pressEPrompt;

    private float holdTimer = 0f;
    private DoorTarget currentDoor;
    private bool isLoading = false;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        if (reticle != null) reticle.enabled = true;
        if (holdIndicator != null) holdIndicator.gameObject.SetActive(false);
        if (pressEPrompt != null) pressEPrompt.SetActive(false);
    }

    void Update()
    {
        if (isLoading) return;

        CheckForDoor();
        HandleInput();
    }

    void CheckForDoor()
    {
        RaycastHit hit;
        Vector3 rayStart = cam.transform.position;
        Vector3 rayDir = cam.transform.forward;

        Debug.DrawRay(rayStart, rayDir * interactDistance, Color.red);

        if (Physics.Raycast(rayStart, rayDir, out hit, interactDistance))
        {
            DoorTarget door = hit.collider.GetComponent<DoorTarget>();
            if (door != null)
            {
                currentDoor = door;
                if (pressEPrompt != null) pressEPrompt.SetActive(true);
                return;
            }
        }

        // No door detected
        if (currentDoor != null)
        {
            ResetHold();
            if (pressEPrompt != null) pressEPrompt.SetActive(false);
            currentDoor = null;
        }
    }

    void HandleInput()
    {
        if (currentDoor == null) return;

        if (Input.GetKey(KeyCode.E))
        {
            holdTimer += Time.deltaTime;

            if (holdIndicator != null)
            {
                holdIndicator.gameObject.SetActive(true);
                holdIndicator.fillAmount = holdTimer / holdTime;
            }

            if (holdTimer >= holdTime)
            {
                isLoading = true;

                // ✅ Set spawn point BEFORE scene load
                PlayerSpawn.spawnPointName = currentDoor.spawnPointName;
                Debug.Log("[EnterDungeon] Setting spawn point → " + PlayerSpawn.spawnPointName);

                // ✅ Directly load the scene (no fade)
                SceneManager.LoadScene(currentDoor.sceneToLoad);
            }
        }
        else
        {
            ResetHold();
        }
    }

    void ResetHold()
    {
        holdTimer = 0f;
        if (holdIndicator != null)
        {
            holdIndicator.fillAmount = 0f;
            holdIndicator.gameObject.SetActive(false);
        }
    }
}
