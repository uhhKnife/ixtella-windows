public class HowToPlayTopMenu : ScrollButtonMenuBase
{
	protected override void Initialize()
	{
		base.Initialize();
		for (int i = 11; i <= 17; i++)
		{
			AddButton((MenuCommonData.MenuType)i, false);
		}
	}

	protected override void OnButtonClick()
	{
		base.OnButtonClick();
		GoToNextMenu(MenuCommonData.MenuType.MENU_OPTION_TOP);
	}
}
