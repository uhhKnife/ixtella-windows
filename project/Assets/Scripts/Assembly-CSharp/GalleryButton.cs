public class GalleryButton : ButtonBase
{
	protected override void Start()
	{
		base.Start();
		SetupOnClickFunction();
	}

	protected override void OnClick()
	{
		base.OnClick();
		PlaySelectSE();
		OpenMenu(MenuCommonData.MenuType.MENU_GALLERY);
	}
}
