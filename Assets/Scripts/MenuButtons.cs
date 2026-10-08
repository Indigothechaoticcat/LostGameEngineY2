using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    // Hand-off to the loading scene. Static so it survives the scene change.
    public static string sceneToLoad;

    // Scenes are loaded by name, so reordering Build Settings can't break this.
    [SerializeField] private string loadingSceneName = "Loading";

    public void StartGame(string sceneName)
    {
        sceneToLoad = sceneName;
        LoadSceneByName(loadingSceneName);
    }

    public void LoadSceneByName(string sceneName)
    {
        Time.timeScale = 1f; // never carry a paused state into the next scene
        SceneManager.LoadScene(sceneName);
    }

    // Kept so existing button wiring still works. Prefer LoadSceneByName.
    public void LoadScene(int sceneNo)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneNo);
    }

    // Kept for compatibility. Prefer PauseManager.Resume, which also closes the panel.
    public void ContinueGame()
    {
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // so Quit visibly works in the editor
#else
        Application.Quit();
#endif
    }
}
