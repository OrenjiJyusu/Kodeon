using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private BackendCommunicator backend;
    public GameObject pauseMenuUI;
    public MonoBehaviour[] disableOnPause;

    public bool IsPaused { get; private set; }
    public static PauseManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        if (backend == null) backend = FindObjectOfType<BackendCommunicator>();
        pauseMenuUI.SetActive(false);
        IsPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        IsPaused = true;
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        IsPaused = false;
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitToMainMenu()
    {
        // Check if we are Host and need to unregister
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            // We are Host
            string myServerId = PlayerPrefs.GetString("MyServerId", "");
            if (!string.IsNullOrEmpty(myServerId)) {
                backend.UnregisterServer(myServerId, () => Debug.Log("Server Unregistered"), (e) => Debug.LogError(e));
            }
            NetworkManager.singleton.StopHost();
        }
        else
        {
            // Just Client
            NetworkManager.singleton.StopClient();
        }

        // Scene change happens automatically by NetworkManager -> Offline Scene
    }
}
