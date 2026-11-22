using UnityEngine;

public class GameQuitDialog : MessageDialogBase
{
	private readonly string BODY_TEXT_ID = "SYS_MENU_0-1_dialog_00";

	protected override void Start()
	{
		base.Start();
		SetAllButtonsText();
		SetOnClickFunctions();
	}

	protected override void OnLeftButtonClick()
	{
		base.OnLeftButtonClick();
		Application.Quit();
	}

	protected override string GetBodyTextId()
	{
		return BODY_TEXT_ID;
	}
}
