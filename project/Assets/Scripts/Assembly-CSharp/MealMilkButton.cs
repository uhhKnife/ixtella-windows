public class MealMilkButton : CareCommandButtonBase
{
	protected MealMilkButton()
		: base(MenuCommonData.ButtonIconType.MEAL_MILK, "SYS_MENU_1-1_meal_00")
	{
	}

	protected override void OnClick()
	{
		base.OnClick();
		MessageSender.SendWithTag("Player", "PlayAction", PlayerController.PlayerActionType.ACT_DRINK_MILK);
	}
}
