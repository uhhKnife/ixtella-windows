public class HowToPlayCommonMenu : SwipeTextMenuBase
{
	protected override void OnButtonClick()
	{
		base.OnButtonClick();
		GoToNextMenu(GetReturnMenuType());
	}

	private MenuCommonData.MenuType GetReturnMenuType()
	{
		switch (GetMenuType())
		{
		case MenuCommonData.MenuType.MENU_HOW_TO_PLAY_1:
		case MenuCommonData.MenuType.MENU_HOW_TO_PLAY_2:
		case MenuCommonData.MenuType.MENU_HOW_TO_PLAY_4:
		case MenuCommonData.MenuType.MENU_HOW_TO_PLAY_5:
		case MenuCommonData.MenuType.MENU_HOW_TO_PLAY_6:
		case MenuCommonData.MenuType.MENU_HOW_TO_PLAY_7:
			return MenuCommonData.MenuType.MENU_HOW_TO_PLAY_TOP;
		case MenuCommonData.MenuType.MENU_HELP_MEAL_COMMAND:
		case MenuCommonData.MenuType.MENU_HELP_PLAY_COMMAND:
		case MenuCommonData.MenuType.MENU_HELP_DIAPER_COMMAND:
		case MenuCommonData.MenuType.MENU_HELP_SLEEP_COMMAND:
		case MenuCommonData.MenuType.MENU_HELP_TREATMENT:
		case MenuCommonData.MenuType.MENU_HELP_PARAMETER:
		case MenuCommonData.MenuType.MENU_HELP_COSTUME:
			return MenuCommonData.MenuType.MENU_HOW_TO_CARE_COMMAND;
		default:
			return MenuCommonData.MenuType.MENU_HOW_TO_PLAY_TOP;
		}
	}
}
