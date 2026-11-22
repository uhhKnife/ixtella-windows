public class CareSleepButton : CareCommandButtonBase
{
	protected CareSleepButton()
		: base(MenuCommonData.ButtonIconType.SLEEP, string.Empty)
	{
	}

	protected override void OnClick()
	{
		base.OnClick();
		MessageSender.SendWithTag("Player", "PlayAction", PlayerController.PlayerActionType.ACT_SLEEP);
	}
}
