public class CareTreatmentButton : CareCommandButtonBase
{
	protected CareTreatmentButton()
		: base(MenuCommonData.ButtonIconType.TREATMENT, string.Empty)
	{
	}

	protected override void OnClick()
	{
		base.OnClick();
		MessageSender.SendWithTag("Player", "PlayAction", PlayerController.PlayerActionType.ACT_CURE);
	}
}
