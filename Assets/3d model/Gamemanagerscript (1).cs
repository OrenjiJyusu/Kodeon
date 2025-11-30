using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamemanagerscript : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] GameObject pauseMenu;     // Your pause panel
   // [SerializeField] GameObject confirmPanel;  // YES/NO confirmation panel

    public static bool isPaused;

    void Start()
    {
        // Hide panels on start
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    void Update()
    {
        // Press ESC to pause/unpause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    private void PauseGame()
    {
        pauseMenu.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void Back()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main menu");
    }
}
