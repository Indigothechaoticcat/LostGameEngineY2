using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Volume Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [FormerlySerializedAs("UiSoundSlider")] [SerializeField] private Slider uiSoundSlider;
    [SerializeField] private Slider voiceSlider;

    [Header("Volume Labels")]
    [SerializeField] private TMP_Text masterLabel;
    [SerializeField] private TMP_Text musicLabel;
    [SerializeField] private TMP_Text sfxLabel;
    [FormerlySerializedAs("ambientLabel")] [SerializeField] private TMP_Text uiSoundLabel;
    [SerializeField] private TMP_Text voiceLabel;

    // These must match the exposed parameter names in the AudioMixer EXACTLY.
    // (Spelling kept as-is, including "Dialoge", so your existing mixer still works.)
    public const string MasterVolume = "MasterVolume";
    public const string MusicVolume = "MusicVolume";
    public const string SfxVolume = "SFXVolume";
    public const string UiSoundVolume = "UiSoundVolume";
    public const string VoiceVolume = "DialogeVolume";

    private static readonly string[] AllParameters =
        { MasterVolume, MusicVolume, SfxVolume, UiSoundVolume, VoiceVolume };

    private const float DefaultVolume = 0.75f;

    private void Start()
    {
        Setup(MasterVolume, masterSlider, masterLabel);
        Setup(MusicVolume, musicSlider, musicLabel);
        Setup(SfxVolume, sfxSlider, sfxLabel);
        Setup(UiSoundVolume, uiSoundSlider, uiSoundLabel);
        Setup(VoiceVolume, voiceSlider, voiceLabel);
    }

    private void Setup(string parameter, Slider slider, TMP_Text label)
    {
        float saved = PlayerPrefs.GetFloat(parameter, DefaultVolume);

        // Set the slider silently, then apply once, then start listening.
        slider.SetValueWithoutNotify(saved);
        UpdateLabel(label, saved);
        ApplyToMixer(audioMixer, parameter, saved);

        slider.onValueChanged.AddListener(value =>
        {
            ApplyToMixer(audioMixer, parameter, value);
            UpdateLabel(label, value);
            PlayerPrefs.SetFloat(parameter, value);
        });
    }

    private static void UpdateLabel(TMP_Text label, float value)
    {
        label.text = $"{Mathf.RoundToInt(value * 100)}%";
    }

    // Sliders are linear (0-1) but decibels are logarithmic, so convert.
    // This makes the slider "feel" even across its whole range.
    private static void ApplyToMixer(AudioMixer mixer, string parameter, float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f; // 0 -> -80 dB, 1 -> 0 dB
        mixer.SetFloat(parameter, dB);
    }

    /// <summary>
    /// Call once at startup (see SettingsBootstrap) so saved volumes apply
    /// even if the settings panel is never opened.
    /// </summary>
    public static void ApplySavedVolumes(AudioMixer mixer)
    {
        foreach (string parameter in AllParameters)
            ApplyToMixer(mixer, parameter, PlayerPrefs.GetFloat(parameter, DefaultVolume));
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}
