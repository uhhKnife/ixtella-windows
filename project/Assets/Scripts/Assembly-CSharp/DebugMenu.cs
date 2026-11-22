using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
	private enum DebugMenuMode
	{
		MODE_RESUME_LOG = 0,
		MODE_MULTI = 1,
		MODE_PHASE_LOG = 2,
		MODE_NONE = 3
	}

	public GameObject actionPanel_;

	public bool isShow_;

	private DebugMenuMode mode_;

	public DateTimeObserber timeObserber_;

	public EventScheduler eventScheduler_;

	private bool showActionPanel_;

	private bool showCostumePanel_;

	public GUIStyle style = new GUIStyle();

	public GUIStyle emphasis = new GUIStyle();

	private Vector3 touchStart = Vector3.zero;

	private Vector3 touchEnd = Vector3.zero;

	[SerializeField]
	private List<string> resumeLog_ = new List<string>();

	private void Awake()
	{
		mode_ = DebugMenuMode.MODE_MULTI;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		style.normal.textColor = Color.black;
		emphasis.normal.textColor = Color.red;
		style.fontSize = 30;
		emphasis.fontSize = 30;
	}

	private void Start()
	{
		// May be null on the title screen; PortDebugPanel refreshes these on scene load.
		timeObserber_ = UnityEngine.Object.FindObjectOfType<DateTimeObserber>();
		GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
		eventScheduler_ = (gameController != null) ? gameController.GetComponent<EventScheduler>() : null;
	}

	private void Update()
	{
		// No swipe gestures: on PC they'd fire on every mouse drag.
		if (!PortDebug.Enabled)
		{
			return;
		}
		InputKey();
	}

	private void InputKey()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			isShow_ = !isShow_;
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			BeforeMode();
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			NextMode();
		}
		if (Input.GetKeyDown(KeyCode.F2))
		{
			showActionPanel_ = !showActionPanel_;
			if (actionPanel_ != null)
			{
				actionPanel_.SetActive(showActionPanel_);
			}
			PortDebugPanel.TogglePanel();
		}
		if (Input.GetKeyDown(KeyCode.F3))
		{
			showCostumePanel_ = !showCostumePanel_;
		}
		if (Input.GetKey(KeyCode.Escape) && Input.GetKeyDown(KeyCode.Delete))
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "FinalizeGame");
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (!axis.Equals(0f))
		{
			int fontSize = (int)((float)style.fontSize + axis * 10f);
			style.fontSize = fontSize;
			emphasis.fontSize = fontSize;
		}
	}

	private void InputTouch()
	{
		if (Input.GetMouseButtonDown(0))
		{
			touchStart = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
		}
		if (!Input.GetMouseButtonUp(0))
		{
			return;
		}
		touchEnd = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
		float num = touchEnd.x - touchStart.x;
		float num2 = touchEnd.y - touchStart.y;
		if (Mathf.Abs(num2) < Mathf.Abs(num))
		{
			if (50f < num)
			{
				actionPanel_.SetActive(true);
			}
			else if (-50f > num)
			{
				NextMode();
			}
		}
		else if (Mathf.Abs(num) < Mathf.Abs(num2))
		{
			if (50f < num2)
			{
				isShow_ = !isShow_;
			}
			else if (-50f > num2)
			{
				actionPanel_.SetActive(false);
			}
		}
	}

	public void NextMode()
	{
		mode_ = ((mode_ == DebugMenuMode.MODE_RESUME_LOG) ? DebugMenuMode.MODE_MULTI : ((mode_ == DebugMenuMode.MODE_MULTI) ? DebugMenuMode.MODE_PHASE_LOG : ((mode_ != DebugMenuMode.MODE_PHASE_LOG) ? mode_ : DebugMenuMode.MODE_RESUME_LOG)));
	}

	public void BeforeMode()
	{
		mode_ = ((mode_ == DebugMenuMode.MODE_RESUME_LOG) ? DebugMenuMode.MODE_PHASE_LOG : ((mode_ != DebugMenuMode.MODE_MULTI) ? ((mode_ == DebugMenuMode.MODE_PHASE_LOG) ? DebugMenuMode.MODE_MULTI : mode_) : DebugMenuMode.MODE_RESUME_LOG));
	}

	private void SetupLog()
	{
		resumeLog_ = SaveData.GetList("ResumeDebugLog", new List<string>());
	}

	private void AddResumeLog(string str)
	{
		if (resumeLog_.Count < 100)
		{
			string item = "[" + DateTime.Now.ToString() + "] : " + str;
			resumeLog_.Add(item);
			SaveData.SetList("ResumeDebugLog", resumeLog_);
			SaveData.Save();
		}
	}

	public void ClearResumeLog()
	{
		resumeLog_.Clear();
		SaveData.SetList("ResumeDebugLog", resumeLog_);
		SaveData.Save();
	}

	private void OnGUI()
	{
		// EventScheduler only exists on the main screen.
		if (isShow_ && PortDebug.Enabled && eventScheduler_ != null && timeObserber_ != null)
		{
			ShowFps();
			ShowKeyHelp();
			DrawLine();
			ShowTimeInfo();
			DrawLine();
			switch (mode_)
			{
			case DebugMenuMode.MODE_RESUME_LOG:
				ShowResumeLog();
				break;
			case DebugMenuMode.MODE_MULTI:
				ShowPlayerInfo();
				DrawLine();
				ShowDayTimeInfo();
				DrawLine();
				ShowEvent();
				DrawLine();
				break;
			case DebugMenuMode.MODE_PHASE_LOG:
				ShowPhaseLog();
				break;
			}
		}
	}

	private void ShowFps()
	{
		FpsObserber component = GetComponent<FpsObserber>();
		GUILayout.Label("FPS : " + component.FrameRate.ToString("0.00"), emphasis);
	}

	private void ShowTimeInfo()
	{
		if (!(timeObserber_ == null))
		{
			GUILayout.Label("Time Rate : " + timeObserber_.TimeRate.ToString("0.0") + " | " + "Elapse Time : " + timeObserber_.ElapseTime.ToString("0.00") + "(" + timeObserber_.GetHoursFromGameStart() + "h" + timeObserber_.GetMinutesFromGameStart() + "m" + timeObserber_.GetSecondsFromGameStart() + "s)", style);
			GUILayout.Label("Last Played Date : " + timeObserber_.LastPlayedTime, style);
			GUILayout.Label("Time Differents : " + timeObserber_.DiffLastTime, style);
		}
	}

	private void ShowKeyHelp()
	{
		GUILayout.Label("[F1] 表示ON/OFF | [F2] コマンドON/OFF | [←][→] : 表示モード切替", style);
		GUILayout.Label("[PageUp/Down] : TimeRate変動 | [Home] : TimeRate Max | [End] : TimeRate Min", style);
	}

	private void ShowPlayerInfo()
	{
		GUILayout.Label("【プレイヤー情報】", emphasis);
		PlayerStatusManager.PlayerStatus playerStatus = SaveData.GetClass("PlayerStatus", new PlayerStatusManager.PlayerStatus());
		if (playerStatus != null)
		{
			GUILayout.Label("[性別] : " + SingletonMonoBehaviour<GameController>.Instance.GetPlayerGender(), style);
			GUILayout.Label("[キャラID] : " + playerStatus.charaId_, style);
			GUILayout.Label("[体重] : " + playerStatus.weight_.ToString("0.00") + "kg", style);
			string text = string.Empty;
			for (int i = 0; i < 6; i++)
			{
				text += ((i >= playerStatus.satiety_) ? "○" : "●");
			}
			string text2 = string.Empty;
			for (int j = 0; j < 6; j++)
			{
				text2 += ((j >= playerStatus.feel_) ? "○" : "●");
			}
			GUILayout.Label("[お腹] : " + text + " | [機嫌] : " + text2, style);
			GUILayout.Label("[体力] : " + playerStatus.strength_.ToString("0.0") + " | [知力] : " + playerStatus.intelligence_.ToString("0.0") + " | [魔力] : " + playerStatus.magic_.ToString("0.0"), style);
			GUILayout.Label("[IsSleepy] : " + playerStatus.isSleepy_ + " | [IsSleeping] : " + playerStatus.isSleeping_, style);
		}
	}

	private void ShowDayTimeInfo()
	{
		GUILayout.Label("【DAY情報】", emphasis);
		GUILayout.Label("Current Day : " + DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00"), style);
		GUILayout.Label("DAYTIME STATE : " + eventScheduler_.GetCurrentDayStateString(), style);
	}

	private void ShowEvent()
	{
		GUILayout.Label("【イベントスケジュール】", emphasis);
		PlayerStatusManager.PlayerStatus playerStatus = SaveData.GetClass("PlayerStatus", new PlayerStatusManager.PlayerStatus());
		GUILayout.Label("GAME STATE : " + SingletonMonoBehaviour<GameController>.Instance.GetGameState(), style);
		GUILayout.Label("#####################################", style);
		GUILayout.Label("Next satiety decrease is : " + eventScheduler_.GetNextSatietyEventTIme() + " [sec] later.", style);
		GUILayout.Label("Next feel decrease is : " + eventScheduler_.GetNextFeelEventTime() + " [sec] later.", style);
		GUILayout.Label("Raise sick is : " + eventScheduler_.GetNextSickedTime() + " [sec] later.", style);
		GUILayout.Label("Failure sing is : " + eventScheduler_.GetNextUnSingTime() + " [sec] later.", style);
		GUILayout.Label("#####################################", style);
		GUILayout.Label("Phaze is UnInitialized : " + eventScheduler_.GetCurrentPhaseNew(), style);
		GUILayout.Label("Current Phase ID : [" + eventScheduler_.GetCurrentPhaseIndex() + "]", style);
		GUILayout.Label("Current PHASE STATE : " + eventScheduler_.GetCurrentPhaseState(), style);
		GUILayout.Label("Sleep in Night: [" + eventScheduler_.IsNightSleeping() + "]", style);
		GUILayout.Label("ACTIVE TIME  : " + eventScheduler_.GetPhaseActiveTime() + "[sec]", style);
		GUILayout.Label("NAPABLE TIME : " + eventScheduler_.GetPhaseNapableTime() + "[sec]", style);
		GUILayout.Label("NAP TIME     : " + eventScheduler_.GetPhaseNapTime() + "[sec]", style);
		GUILayout.Label("Next <COOK> judge is : " + eventScheduler_.GetCookTime() + " [sec] later. FLAG=[" + SingletonMonoBehaviour<GameController>.Instance.CanCook() + "]", style);
		GUILayout.Label("Next <SING> judge is : " + eventScheduler_.GetSingTime() + " [sec] later. FLAG=[" + SingletonMonoBehaviour<GameController>.Instance.CanSing() + "]", style);
		GUILayout.Label("Next <SICK> judge is : " + eventScheduler_.GetSickTime() + " [sec] later. FLAG=[" + playerStatus.isSick_ + "]", style);
		GUILayout.Label("Next <DIAPER> judge is : " + eventScheduler_.GetDiaperTime() + " [sec] later. FLAG=[" + playerStatus.isDiaper_ + "]", style);
		GUILayout.Label("#####################################", style);
		GUILayout.Label("Satiety Limit : " + eventScheduler_.GetSatietyLimit() + "[sec]", style);
		GUILayout.Label("Feel Limit : " + eventScheduler_.GetFeelLimit() + "[sec]", style);
		GUILayout.Label("Sick Limit : " + eventScheduler_.GetSickLimit() + "[sec]", style);
	}

	private void ShowMenuInfo()
	{
		GUILayout.Label("【メニュー情報】", emphasis);
	}

	private void DrawLine()
	{
		GUILayout.Label("---------------------------------------------------------------------", style);
	}

	private void ShowResumeLog()
	{
		GUILayout.Label("【RESUME LOG】", emphasis);
		foreach (string item in resumeLog_)
		{
			GUILayout.Label(item, style);
		}
	}

	private void ShowPhaseLog()
	{
		for (int i = 0; i < eventScheduler_.GetCurrentPhaseIndex(); i++)
		{
			GUILayout.Label("【フェーズ" + i + " 終了時のパラメーター】", emphasis);
			string text = "Phase" + i;
			GUILayout.Label("体重：" + SaveData.GetFloat(text + "Weight"), style);
			GUILayout.Label("経過時間：" + SaveData.GetInt(text + "Hour") + ":" + SaveData.GetInt(text + "Minute") + ":" + SaveData.GetInt(text + "Second"), style);
			GUILayout.Label("体力：" + SaveData.GetFloat(text + "Strength"), style);
			GUILayout.Label("知力：" + SaveData.GetFloat(text + "Intelligence"), style);
			GUILayout.Label("魔力：" + SaveData.GetFloat(text + "Magic"), style);
			DrawLine();
		}
	}
}
