using UnityEngine;

public class MainMenuInitializer : MonoBehaviour
{
    void Start()
    {
        Debug.Log("MainMenuInitializer: resetting Time.timeScale and PauseManager");

        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.Resume();
        }
        else
        {
            Debug.LogWarning("PauseManager instance not found!");
        }
    }
}
