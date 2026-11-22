using UnityEngine;

public abstract class ScrollButtonMenuBase : MenuBase
{
	private ScrollAreaController areaController_;

	protected override void LoadPrefab()
	{
		base.LoadPrefab();
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.SCROLL_TEXT_CONTROLLER);
	}

	protected override void Initialize()
	{
		base.Initialize();
		areaController_ = GetChildObject(MenuCommonData.ChildObjectType.SCROLL_TEXT_CONTROLLER).GetComponent<ScrollAreaController>();
		areaController_.SetChildObjectActive(ScrollAreaController.ChildObjectType.SCROLL_TEXT, false);
		areaController_.SetChildObjectActive(ScrollAreaController.ChildObjectType.CHECK_BOX, false);
		areaController_.SetChildObjectActive(ScrollAreaController.ChildObjectType.SCROLL_BUTTON, true);
	}

	protected void AddButton(MenuCommonData.MenuType menuType, bool isIconActive)
	{
		GameObject buttonObject = areaController_.AddButton();
		HowToPlayCommonButton button = buttonObject.AddComponent<HowToPlayCommonButton>();
		button.SetButtonParam(menuType, isIconActive);
	}
}
