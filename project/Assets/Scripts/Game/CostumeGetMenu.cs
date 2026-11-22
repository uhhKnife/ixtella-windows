using System;
using System.Collections.Generic;
using UnityEngine;

public class CostumeGetMenu : SwipeCharacterMenuBase
{
	protected override void Initialize()
	{
		base.Initialize();
		SetCharacterShowParam(false, false, true, true);
		SetButtonVisible(false);
		SetGenderObjectActive(GenderSwitchObjectsController.SwitchObjectType.COSTUME_COUNT, false);
		SetGenderObjectActive(GenderSwitchObjectsController.SwitchObjectType.CURRENT_GENDER, false);
		SetGenderObjectActive(GenderSwitchObjectsController.SwitchObjectType.GENDER_SELECT_MESSAGE, false);
		SetGenderObjectActive(GenderSwitchObjectsController.SwitchObjectType.COSTUME_GET_MESSAGE, true);
		SetCostumeGetMessage(SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(GetMainTextId()));
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "SetButtonInteractive", 7, false);
	}

	protected override void AddScriptToDialog(ref GameObject dialogObject, Define.CharaId charaId)
	{
		CostumeGetDialog costumeGetDialog = dialogObject.AddComponent<CostumeGetDialog>();
		costumeGetDialog.SetCharaId(charaId);
	}

	protected override void SetupCharacter(List<Define.CharaId> charaIdList)
	{
		Comparison<Define.CharaId> comparison = base.Compare;
		charaIdList.Sort(comparison);
		base.SetupCharacter(charaIdList);
	}
}
