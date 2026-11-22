using UnityEngine;

public class CarePlayButton : OpenDialogButtonBase
{
	protected override void Start()
	{
		base.Start();
		SetDialogType(DialogCommonData.DialogType.DIALOG_BUTTON);
	}

	protected override void AddScriptToDialog(ref GameObject dialogObject)
	{
		base.AddScriptToDialog(ref dialogObject);
		dialogObject.AddComponent<PlaySelectDialog>();
	}

	protected override bool ShouldOpenDialog()
	{
		return true;
	}
}
