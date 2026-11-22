using System;
using System.Collections.Generic;
using UnityEngine;

public class CostumeChangeMenu : SwipeCharacterMenuBase
{
	protected override void Initialize()
	{
		base.Initialize();
		SetCharacterShowParam(true, true, false, false);
		SetGenderObjectActive(GenderSwitchObjectsController.SwitchObjectType.COSTUME_COUNT, true);
		SetGenderObjectActive(GenderSwitchObjectsController.SwitchObjectType.CURRENT_GENDER, true);
		SetGenderObjectActive(GenderSwitchObjectsController.SwitchObjectType.GENDER_SELECT_MESSAGE, false);
		SetGenderObjectActive(GenderSwitchObjectsController.SwitchObjectType.COSTUME_GET_MESSAGE, false);
		SetCurrentGender();
		List<Define.CharaId> charaIdList = new List<Define.CharaId>();
		GetCharaIdList(ref charaIdList);
		SetupCharacter(charaIdList);
		SetCostumeNum(GetUnlockedCostumeNum(ref charaIdList), charaIdList.Count);
	}

	protected override void AddScriptToDialog(ref GameObject dialogObject, Define.CharaId charaId)
	{
		CostumeChangeDialog costumeChangeDialog = dialogObject.AddComponent<CostumeChangeDialog>();
		costumeChangeDialog.SetCharaId(charaId);
	}

	protected override void OnButtonClick()
	{
		base.OnButtonClick();
		CloseMenu();
	}

	protected override void OnCharaClickFunction(Define.CharaId charaId)
	{
		if (MenuCommonData.CanExecuteButtonOnClick(base.gameObject.scene.name))
		{
			if (charaId == SingletonMonoBehaviour<PlayerStatusManager>.Instance.GetCurrentCharaId())
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlaySE", SeController.SeType.SE_WARNING);
			}
			else
			{
				base.OnCharaClickFunction(charaId);
			}
		}
	}

	private void GetCharaIdList(ref List<Define.CharaId> charaIdList)
	{
		List<Define.CharaId> maleCharaIdList = new List<Define.CharaId>();
		List<Define.CharaId> femaleCharaIdList = new List<Define.CharaId>();
		GetGenderCharaIdList(ref maleCharaIdList, ref femaleCharaIdList);
		Comparison<Define.CharaId> comparison = base.Compare;
		maleCharaIdList.Sort(comparison);
		femaleCharaIdList.Sort(comparison);
		int num = GetPageMaxCharacterNum() / 2;
		if (maleCharaIdList.Count == femaleCharaIdList.Count)
		{
			int num2 = ((num != 0) ? ((maleCharaIdList.Count - 1) / num + 1) : 0);
			for (int i = 0; i < num2; i++)
			{
				AddCharacterEachPage(ref charaIdList, ref maleCharaIdList, i, num);
				AddCharacterEachPage(ref charaIdList, ref femaleCharaIdList, i, num);
			}
		}
		else
		{
			Debug.LogError("[" + GetType().ToString() + "] Error: male chara num and female chara num is different");
		}
	}

	private void GetGenderCharaIdList(ref List<Define.CharaId> maleCharaIdList, ref List<Define.CharaId> femaleCharaIdList)
	{
		for (int i = 0; i < 12; i++)
		{
			Define.CharaId charaId = (Define.CharaId)i;
			if (Define.IsMaleCharacter(charaId))
			{
				maleCharaIdList.Add(charaId);
			}
			else
			{
				femaleCharaIdList.Add(charaId);
			}
		}
	}

	private void AddCharacterEachPage(ref List<Define.CharaId> destAllCharaIdList, ref List<Define.CharaId> genderCharaIdList, int pageIndex, int pageHalfCharaNum)
	{
		for (int i = 0; i < pageHalfCharaNum; i++)
		{
			int num = pageIndex * pageHalfCharaNum + i;
			if (num < genderCharaIdList.Count)
			{
				destAllCharaIdList.Add(genderCharaIdList[num]);
			}
		}
	}

	private int GetUnlockedCostumeNum(ref List<Define.CharaId> charaIdList)
	{
		int num = 0;
		for (int i = 0; i < charaIdList.Count; i++)
		{
			if (Define.IsMasterId(charaIdList[i]))
			{
				num++;
				continue;
			}
			GalleryData.CostumeUnlockData costumeUnlockData = SaveData.GetClass(GalleryData.GetUnlockSaveDataKey(charaIdList[i]), new GalleryData.CostumeUnlockData());
			if (costumeUnlockData.GetUnlockFlag() != GalleryData.UnlockFlag.FLAG_LOCKED)
			{
				num++;
			}
		}
		return num;
	}
}
