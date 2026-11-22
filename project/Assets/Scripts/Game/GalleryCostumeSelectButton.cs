using UnityEngine;

public class GalleryCostumeSelectButton : OpenDialogButtonBase
{
	private Define.CharaId charaId_;

	protected override void Start()
	{
		base.Start();
		SetDialogType(DialogCommonData.DialogType.DIALOG_MESSAGE);
	}

	public void SetServant(Define.CharaId charaId)
	{
		charaId_ = charaId;
	}

	protected override void AddScriptToDialog(ref GameObject dialogObject)
	{
		base.AddScriptToDialog(ref dialogObject);
		CostumeChangeDialog costumeChangeDialog = dialogObject.AddComponent<CostumeChangeDialog>();
		costumeChangeDialog.SetCharaId(charaId_);
	}
}
