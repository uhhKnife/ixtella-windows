using UnityEngine;

public class CreditMenu : ScrollTextMenuBase
{
	private readonly string TEXT_FILENAME = "ixtella_credit";

	private readonly int fontSize = 40;

	private ScrollAreaController textController_;

	protected override void Initialize()
	{
		base.Initialize();
		textController_ = GetChildObject(MenuCommonData.ChildObjectType.SCROLL_TEXT_CONTROLLER).GetComponent<ScrollAreaController>();
		textController_.SetAlignment(TextAnchor.MiddleCenter);
		textController_.SetChildObjectActive(ScrollAreaController.ChildObjectType.SCROLL_TEXT, true);
		textController_.SetChildObjectActive(ScrollAreaController.ChildObjectType.CHECK_BOX, false);
		textController_.SetChildObjectActive(ScrollAreaController.ChildObjectType.SCROLL_BUTTON, false);
		textController_.SetScrollText(GetTextAsset().text);
		textController_.SetScrollTextFontSize(fontSize);
	}

	protected override void OnButtonClick()
	{
		base.OnButtonClick();
		GoToNextMenu(MenuCommonData.MenuType.MENU_OPTION_TOP);
	}

	protected override string GetTextFilename()
	{
		return TEXT_FILENAME;
	}
}
