using UnityEngine;
using UnityEngine.UI;

public abstract class SoundSettingToggleBase : MonoBehaviour
{
	private readonly string TOGGLE_ANIME_TRIGGER_NAME = "ToggleIsOn";

	private readonly string TOGGLE_ON_ANIME_TRIGGER_NAME = "StayOn";

	private readonly string TOGGLE_OFF_ANIME_TRIGGER_NAME = "StayOff";

	private readonly string ON_TEXT_ID = "SYS_MENU_0-1-2_setting_on";

	private readonly string OFF_TEXT_ID = "SYS_MENU_0-1-2_setting_off";

	private readonly string SETTING_TEXT_OBJECT_NAME = "LabelBtn";

	private readonly string ON_AREA_OBJECT_NAME = "AreaOnOff/AreaOn";

	private readonly string OFF_AREA_OBJECT_NAME = "AreaOnOff/AreaOff";

	private Toggle toggle_;

	protected void Start()
	{
		toggle_.isOn = !IsMuteSaveData();
		InitializeAnimation();
		toggle_.onValueChanged.AddListener(OnClick);
		SetSettingText();
		SetOnText();
		SetOffText();
	}

	protected void Awake()
	{
		toggle_ = GetComponent<Toggle>();
	}

	protected abstract bool IsMuteSaveData();

	protected abstract void ReflectMuteState();

	protected abstract void SaveMuteState();

	protected abstract string GetSettingTextId();

	protected void OnClick(bool flag)
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlaySE", SeController.SeType.SE_SELECT_BUTTON);
		UpdateAnimation();
		ReflectMuteState();
		SaveMuteState();
	}

	protected bool IsMuteState()
	{
		return !toggle_.isOn;
	}

	protected void UpdateAnimation()
	{
		Animator animator = GetComponent<Animator>();
		animator.SetBool(TOGGLE_ANIME_TRIGGER_NAME, toggle_.isOn);
	}

	private void InitializeAnimation()
	{
		Animator animator = GetComponent<Animator>();
		animator.SetBool(TOGGLE_ANIME_TRIGGER_NAME, toggle_.isOn);
		animator.SetTrigger(toggle_.isOn ? TOGGLE_ON_ANIME_TRIGGER_NAME : TOGGLE_OFF_ANIME_TRIGGER_NAME);
	}

	private void SetSettingText()
	{
		GameObject textObject = transform.Find(SETTING_TEXT_OBJECT_NAME).gameObject;
		textObject.GetComponent<Text>().text = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(GetSettingTextId());
	}

	private void SetOnText()
	{
		GameObject onArea = transform.Find(ON_AREA_OBJECT_NAME).gameObject;
		onArea.GetComponentInChildren<Text>().text = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(ON_TEXT_ID);
	}

	private void SetOffText()
	{
		GameObject offArea = transform.Find(OFF_AREA_OBJECT_NAME).gameObject;
		offArea.GetComponentInChildren<Text>().text = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(OFF_TEXT_ID);
	}
}
