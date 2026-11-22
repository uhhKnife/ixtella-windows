using UnityEngine;

public class GameResetButton : OpenDialogButtonBase
{
	private string BUTTON_TEXT_ID = "SYS_MENU_0-1-2_game_reset_button";

	protected override void Start()
	{
		base.Start();
		SetButtonMainText(BUTTON_TEXT_ID);
		SetDialogType(DialogCommonData.DialogType.DIALOG_MESSAGE);
	}

	protected override void AddScriptToDialog(ref GameObject dialogObject)
	{
		base.AddScriptToDialog(ref dialogObject);
		dialogObject.AddComponent<GameResetDialog>();
	}
}
