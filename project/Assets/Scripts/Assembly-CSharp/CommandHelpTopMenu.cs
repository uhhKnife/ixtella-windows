public class CommandHelpTopMenu : ScrollButtonMenuBase
{
	protected override void Initialize()
	{
		base.Initialize();
		for (int i = 18; i <= 24; i++)
		{
			AddButton((MenuCommonData.MenuType)i, true);
		}
	}

	protected override void OnButtonClick()
	{
		base.OnButtonClick();
		GoToNextMenu(MenuCommonData.MenuType.MENU_HOW_TO_PLAY_TOP);
	}
}
