using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Replaces InGameEscape. One place owns "paused or not", so the Escape key
/// and the Continue button both call the same method.
/// </summary>
public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private InputActionReference escapeAction; // drag Player/Escape here
    [SerializeField] private bool lockCursorWhilePlaying = false; // tick for first/third person games

    public bool IsPaused { get; private set; }

    private void OnEnable()
    {
        escapeAction.action.Enable();
        escapeAction.action.performed += OnEscape;
    }

    private void OnDisable()
    {
        // Only unsubscribe. Other scripts may share this action, so we don't Disable() it.
        escapeAction.action.performed -= OnEscape;
    }

    private void Start()
    {
        SetPaused(false); // always start in a known state
    }

    private void OnDestroy()
    {
        // Safety net: never leave the next scene frozen.
        Time.timeScale = 1f;
    }

    private void OnEscape(InputAction.CallbackContext context) => TogglePause();

    public void TogglePause() => SetPaused(!IsPaused);

    // Hook the Continue button's OnClick to this.
    public void Resume() => SetPaused(false);

    public void SetPaused(bool paused)
    {
        IsPaused = paused;
        pauseMenu.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;

        if (paused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (lockCursorWhilePlaying)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
