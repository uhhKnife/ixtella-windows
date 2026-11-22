public class PlayableCheckErrorDialog : MessageDialogBase
{
	private readonly string RETRY_TEXT_ID = "SYS_MENU_0_dialog_button_retry";

	private string bodyTextId_;

	protected override void Start()
	{
		base.Start();
		SetButtonVisible(ButtonKind.BUTTON_RIGHT, false);
		SetButtonText(ButtonKind.BUTTON_LEFT, RETRY_TEXT_ID);
		SetOnClickFunction(ButtonKind.BUTTON_LEFT);
	}

	private void OnDestroy()
	{
		if (GetButtonSelectState() == ButtonSelectState.LEFT_SELECTED)
		{
			MessageSender.SendWithTag("PlayableStateChecker", "StartPlayableStateCheck");
		}
	}

	public void SetBodyTextId(string textId)
	{
		bodyTextId_ = textId;
	}

	protected override string GetBodyTextId()
	{
		return bodyTextId_;
	}

	protected override void OnLeftButtonClick()
	{
		base.OnLeftButtonClick();
	}
}
