public class OptionMenu : MenuBase
{
	protected override void LoadPrefab()
	{
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.BOTTOM_BUTTON);
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.OPTION_BUTTONS);
	}

	protected override void OnButtonClick()
	{
		base.OnButtonClick();
		CloseMenu();
	}
}
