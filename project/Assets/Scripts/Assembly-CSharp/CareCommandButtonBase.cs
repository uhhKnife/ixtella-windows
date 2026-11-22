public abstract class CareCommandButtonBase : ButtonBase
{
	private readonly MenuCommonData.ButtonIconType buttonType_;

	private readonly string buttonTextId_;

	protected CareCommandButtonBase(MenuCommonData.ButtonIconType buttonType, string buttonTextId = "")
	{
		buttonType_ = buttonType;
		buttonTextId_ = buttonTextId;
	}

	protected override void Start()
	{
		base.Start();
		if (!string.IsNullOrEmpty(buttonTextId_))
		{
			SetButtonMainText(buttonTextId_);
		}
		if (IsDialogButton())
		{
			SetIconActive(true);
			SetIcon(buttonType_);
		}
		SetupOnClickFunction();
	}

	protected override void OnClick()
	{
		base.OnClick();
		if (IsDialogButton())
		{
			base.gameObject.transform.parent.parent.gameObject.SendMessage("CloseDialog");
		}
	}

	protected PlayerStatusManager.PlayerStatus GetPlayerStatus()
	{
		return SaveData.GetClass("PlayerStatus", new PlayerStatusManager.PlayerStatus());
	}

	private bool IsDialogButton()
	{
		switch (buttonType_)
		{
		case MenuCommonData.ButtonIconType.MEAL_MILK:
		case MenuCommonData.ButtonIconType.MEAL_SWEET:
		case MenuCommonData.ButtonIconType.MEAL_SPECIAL:
		case MenuCommonData.ButtonIconType.PLAY_BALL:
		case MenuCommonData.ButtonIconType.PLAY_BOOK:
		case MenuCommonData.ButtonIconType.PLAY_SONG:
			return true;
		default:
			return false;
		}
	}
}
