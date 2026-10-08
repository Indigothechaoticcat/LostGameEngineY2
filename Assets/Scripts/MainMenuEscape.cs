using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class MainMenuEscape : MonoBehaviour
{
    // FormerlySerializedAs keeps your existing Inspector references after the rename.
    [FormerlySerializedAs("SettingsPanel")] [SerializeField] private GameObject settingsPanel;
    [FormerlySerializedAs("PlayerPanel")]   [SerializeField] private GameObject playerPanel;
    [FormerlySerializedAs("MainMenuPanel")] [SerializeField] private GameObject mainMenuPanel;
    [FormerlySerializedAs("QuitPanel")]     [SerializeField] private GameObject quitPanel;

    [SerializeField] private InputActionReference escapeAction; // drag Player/Escape here

    private void OnEnable()
    {
        escapeAction.action.Enable();
        escapeAction.action.performed += OnEscape;
    }

    private void OnDisable()
    {
        escapeAction.action.performed -= OnEscape;
    }

    private void OnEscape(InputAction.CallbackContext context)
    {
        // 1. Quit prompt open -> close it
        if (quitPanel.activeSelf)
        {
            quitPanel.SetActive(false);
        }
        // 2. A sub-menu is open (main menu hidden) -> go back to main menu
        else if (!mainMenuPanel.activeSelf)
        {
            settingsPanel.SetActive(false);
            playerPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
        // 3. Already on the main menu -> ask to quit
        else
        {
            quitPanel.SetActive(true);
        }
    }
}
