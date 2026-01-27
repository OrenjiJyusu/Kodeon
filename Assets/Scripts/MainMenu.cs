using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{

    public void Start()
    {
        Time.timeScale = 1f;
    }
    public void PlayGame()
    {
      SceneManager.LoadScene("Outside");
    }
    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
