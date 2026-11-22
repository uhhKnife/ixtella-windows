using UnityEngine;

public class CareMealButton : OpenDialogButtonBase
{
	protected override void Start()
	{
		base.Start();
		SetDialogType(DialogCommonData.DialogType.DIALOG_BUTTON);
	}

	protected override void AddScriptToDialog(ref GameObject dialogObject)
	{
		base.AddScriptToDialog(ref dialogObject);
		dialogObject.AddComponent<MealSelectDialog>();
	}
}
