using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SwipeCharacterMenuBase : MenuBase
{
	private SwipeCharacterController pageController_;

	private GenderSwitchObjectsController genderObjectsController_;

	protected override void LoadPrefab()
	{
		base.LoadPrefab();
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.CHARA_PAGE_CONTROLLER);
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.GENDER_OBJECTS_CONTROLLER);
	}

	protected override void Initialize()
	{
		base.Initialize();
		pageController_ = GetChildObject(MenuCommonData.ChildObjectType.CHARA_PAGE_CONTROLLER).GetComponent<SwipeCharacterController>();
		genderObjectsController_ = GetChildObject(MenuCommonData.ChildObjectType.GENDER_OBJECTS_CONTROLLER).GetComponent<GenderSwitchObjectsController>();
	}

	protected virtual void SetupCharacter(List<Define.CharaId> charaIdList)
	{
		pageController_.SetTotalCharacterNum(charaIdList.Count);
		for (int i = 0; i < charaIdList.Count; i++)
		{
			pageController_.AddCharacter(charaIdList[i], OnCharaClickFunction);
		}
	}

	protected virtual void OnCharaClickFunction(Define.CharaId charaId)
	{
		if (MenuCommonData.CanExecuteButtonOnClick(base.gameObject.scene.name))
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlaySE", SeController.SeType.SE_SELECT_BUTTON);
			StartCoroutine(ControlOpenDialog(charaId));
		}
	}

	private IEnumerator ControlOpenDialog(Define.CharaId charaId)
	{
		DialogCommonData.SetCanvasComponentActive(base.gameObject.scene.name, true);
		ResourceRequest prefabAsync = Resources.LoadAsync(DialogCommonData.GetDialogPrefabName(DialogCommonData.DialogType.DIALOG_MESSAGE));
		if (!prefabAsync.isDone)
		{
			yield return null;
		}
		GameObject dialogObject = DialogCommonData.InstantiateDialog(prefabAsync, base.gameObject.scene.name);
		dialogObject.SetActive(true);
		AddScriptToDialog(ref dialogObject, charaId);
		DialogCommonData.SetAnimeTriggerFlag(base.gameObject.scene.name, true);
	}

	protected abstract void AddScriptToDialog(ref GameObject dialogObject, Define.CharaId charaId);

	protected void SetGenderObjectActive(GenderSwitchObjectsController.SwitchObjectType objectType, bool flag)
	{
		genderObjectsController_.SetObjectActive(objectType, flag);
	}

	protected void SetCurrentGender()
	{
		genderObjectsController_.SetCurrentGender(GetCurrentGender());
	}

	protected void SetGenderSelectMessage(string text)
	{
		genderObjectsController_.SetGenderSelectMessage(text);
	}

	protected void SetCostumeGetMessage(string text)
	{
		genderObjectsController_.SetCostumeGetMessage(text);
	}

	protected void SetCostumeNum(int havingNum, int maxNum)
	{
		genderObjectsController_.SetHavingCostumeNum(havingNum);
		genderObjectsController_.SetMaxCostumeNum(maxNum);
	}

	protected int GetPageMaxCharacterNum()
	{
		return pageController_.GetPageMaxCharacterNum();
	}

	protected int Compare(Define.CharaId charaId0, Define.CharaId charaId1)
	{
		return GetSortIndex(charaId0) - GetSortIndex(charaId1);
	}

	protected int GetSortIndex(Define.CharaId charaId)
	{
		return (!Define.IsMasterId(charaId)) ? SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSortIndex(charaId) : (-1);
	}

	protected void SetFrame(Define.CharaId charaId)
	{
		pageController_.SetFrame(charaId);
	}

	protected void SetCharacterShowParam(bool isHiddenLockedChara, bool isShownSelectingFrame, bool isShownNewIcon, bool isSelectableLockedChara)
	{
		pageController_.SetHideLockedCharaFlag(isHiddenLockedChara);
		pageController_.SetShowSelectingFrameFlag(isShownSelectingFrame);
		pageController_.SetShowNewIconFlag(isShownNewIcon);
		pageController_.SetLockedCharaSelectableFlag(isSelectableLockedChara);
	}
}
