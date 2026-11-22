public class BgmSettingToggle : SoundSettingToggleBase
{
	private readonly string BGM_MUTE_STATE_SAVE_KEY = "IsMuteBGM";

	private readonly string BGM_SETTING_TEXT_ID = "SYS_MENU_0-1-2_bgm_setting";

	protected override bool IsMuteSaveData()
	{
		return SaveData.GetInt(BGM_MUTE_STATE_SAVE_KEY) == 1;
	}

	protected override void ReflectMuteState()
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_BGM_CONTROLLER), "SetMuteBgm", IsMuteState());
	}

	protected override void SaveMuteState()
	{
		SaveData.SetInt(BGM_MUTE_STATE_SAVE_KEY, IsMuteState() ? 1 : 0);
	}

	protected override string GetSettingTextId()
	{
		return BGM_SETTING_TEXT_ID;
	}
}
