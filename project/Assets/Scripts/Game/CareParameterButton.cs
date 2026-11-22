public class CareParameterButton : ButtonBase
{
	protected override void Start()
	{
		base.Start();
		SetupOnClickFunction();
	}

	protected override void OnClick()
	{
		base.OnClick();
		OpenMenu(MenuCommonData.MenuType.MENU_PARAMETER_CHECK);
	}
}
