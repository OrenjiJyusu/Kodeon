using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class EnterDungeon : NetworkBehaviour
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
        // Try to get Main Camera, if null, try to find by tag
        if (cam == null) cam = Camera.main;
        if (cam == null) cam = GetComponentInChildren<Camera>(); 
        if (cam == null)
        {
             Transform head = transform.Find("Head");
             if (head != null)
             {
                 Transform mainCam = head.Find("Main Camera");
                 if (mainCam != null) cam = mainCam.GetComponent<Camera>();
             }
        }

        // Dynamic UI Lookup if references are missing (common in Instantiate)
        if (reticle == null || holdIndicator == null || pressEPrompt == null)
        {
            if (isLocalPlayer) // Only need UI for local player
            {
                GameObject canvas = GameObject.Find("Canvas");
                if (canvas != null)
                {
                    if (reticle == null) 
                    {
                        Transform t = canvas.transform.Find("Crosshair");
                        if (t) reticle = t.GetComponent<Image>();
                    }

                    if (holdIndicator == null) 
                    {
                        Transform t = canvas.transform.Find("EnterDungeon");
                        if (t) holdIndicator = t.GetComponent<Image>();
                    }

                    if (pressEPrompt == null) 
                    {
                        Transform t = canvas.transform.Find("DungeonPrompt");
                        if (t) pressEPrompt = t.gameObject;
                    }
                }
            }
        }

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
        if (cam == null) return;
        Vector3 rayStart = cam.transform.position;
        Vector3 rayDir = cam.transform.forward;

        // Debug.DrawRay(rayStart, rayDir * interactDistance, Color.red);

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
                
                // Set locally so the client knows where to spawn in the new scene
                PlayerSpawn.spawnPointName = currentDoor.spawnPointName;
                
                // Call the server to change the scene for EVERYONE
                CmdEnterDungeon(currentDoor.sceneToLoad, currentDoor.spawnPointName);
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

    // Command sent from Client -> Server
    [Command]
    void CmdEnterDungeon(string sceneName, string spawnPoint)
    {
        // Set the spawn point name in the PlayerSpawn statics (or Game Manager)
        PlayerSpawn.spawnPointName = spawnPoint;
        Debug.Log("[EnterDungeon] Requesting scene change to " + sceneName + " at spawn " + spawnPoint);
        
        // Tell NetworkManager to switch scenes for everyone
        NetworkManager.singleton.ServerChangeScene(sceneName);
    }
}
