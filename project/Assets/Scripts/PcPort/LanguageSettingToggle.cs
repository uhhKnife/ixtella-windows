// Toggle "on" = English, "off" = 日本語.
using UnityEngine;
using UnityEngine.UI;

public class LanguageSettingToggle : SoundSettingToggleBase
{
    private readonly string LANGUAGE_SETTING_TEXT_ID = "SYS_MENU_0-1-2_language_setting";

    private readonly string ON_AREA_OBJECT_NAME = "AreaOnOff/AreaOn";

    private readonly string OFF_AREA_OBJECT_NAME = "AreaOnOff/AreaOff";

    // Each language is always shown in its own script so it can be found either way.
    private readonly string ENGLISH_LABEL = "English";

    private readonly string JAPANESE_LABEL = "日本語";

    private new void Start()
    {
        base.Start();
        SetAreaLabel(ON_AREA_OBJECT_NAME, ENGLISH_LABEL);
        SetAreaLabel(OFF_AREA_OBJECT_NAME, JAPANESE_LABEL);
    }

    private void SetAreaLabel(string path, string label)
    {
        Transform area = transform.Find(path);
        if (area != null)
        {
            Text text = area.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = label;
            }
        }
    }

    // The base class treats "muted" as toggle off. Here off = Japanese.
    protected override bool IsMuteSaveData()
    {
        return !Localization.IsEnglish;
    }

    protected override void ReflectMuteState()
    {
    }

    protected override void SaveMuteState()
    {
        Localization.SwitchAndReloadTitle(IsMuteState() ? Localization.Language.Japanese : Localization.Language.English);
    }

    protected override string GetSettingTextId()
    {
        return LANGUAGE_SETTING_TEXT_ID;
    }
}
