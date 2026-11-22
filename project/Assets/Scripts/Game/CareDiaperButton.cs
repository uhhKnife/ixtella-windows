public class CareDiaperButton : CareCommandButtonBase
{
	protected CareDiaperButton()
		: base(MenuCommonData.ButtonIconType.DIAPER, string.Empty)
	{
	}

	protected override void OnClick()
	{
		base.OnClick();
		MessageSender.SendWithTag("Player", "PlayAction", PlayerController.PlayerActionType.ACT_DIAPER);
	}
}
