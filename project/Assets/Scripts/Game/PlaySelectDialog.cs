using UnityEngine;

public class PlaySelectDialog : ButtonListDialogBase
{
	private enum ButtonType
	{
		BUTTON_BALL = 0,
		BUTTON_BOOK = 1,
		BUTTON_SONG = 2,
		BUTTON_MAX = 3
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
		case ButtonType.BUTTON_BALL:
			buttonObject.AddComponent<PlayBallButton>();
			break;
		case ButtonType.BUTTON_BOOK:
			buttonObject.AddComponent<PlayBookButton>();
			break;
		case ButtonType.BUTTON_SONG:
			buttonObject.AddComponent<PlaySongButton>();
			break;
		}
	}
}
