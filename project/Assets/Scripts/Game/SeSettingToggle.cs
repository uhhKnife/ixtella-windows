public class SeSettingToggle : SoundSettingToggleBase
{
	private readonly string SE_MUTE_STATE_SAVE_KEY = "IsMuteSE";

	private readonly string SE_SETTING_TEXT_ID = "SYS_MENU_0-1-2_se_setting";

	protected override bool IsMuteSaveData()
	{
		return SaveData.GetInt(SE_MUTE_STATE_SAVE_KEY) == 1;
	}

	protected override void ReflectMuteState()
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "SetMuteSE", IsMuteState());
	}

	protected override void SaveMuteState()
	{
		SaveData.SetInt(SE_MUTE_STATE_SAVE_KEY, IsMuteState() ? 1 : 0);
	}

	protected override string GetSettingTextId()
	{
		return SE_SETTING_TEXT_ID;
	}
}
