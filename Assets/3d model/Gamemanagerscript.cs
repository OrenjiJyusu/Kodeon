using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;

public class Gamemanagerscript : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;
    public static bool isPaused;

    void Start()
    {
        if (pauseMenu == null)
        {
            Debug.LogError("pauseMenu is NOT assigned in the inspector!");
        }
        else
        {
            pauseMenu.SetActive(false);
            isPaused = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();

            }
                
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
        Debug.Log("Resume button clicked!");
        pauseMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;

    }
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("First Menu");

    }
    public void QuitGame()
    {
        Application.Quit();
    }
}