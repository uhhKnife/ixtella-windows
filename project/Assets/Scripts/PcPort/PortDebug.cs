// Options > Debug. Replaces Debug.isDebugBuild in the game code. Off by default.
using UnityEngine;

public static class PortDebug
{
    private const string PREF_KEY = "PcPortDebug";

    private static bool loaded_;

    private static bool enabled_;

    private static GameObject root_;

    public static bool Enabled
    {
        get
        {
            if (!loaded_)
            {
                enabled_ = PlayerPrefs.GetInt(PREF_KEY, 0) == 1;
                loaded_ = true;
            }
            return enabled_;
        }
    }

    public static void Set(bool on)
    {
        enabled_ = on;
        loaded_ = true;
        PlayerPrefs.SetInt(PREF_KEY, on ? 1 : 0);
        PlayerPrefs.Save();
        if (!on)
        {
            // Leave no debug state behind: normal speed, overlay and panel closed.
            Time.timeScale = 1f;
            DateTimeObserber dto = Object.FindAnyObjectByType<DateTimeObserber>();
            if (dto != null)
            {
                dto.timeRate_ = 1f;
                dto.isTimePause_ = false;
            }
            if (root_ != null)
            {
                root_.GetComponent<DebugMenu>().isShow_ = false;
            }
            PortDebugPanel.Close();
        }
    }

    // Use debug mode for this run only, without saving it (PortTour).
    public static void OverrideForSession(bool on)
    {
        enabled_ = on;
        loaded_ = true;
    }

    // Replaces the original LoadScene("DebugMenu") in StaticObjectsManager.Awake.
    public static void CreateObjects()
    {
        if (root_ != null)
        {
            return;
        }
        root_ = new GameObject("DebugMenu");
        Object.DontDestroyOnLoad(root_);
        root_.AddComponent<FpsObserber>();
        root_.AddComponent<DebugMenu>();
        root_.AddComponent<DebugPlayerActionSender>();
        Assets.Scripts.Common.DebugConsole console = root_.AddComponent<Assets.Scripts.Common.DebugConsole>();
        console.Show = false;
        root_.AddComponent<PortDebugPanel>();
    }
}
