using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

/// <summary>
/// NEW. Put this on an always-active object in your first scene (MainMenu).
/// It applies saved audio, graphics and key bindings at launch, so settings
/// work even if the settings panel starts disabled.
/// </summary>
public class SettingsBootstrap : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private InputActionAsset inputActions;

    // Start (not Awake) because AudioMixer.SetFloat can be ignored too early in builds.
    private void Start()
    {
        SoundManager.ApplySavedVolumes(audioMixer);
        SettingsManager.ApplySavedSettings();
        ResetAllBindings.LoadBindings(inputActions);
    }
}
