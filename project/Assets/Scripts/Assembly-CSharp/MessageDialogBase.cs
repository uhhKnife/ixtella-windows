using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class MessageDialogBase : DialogBase
{
	protected enum ButtonKind
	{
		BUTTON_LEFT = 0,
		BUTTON_RIGHT = 1
	}

	public enum ButtonSelectState
	{
		NONE = 0,
		LEFT_SELECTED = 1,
		RIGHT_SELECTED = 2
	}

	private readonly string YES_TEXT_ID = "SYS_dialog_button_yes";

	private readonly string NO_TEXT_ID = "SYS_dialog_button_no";

	private readonly string OK_TEXT_ID = "SYS_dialog_button_ok";

	private readonly string BUTTON_PARENT_OBJECT_NAME = "AreaVertical/AreaButton/AreaHorizontal";

	private readonly string BUTTON_OBJECT_NAME = "BtnCommon_";

	private readonly string CHARA_IMAGE_OBJECT_NAME = "AreaVertical/AreaChara";

	private Dictionary<ButtonKind, GameObject> buttonObjectList_;

	private GameObject charaImageObject_;

	private ButtonSelectState buttonSelectState_;

	protected override void Start()
	{
		base.Start();
		UpdateCurrentPageContents();
		SetCharaImageActive(IsCharaImageActive());
		buttonSelectState_ = ButtonSelectState.NONE;
	}

	protected abstract string GetBodyTextId();

	protected override void UpdateCurrentPageContents()
	{
		base.UpdateCurrentPageContents();
		SetBodyText(GetBodyTextId());
	}

	protected virtual int GetTotalPageNum()
	{
		return 1;
	}

	protected override void RegisterGameObjects()
	{
		base.RegisterGameObjects();
		GameObject gameObject = base.transform.Find(BUTTON_PARENT_OBJECT_NAME).gameObject;
		buttonObjectList_ = new Dictionary<ButtonKind, GameObject>();
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			buttonObjectList_.Add((ButtonKind)i, gameObject.transform.Find(BUTTON_OBJECT_NAME + (i + 1)).gameObject);
		}
		charaImageObject_ = GetChildObject(CHARA_IMAGE_OBJECT_NAME);
	}

	private void OnLeftButtonClickCommon()
	{
		if (buttonSelectState_ == ButtonSelectState.NONE && MenuCommonData.CanExecuteButtonOnClick(base.gameObject.scene.name))
		{
			OnLeftButtonClick();
		}
	}

	private void OnRightButtonClickCommon()
	{
		if (buttonSelectState_ == ButtonSelectState.NONE && MenuCommonData.CanExecuteButtonOnClick(base.gameObject.scene.name))
		{
			OnRightButtonClick();
		}
	}

	protected virtual void OnLeftButtonClick()
	{
		PlaySelectSE();
		CloseDialog();
		buttonSelectState_ = ButtonSelectState.LEFT_SELECTED;
	}

	protected virtual void OnRightButtonClick()
	{
		PlaySelectSE();
		CloseDialog();
		buttonSelectState_ = ButtonSelectState.RIGHT_SELECTED;
	}

	protected virtual void SetAllButtonsText()
	{
		SetButtonText(ButtonKind.BUTTON_LEFT, YES_TEXT_ID);
		SetButtonText(ButtonKind.BUTTON_RIGHT, NO_TEXT_ID);
	}

	protected string GetOkTextId()
	{
		return OK_TEXT_ID;
	}

	protected void SetButtonText(ButtonKind buttonPosition, string textId)
	{
		buttonObjectList_[buttonPosition].GetComponentInChildren<Text>().text = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(textId);
	}

	protected void SetOnClickFunctions()
	{
		for (int i = 0; i < buttonObjectList_.Count; i++)
		{
			ButtonKind onClickFunction = (ButtonKind)i;
			SetOnClickFunction(onClickFunction);
		}
	}

	protected void SetOnClickFunction(ButtonKind buttonKind)
	{
		buttonObjectList_[buttonKind].GetComponent<Button>().onClick.AddListener(GetOnClickFunction(buttonKind));
	}

	protected void SetButtonVisible(ButtonKind buttonPosition, bool flag)
	{
		buttonObjectList_[buttonPosition].SetActive(flag);
	}

	protected ButtonSelectState GetButtonSelectState()
	{
		return buttonSelectState_;
	}

	private UnityAction GetOnClickFunction(ButtonKind buttonPosition)
	{
		switch (buttonPosition)
		{
		case ButtonKind.BUTTON_LEFT:
			return OnLeftButtonClickCommon;
		case ButtonKind.BUTTON_RIGHT:
			return OnRightButtonClickCommon;
		default:
			return null;
		}
	}

	protected void SetCharaImageActive(bool flag)
	{
		charaImageObject_.SetActive(flag);
	}

	protected void SetDisplaytCharaId(Define.CharaId charaId)
	{
		Image componentInChildren = charaImageObject_.GetComponentInChildren<Image>();
		componentInChildren.sprite = PortSprites.Load(MenuCommonData.GetCharaSpriteFilename(charaId));
	}

	protected virtual bool IsCharaImageActive()
	{
		return false;
	}
}
