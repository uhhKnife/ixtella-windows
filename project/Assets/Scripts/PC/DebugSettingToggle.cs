public class DebugSettingToggle : SoundSettingToggleBase
{
    private readonly string DEBUG_SETTING_TEXT_ID = "SYS_MENU_0-1-2_debug_setting";

    // The base class treats "muted" as toggle off. Here off = debug OFF.
    protected override bool IsMuteSaveData()
    {
        return !DebugMode.Enabled;
    }

    protected override void ReflectMuteState()
    {
    }

    protected override void SaveMuteState()
    {
        DebugMode.Set(!IsMuteState());
    }

    protected override string GetSettingTextId()
    {
        return DEBUG_SETTING_TEXT_ID;
    }
}
