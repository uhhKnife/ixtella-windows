using System.Collections.Generic;
using UnityEngine;

public class MasterSelectMenu : SwipeCharacterMenuBase
{
	protected override void UpdateMenu()
	{
	}

	protected override void Initialize()
	{
		base.Initialize();
		SetCharacterShowParam(false, false, false, true);
		List<Define.CharaId> list = new List<Define.CharaId>();
		list.Add(Define.CharaId.CHID_MASTER_M);
		list.Add(Define.CharaId.CHID_MASTER_F);
		List<Define.CharaId> charaIdList = list;
		SetupCharacter(charaIdList);
		SetGenderObjectActive(GenderSwitchObjectsController.SwitchObjectType.COSTUME_COUNT, false);
		SetGenderObjectActive(GenderSwitchObjectsController.SwitchObjectType.CURRENT_GENDER, false);
		SetGenderObjectActive(GenderSwitchObjectsController.SwitchObjectType.GENDER_SELECT_MESSAGE, true);
		SetGenderObjectActive(GenderSwitchObjectsController.SwitchObjectType.COSTUME_GET_MESSAGE, false);
		SetGenderSelectMessage(SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(GetMainTextId()));
	}

	protected override void AddScriptToDialog(ref GameObject dialogObject, Define.CharaId charaId)
	{
		MasterSelectDialog masterSelectDialog = dialogObject.AddComponent<MasterSelectDialog>();
		PlayerStatusManager.Gender masterGender = ((charaId != Define.CharaId.CHID_MASTER_M) ? PlayerStatusManager.Gender.GENDER_FEMALE : PlayerStatusManager.Gender.GENDER_MALE);
		masterSelectDialog.SetMasterGender(masterGender);
	}

	protected override void OnButtonClick()
	{
		base.OnButtonClick();
		CloseMenu();
	}
}
