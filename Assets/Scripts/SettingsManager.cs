using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Resolution Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Graphics Settings")]
    [SerializeField] private Slider lodSlider;
    [SerializeField] private TMP_Text lodSliderLabel;
    [SerializeField] private Slider shadowQualitySlider;
    [SerializeField] private TMP_Text shadowQualityLabel;

    private const string KeyWidth = "res_width";
    private const string KeyHeight = "res_height";
    private const string KeyFullscreen = "fullscreen";
    private const string KeyLod = "lod_bias";
    private const string KeyShadows = "shadow_quality";

    private List<Resolution> uniqueResolutions;
    private bool isFullscreen;

    private void Start()
    {
		ApplySavedSettings();
		SetupResolutionSettings();
        SetupLODSlider();
        SetupShadowQualitySlider();
    }

    // ---------- Startup (works even if the settings panel is never opened) ----------

    /// <summary>Called by SettingsBootstrap at launch.</summary>
    public static void ApplySavedSettings()
    {
        if (PlayerPrefs.HasKey(KeyWidth) && PlayerPrefs.HasKey(KeyHeight))
        {
            bool fullscreen = PlayerPrefs.GetInt(KeyFullscreen, Screen.fullScreen ? 1 : 0) == 1;
            Screen.SetResolution(PlayerPrefs.GetInt(KeyWidth), PlayerPrefs.GetInt(KeyHeight), fullscreen);
        }

        if (PlayerPrefs.HasKey(KeyLod))
            QualitySettings.lodBias = PlayerPrefs.GetFloat(KeyLod);

        if (PlayerPrefs.HasKey(KeyShadows))
            ApplyShadowQuality(PlayerPrefs.GetInt(KeyShadows));
    }

    // ---------- Resolution + fullscreen ----------

    private void SetupResolutionSettings()
    {
        // One entry per width x height. Screen.resolutions is sorted lowest to highest
        // refresh rate, so Last() keeps the best refresh rate for each size.
        uniqueResolutions = Screen.resolutions
            .GroupBy(res => new { res.width, res.height })
            .Select(group => group.Last())
            .ToList();

        PopulateResolutionDropdown();
        SetupFullscreenToggle();
    }

    private void PopulateResolutionDropdown()
    {
        // Select the saved resolution if there is one, otherwise the current one.
        int targetWidth = PlayerPrefs.GetInt(KeyWidth, Screen.currentResolution.width);
        int targetHeight = PlayerPrefs.GetInt(KeyHeight, Screen.currentResolution.height);

        var options = new List<string>();
        int selectedIndex = 0;

        for (int i = 0; i < uniqueResolutions.Count; i++)
        {
            Resolution res = uniqueResolutions[i];
            options.Add($"{res.width} x {res.height}");

            if (res.width == targetWidth && res.height == targetHeight)
                selectedIndex = i;
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.SetValueWithoutNotify(selectedIndex);
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    private void SetupFullscreenToggle()
    {
        isFullscreen = PlayerPrefs.GetInt(KeyFullscreen, Screen.fullScreen ? 1 : 0) == 1;
        fullscreenToggle.SetIsOnWithoutNotify(isFullscreen);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    private void SetResolution(int resolutionIndex)
    {
        Resolution selected = uniqueResolutions[resolutionIndex];
        Screen.SetResolution(selected.width, selected.height, isFullscreen);

        PlayerPrefs.SetInt(KeyWidth, selected.width);
        PlayerPrefs.SetInt(KeyHeight, selected.height);
    }

    private void SetFullscreen(bool fullscreen)
    {
        isFullscreen = fullscreen;
        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt(KeyFullscreen, fullscreen ? 1 : 0);
    }

    // ---------- LOD ----------

    private void SetupLODSlider()
    {
        lodSlider.minValue = 0.5f;
        lodSlider.maxValue = 2.0f;
        lodSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(KeyLod, QualitySettings.lodBias));
        UpdateLODLabel(lodSlider.value);

        lodSlider.onValueChanged.AddListener(value =>
        {
            QualitySettings.lodBias = value;
            UpdateLODLabel(value);
            PlayerPrefs.SetFloat(KeyLod, value);
        });
    }

    private void UpdateLODLabel(float value)
    {
        lodSliderLabel.text = $"LOD Bias: {value:F1}";
    }

    // ---------- Shadows ----------
    // NOTE: QualitySettings.shadows only affects the Built-in render pipeline.
    // In URP/HDRP, shadows are set on the pipeline asset instead.

    private void SetupShadowQualitySlider()
    {
        shadowQualitySlider.minValue = 0;
        shadowQualitySlider.maxValue = 2;
        shadowQualitySlider.wholeNumbers = true;
        shadowQualitySlider.SetValueWithoutNotify(PlayerPrefs.GetInt(KeyShadows, (int)QualitySettings.shadows));
        UpdateShadowQualityLabel((int)shadowQualitySlider.value);

        shadowQualitySlider.onValueChanged.AddListener(value =>
        {
            int level = (int)value;
            ApplyShadowQuality(level);
            UpdateShadowQualityLabel(level);
            PlayerPrefs.SetInt(KeyShadows, level);
        });
    }

    private static void ApplyShadowQuality(int level)
    {
        QualitySettings.shadows = level switch
        {
            0 => ShadowQuality.Disable,
            1 => ShadowQuality.HardOnly,
            _ => ShadowQuality.All
        };
    }

    private void UpdateShadowQualityLabel(int level)
    {
        string quality = level switch
        {
            0 => "Disabled",
            1 => "Hard Shadows",
            2 => "Soft Shadows",
            _ => "Unknown"
        };
        shadowQualityLabel.text = $"Shadows: {quality}";
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}
