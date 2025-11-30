using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;

    public Image fadePanel;
    public float fadeDuration = 2f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (fadePanel != null)
            {
                fadePanel.gameObject.SetActive(true);
                fadePanel.color = new Color(0, 0, 0, 0f);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Fade IN after scene loads
        StartCoroutine(Fade(1f, 0f, fadeDuration));
    }

    public void FadeOutThenLoad(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        // Fade out
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // Load scene ASYNC to avoid timing issues
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone) yield return null;
    }

    private IEnumerator Fade(float fromAlpha, float toAlpha, float duration)
    {
        if (fadePanel == null) yield break;

        float timer = 0f;
        Color color = fadePanel.color;
        color.a = fromAlpha;
        fadePanel.color = color;
        fadePanel.gameObject.SetActive(true);

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime; // Use unscaledDeltaTime here!
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, timer / duration);
            color.a = alpha;
            fadePanel.color = color;
            yield return null;
        }

        color.a = toAlpha;
        fadePanel.color = color;

        if (toAlpha == 0f) fadePanel.gameObject.SetActive(false);
    }

}
