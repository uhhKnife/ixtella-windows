public class UserPolicyMenu : ScrollTextMenuBase
{
	private readonly string TEXT_FILENAME = "ixtella_user_policy";

	private readonly string CHECK_BOX_TEXT_ID = "SYS_MENU_0-5_check_message";

	private readonly string AGREE_BUTTON_TEXT_ID = "SYS_MENU_0-5_button_agree";

	private readonly string DISAGREE_BUTTON_TEXT_ID = "SYS_MENU_0-5_button_disagree";

	private ScrollAreaController textController_;

	private MenuCommonButton subButton_;

	private bool isAgreedUserPolicy_;

	protected override void Awake()
	{
		isAgreedUserPolicy_ = MenuCommonData.IsAgreedUserPolicy();
		base.Awake();
	}

	protected override void LoadPrefab()
	{
		base.LoadPrefab();
		if (!isAgreedUserPolicy_)
		{
			AddPrefabLoadRequest(MenuCommonData.ChildObjectType.BOTTOM_BUTTON_SUB);
		}
	}

	protected override void Initialize()
	{
		base.Initialize();
		textController_ = GetChildObject(MenuCommonData.ChildObjectType.SCROLL_TEXT_CONTROLLER).GetComponent<ScrollAreaController>();
		textController_.SetScrollText(GetTextAsset().text);
		UnloadTextAsset();
		if (isAgreedUserPolicy_)
		{
			textController_.SetChildObjectActive(ScrollAreaController.ChildObjectType.SCROLL_TEXT, true);
			textController_.SetChildObjectActive(ScrollAreaController.ChildObjectType.CHECK_BOX, false);
			textController_.SetChildObjectActive(ScrollAreaController.ChildObjectType.SCROLL_BUTTON, false);
			return;
		}
		textController_.SetChildObjectActive(ScrollAreaController.ChildObjectType.SCROLL_TEXT, true);
		textController_.SetChildObjectActive(ScrollAreaController.ChildObjectType.CHECK_BOX, true);
		textController_.SetChildObjectActive(ScrollAreaController.ChildObjectType.SCROLL_BUTTON, false);
		textController_.SetCheckBoxText(SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(CHECK_BOX_TEXT_ID));
		textController_.SetOnCheckChangedFunction(OnCheckChanged);
		SetupSubButton();
	}

	protected override string GetButtonTextId()
	{
		return (!isAgreedUserPolicy_) ? DISAGREE_BUTTON_TEXT_ID : base.GetButtonTextId();
	}

	protected override void OnButtonClick()
	{
		base.OnButtonClick();
		if (isAgreedUserPolicy_)
		{
			GoToNextMenu(MenuCommonData.MenuType.MENU_OPTION_TOP);
		}
		else
		{
			CloseMenu();
		}
	}

	private void OnSubButtonClick()
	{
		if (MenuCommonData.CanExecuteButtonOnClick(gameObject.scene.name))
		{
			subButton_.PlaySelectSE();
			SaveData.SetInt("IsAgreedUserPolicy", 1);
			CloseMenu();
		}
	}

	protected override string GetTextFilename()
	{
		return TEXT_FILENAME;
	}

	private void OnCheckChanged(bool flag)
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlaySE", SeController.SeType.SE_SELECT_BUTTON);
		subButton_.SetInteractable(flag);
	}

	private void SetupSubButton()
	{
		subButton_ = GetChildObject(MenuCommonData.ChildObjectType.BOTTOM_BUTTON_SUB).AddComponent<MenuCommonButton>();
		subButton_.SetButtonMainText(AGREE_BUTTON_TEXT_ID);
		subButton_.SetInteractable(false);
		subButton_.SetOnClickFunction(OnSubButtonClick);
	}
}
