using UnityEngine;

public class MasterSelectButton : OpenDialogButtonBase
{
	private PlayerStatusManager.Gender gender_;

	protected override void Start()
	{
		base.Start();
		SetDialogType(DialogCommonData.DialogType.DIALOG_MESSAGE);
	}

	public void SetMasterGender(PlayerStatusManager.Gender gender)
	{
		gender_ = gender;
	}

	protected override void AddScriptToDialog(ref GameObject dialogObject)
	{
		base.AddScriptToDialog(ref dialogObject);
		MasterSelectDialog masterSelectDialog = dialogObject.AddComponent<MasterSelectDialog>();
		masterSelectDialog.SetMasterGender(gender_);
	}
}
