// [PC port] Debug row in the options menu (BtnOption_8 in Resources/AreaButtonVertical, cloned from
// the SE row by Assets/Editor/PortLocalize.cs). ON/OFF labels and animation come from the base class.
public class DebugSettingToggle : SoundSettingToggleBase
{
    private readonly string DEBUG_SETTING_TEXT_ID = "SYS_MENU_0-1-2_debug_setting";

    // The base class treats "muted" as toggle off. Here off = debug OFF.
    protected override bool IsMuteSaveData()
    {
        return !PortDebug.Enabled;
    }

    protected override void ReflectMuteState()
    {
    }

    protected override void SaveMuteState()
    {
        PortDebug.Set(!IsMuteState());
    }

    protected override string GetSettingTextId()
    {
        return DEBUG_SETTING_TEXT_ID;
    }
}
