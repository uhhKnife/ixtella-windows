using UnityEngine;

public class MealSelectDialog : ButtonListDialogBase
{
	private enum ButtonType
	{
		BUTTON_MILK = 0,
		BUTTON_SWEET = 1,
		BUTTON_SPECIAL = 2,
		BUTTON_MAX = 3
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		base.Update();
		MonitorOutsideTouchEvent();
	}

	protected override void Awake()
	{
		base.Awake();
		InitializeTouchEventController();
	}

	private void OnDestroy()
	{
		CheckClearState();
	}

	protected override int GetTotalButtonNum()
	{
		return 3;
	}

	protected override void AddScriptToButton(ref GameObject buttonObject, int buttonDataIndex)
	{
		switch ((ButtonType)buttonDataIndex)
		{
		case ButtonType.BUTTON_MILK:
			buttonObject.AddComponent<MealMilkButton>();
			break;
		case ButtonType.BUTTON_SWEET:
			buttonObject.AddComponent<MealSweetButton>();
			break;
		case ButtonType.BUTTON_SPECIAL:
			buttonObject.AddComponent<MealSpecialButton>();
			break;
		}
	}

	protected override ButtonShowState GetButtonShowState(int buttonDataIndex)
	{
		switch ((ButtonType)buttonDataIndex)
		{
		case ButtonType.BUTTON_MILK:
		case ButtonType.BUTTON_SWEET:
			return ButtonShowState.STATE_NORMAL;
		case ButtonType.BUTTON_SPECIAL:
			return (!IsExecutableSpecialMeal()) ? ButtonShowState.STATE_HIDDEN : ButtonShowState.STATE_NORMAL;
		default:
			return ButtonShowState.STATE_NORMAL;
		}
	}

	private bool IsExecutableSpecialMeal()
	{
		return SingletonMonoBehaviour<GameController>.Instance.CanCook();
	}
}
