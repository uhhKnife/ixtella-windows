public class GameResetComfirmDialog : MessageDialogBase
{
	private readonly string bodyTextId_ = "SYS_MENU_0-1-2_dialog_01";

	protected override void Start()
	{
		base.Start();
		SetButtonVisible(ButtonKind.BUTTON_RIGHT, false);
		SetOnClickFunction(ButtonKind.BUTTON_LEFT);
	}

	protected override string GetBodyTextId()
	{
		return bodyTextId_;
	}
}
