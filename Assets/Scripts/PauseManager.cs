using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;        // Assign your pause menu Canvas here
    public MonoBehaviour[] disableOnPause; // Add all scripts you want to disable when paused
    private bool isPaused = false;
    public static PauseManager Instance;
    void Start()
    {
        pauseMenuUI.SetActive(false);
        isPaused = false; // menu is hidden → game is not paused
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC pressed");
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f; // Freeze the game

        Debug.Log("Game Paused: " + isPaused);

        // Show and unlock cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Disable player/camera scripts
        foreach (var script in disableOnPause)
        {
            script.enabled = false;
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f; // Resume game

        Debug.Log("Game Resumed: " + isPaused);

        // Hide and lock cursor again
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Re-enable player/camera scripts
        foreach (var script in disableOnPause)
        {
            script.enabled = true;
        }
    }

    public void QuitToMainMenu()
    {
        // Reset the pause state explicitly before loading the main menu
        isPaused = false;

        // Reset time scale (to avoid pausing when loading the new scene)
        Time.timeScale = 1f; // Reset time scale to normal

        // Optional: restore cursor visibility and unlock state
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Load the main menu scene (you may need to adjust the scene index/name)
        SceneManager.LoadScene(1); // Replace with your main menu scene index or name
    }
    void OnEnable()
    {
        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");

        if (scene.name == "Main Menu")
        {
            Debug.Log("Main Menu loaded, resetting pause state.");
            isPaused = false;
            Time.timeScale = 1f;
            pauseMenuUI.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            foreach (var script in disableOnPause)
            {
                script.enabled = true;
            }
        }
    }
}

