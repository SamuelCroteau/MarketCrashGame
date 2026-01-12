using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppController : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        LoadMainMenu();
    }

    public void LoadMainMenu()
    {
        IEnumerator Routine()
        {
            yield return SceneManager.LoadSceneAsync("MainMenu");
        }
        StartCoroutine(Routine());
    }

    public void LoadFirstLevel()
    {
        IEnumerator Routine()
        {
            yield return SceneManager.LoadSceneAsync("Level01Scene");
        }
        StartCoroutine(Routine());
    }

    public void LoadLevel(string levelName)
    {
        IEnumerator Routine()
        {
            yield return SceneManager.LoadSceneAsync(levelName);
            var levelManager = Finder.LevelManager;
        }
        StartCoroutine(Routine());
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadLevelSelector()
    {
        IEnumerator Routine()
        {
            yield return SceneManager.LoadSceneAsync("LevelSelector");
        }
        StartCoroutine(Routine());
    }

    public void LoadCredits()
    {
        IEnumerator Routine()
        {
            yield return SceneManager.LoadSceneAsync("Credits");
        }
        StartCoroutine(Routine());
    }
}