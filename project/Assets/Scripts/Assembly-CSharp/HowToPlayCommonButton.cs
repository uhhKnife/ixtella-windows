public class HowToPlayCommonButton : MenuTransitionButtonController
{
	public void SetButtonParam(MenuCommonData.MenuType menuType, bool isIconActive)
	{
		nextMenuType_ = menuType;
		SetIconActive(isIconActive);
		if (isIconActive)
		{
			SetIcon(MenuCommonData.GetButtonIconType(menuType));
		}
	}
}
