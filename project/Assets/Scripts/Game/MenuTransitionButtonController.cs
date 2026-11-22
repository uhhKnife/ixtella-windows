public class MenuTransitionButtonController : ButtonBase
{
	public MenuCommonData.MenuType nextMenuType_;

	protected override void Start()
	{
		base.Start();
		SetButtonMainText(MenuCommonData.GetMenuTitleTextId(nextMenuType_));
		SetupOnClickFunction();
	}

	protected override void OnClick()
	{
		base.OnClick();
		OpenMenu(nextMenuType_);
	}
}
