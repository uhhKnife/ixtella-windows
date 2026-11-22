public class MealSweetButton : CareCommandButtonBase
{
	protected MealSweetButton()
		: base(MenuCommonData.ButtonIconType.MEAL_SWEET, "SYS_MENU_1-1_meal_01")
	{
	}

	protected override void OnClick()
	{
		base.OnClick();
		MessageSender.SendWithTag("Player", "PlayAction", PlayerController.PlayerActionType.ACT_EAT_SNACK);
	}
}
