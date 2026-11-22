using System.Collections.Generic;
using UnityEngine;

public abstract class ButtonDialogBase : DialogBase
{
	public enum ButtonShowState
	{
		STATE_NORMAL = 0,
		STATE_HIDDEN = 1,
		STATE_UNSELECTABLE = 2
	}

	private class ButtonData
	{
		private int showTotalIndex_;

		private ButtonShowState state_;

		public ButtonData(int showTotalIndex, ButtonShowState state)
		{
			showTotalIndex_ = showTotalIndex;
			state_ = state;
		}

		public int GetShowTotalIndex()
		{
			return showTotalIndex_;
		}

		public ButtonShowState GetButtonState()
		{
			return state_;
		}
	}

	private readonly string BUTTON_OBJECT_NAME_COMMON = "AreaVertical/BtnCommon_";

	private GameObject[] buttonObjectList_;

	private List<ButtonData> buttonDataList_;

	private int displayButtonNum_;

	protected override void Start()
	{
		base.Start();
		SetAllButtonData();
		UpdateCurrentPageContents();
	}

	protected override void RegisterGameObjects()
	{
		base.RegisterGameObjects();
		buttonObjectList_ = new GameObject[GetPageMaxButtonNum()];
		for (int i = 0; i < buttonObjectList_.Length; i++)
		{
			string objectName = BUTTON_OBJECT_NAME_COMMON + (i + 1);
			buttonObjectList_[i] = GetChildObject(objectName);
		}
	}

	protected override void UpdateCurrentPageContents()
	{
		base.UpdateCurrentPageContents();
		for (int i = 0; i < GetPageMaxButtonNum(); i++)
		{
			int buttonDataIndex = GetButtonDataIndex(i);
			if (0 <= buttonDataIndex && buttonDataIndex < buttonDataList_.Count && buttonDataList_[buttonDataIndex].GetButtonState() != ButtonShowState.STATE_HIDDEN)
			{
				SetButtonVisible(i, true);
				AddScriptToButton(i, buttonDataIndex);
			}
			else
			{
				SetButtonVisible(i, false);
			}
		}
	}

	protected void SetButtonVisible(int buttonObjectIndex, bool flag)
	{
		if (IsValidButtonObjectIndex(buttonObjectIndex))
		{
			buttonObjectList_[buttonObjectIndex].SetActive(flag);
		}
	}

	protected void SetAllButtonData()
	{
		buttonDataList_ = new List<ButtonData>();
		int totalButtonNum = GetTotalButtonNum();
		int num = 0;
		for (int i = 0; i < totalButtonNum; i++)
		{
			ButtonShowState buttonShowState = GetButtonShowState(i);
			switch (buttonShowState)
			{
			case ButtonShowState.STATE_NORMAL:
			case ButtonShowState.STATE_UNSELECTABLE:
				buttonDataList_.Add(new ButtonData(num, buttonShowState));
				num++;
				break;
			case ButtonShowState.STATE_HIDDEN:
				buttonDataList_.Add(new ButtonData(-1, buttonShowState));
				break;
			}
		}
		CalculateShowButtonNum();
	}

	protected virtual ButtonShowState GetButtonShowState(int buttonDataIndex)
	{
		return ButtonShowState.STATE_NORMAL;
	}

	private void AddScriptToButton(int buttonObjectIndex, int buttonDataIndex)
	{
		AddScriptToButton(ref buttonObjectList_[buttonObjectIndex], buttonDataIndex);
	}

	protected abstract int GetPageMaxButtonNum();

	protected abstract int GetTotalButtonNum();

	protected abstract void AddScriptToButton(ref GameObject buttonObject, int buttonDataIndex);

	protected virtual Sprite GetButtonSprite(int buttonDataIndex)
	{
		return null;
	}

	private bool IsValidButtonObjectIndex(int buttonIndex)
	{
		if (0 <= buttonIndex && buttonIndex < buttonObjectList_.Length)
		{
			return true;
		}
		Debug.LogError("buttonIndex " + buttonIndex + " is invalid");
		return false;
	}

	private void CalculateShowButtonNum()
	{
		displayButtonNum_ = 0;
		for (int i = 0; i < buttonDataList_.Count; i++)
		{
			if (buttonDataList_[i].GetButtonState() != ButtonShowState.STATE_HIDDEN)
			{
				displayButtonNum_++;
			}
		}
	}

	private int GetButtonDataIndex(int showTotalIndex)
	{
		for (int i = 0; i < buttonDataList_.Count; i++)
		{
			if (buttonDataList_[i].GetShowTotalIndex() == showTotalIndex)
			{
				return i;
			}
		}
		return -1;
	}
}
