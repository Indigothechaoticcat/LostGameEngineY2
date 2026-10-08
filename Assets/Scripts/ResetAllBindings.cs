using UnityEngine;
using UnityEngine.InputSystem;

// Renamed from ResetAllBndings (typo). Re-link the button's OnClick after swapping the script.
public class ResetAllBindings : MonoBehaviour
{
    private const string BindingsKey = "rebinds";

    [SerializeField] private InputActionAsset inputActions;

    // Hook to the Reset button's OnClick.
    public void ResetBindings()
    {
        foreach (InputActionMap map in inputActions.actionMaps)
            map.RemoveAllBindingOverrides();

        PlayerPrefs.DeleteKey(BindingsKey);
        PlayerPrefs.Save();
    }

    // Call this whenever a rebind completes (e.g. in your rebind operation's OnComplete).
    public static void SaveBindings(InputActionAsset asset)
    {
        PlayerPrefs.SetString(BindingsKey, asset.SaveBindingOverridesAsJson());
        PlayerPrefs.Save();
    }

    // Call this at startup (SettingsBootstrap does it for you).
    public static void LoadBindings(InputActionAsset asset)
    {
        string json = PlayerPrefs.GetString(BindingsKey, string.Empty);
        if (!string.IsNullOrEmpty(json))
            asset.LoadBindingOverridesFromJson(json);
    }
}
