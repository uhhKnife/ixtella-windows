public class MealSpecialButton : CareCommandButtonBase
{
	protected MealSpecialButton()
		: base(MenuCommonData.ButtonIconType.MEAL_SPECIAL, "SYS_MENU_1-1_meal_02")
	{
	}

	protected override void OnClick()
	{
		base.OnClick();
		MessageSender.SendWithTag("Player", "PlayAction", PlayerController.PlayerActionType.ACT_EAT_SPECIAL);
	}
}
