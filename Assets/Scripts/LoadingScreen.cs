using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// File name now matches the class name (Unity requires this for MonoBehaviours).
public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private string sceneToLoad; // fallback if you press Play in this scene directly

    private void Start()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(MenuButtons.sceneToLoad))
            sceneToLoad = MenuButtons.sceneToLoad;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("LoadingScreen: no scene to load. Set one in the Inspector or start from the main menu.");
            return;
        }

        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false; // hold the scene until the player is ready

        while (!operation.isDone)
        {
            // Progress stops at 0.9 while activation is held, so rescale to 0-1.
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;

            if (operation.progress >= 0.9f)
            {
                progressText.text = "Press any key to continue";
                if (AnyKeyPressed())
                    operation.allowSceneActivation = true;
            }
            else
            {
                progressText.text = $"{progress * 100f:F0}%";
            }

            yield return null;
        }
    }

    // New Input System replacement for Input.anyKeyDown
    private static bool AnyKeyPressed()
    {
        return (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            || (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            || (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame);
    }
}
