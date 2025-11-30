using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("[PlayerSpawner] Spawn point at OnEnable → " + PlayerSpawn.spawnPointName);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!string.IsNullOrEmpty(PlayerSpawn.spawnPointName))
        {
            // Look for spawn object by tag
            GameObject spawn = GameObject.FindWithTag(PlayerSpawn.spawnPointName);

            if (spawn != null)
            {
                // Move player to spawn
                transform.position = spawn.transform.position;
                transform.rotation = spawn.transform.rotation;

                Debug.Log($"[PlayerSpawner] Player spawned at '{spawn.name}' in scene '{scene.name}' using tag '{PlayerSpawn.spawnPointName}'");

                // Optional: reset after use
                PlayerSpawn.spawnPointName = null;
            }
            else
            {
                Debug.LogWarning($"[PlayerSpawner] Spawn point with tag '{PlayerSpawn.spawnPointName}' NOT FOUND in scene '{scene.name}'. Check that the object exists and is correctly tagged.");
            }
        }
        else
        {
            Debug.Log($"[PlayerSpawner] No spawn point set for scene '{scene.name}', player stays in place.");
        }
    }
}
