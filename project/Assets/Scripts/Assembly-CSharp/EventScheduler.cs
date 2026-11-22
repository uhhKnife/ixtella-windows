using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventScheduler : MonoBehaviour
{
	[Serializable]
	public class PhaseEventInfo
	{
		public enum PhaseState : uint
		{
			PHASE_READY = 0u,
			PHASE_ACTIVE_TIME = 1u,
			PHASE_NAPABLE_TIME = 2u,
			PHASE_NAP_TIME = 3u
		}

		[Serializable]
		public class Matter
		{
			public float time_;

			public int probability_;

			public Matter()
			{
				time_ = 0f;
				probability_ = 100;
			}

			public Matter(float t, int p)
			{
				time_ = t;
				probability_ = p;
			}
		}

		public bool isNew_;

		public int id_;

		public PhaseState state_;

		public float activeTime_;

		public float napableTime_;

		public float napTime_;

		public bool isNightSleep_;

		public List<Matter> cookParams_;

		public List<Matter> singParams_;

		public List<Matter> sickParams_;

		public List<Matter> diaperParams_;

		public PhaseEventInfo()
		{
			Initialize();
		}

		public void Initialize()
		{
			isNew_ = true;
			id_ = 0;
			state_ = PhaseState.PHASE_READY;
			activeTime_ = 0f;
			napableTime_ = 0f;
			napTime_ = 0f;
			isNightSleep_ = false;
			cookParams_ = new List<Matter>();
			singParams_ = new List<Matter>();
			sickParams_ = new List<Matter>();
			diaperParams_ = new List<Matter>();
		}

		public bool IsActiveTime()
		{
			return state_ == PhaseState.PHASE_ACTIVE_TIME;
		}

		public bool IsNapableTime()
		{
			return state_ == PhaseState.PHASE_NAPABLE_TIME;
		}

		public bool IsSleepingTime()
		{
			return state_ == PhaseState.PHASE_NAP_TIME;
		}

		public void SetNightSleep(bool b)
		{
			isNightSleep_ = b;
		}

		public bool IsNightSleeping()
		{
			return isNightSleep_;
		}

		private bool IsBonusPhase()
		{
			return id_ == SingletonMonoBehaviour<GrowthDatabase>.Instance.GetTotalPhaseNum() - 1;
		}

		public void ForwardPhase()
		{
			id_++;
			if (id_ < SingletonMonoBehaviour<GrowthDatabase>.Instance.GetTotalPhaseNum())
			{
				if (IsBonusPhase())
				{
					if (SingletonMonoBehaviour<GameController>.Instance.CheckGameFlag(GameController.GameFlag.FLAG_GET_COSTUME))
					{
						state_ = PhaseState.PHASE_ACTIVE_TIME;
					}
					else
					{
						MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetGameState", GameController.GameState.GAME_STATE_CLEAR);
					}
				}
				else
				{
					state_ = PhaseState.PHASE_ACTIVE_TIME;
				}
			}
			else
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetGameState", GameController.GameState.GAME_STATE_CLEAR);
			}
		}

		public void OnSleep()
		{
			state_ = PhaseState.PHASE_NAP_TIME;
		}

		public void UpdatePhase()
		{
			if (state_ != PhaseState.PHASE_READY)
			{
				UpdatePhaseTime();
				UpdateCookingTime();
				UpdateSingTime();
				UpdateSickTime();
				UpdateDiaperTime();
			}
		}

		public void UpdatePhaseTime()
		{
			switch (state_)
			{
			case PhaseState.PHASE_ACTIVE_TIME:
				activeTime_ -= Time.deltaTime;
				if (activeTime_ <= 0f)
				{
					state_ = PhaseState.PHASE_NAPABLE_TIME;
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SLEEPY, true);
				}
				break;
			case PhaseState.PHASE_NAPABLE_TIME:
				napableTime_ -= Time.deltaTime;
				if (!(napableTime_ <= 0f))
				{
					break;
				}
				state_ = PhaseState.PHASE_READY;
				if (id_ == 0)
				{
					SingletonMonoBehaviour<GameController>.Instance.GetComponent<EventScheduler>().CheckDaysTime();
					if (SingletonMonoBehaviour<GameController>.Instance.GetComponent<EventScheduler>().IsNight())
					{
						break;
					}
				}
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetNextPhase");
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SLEEPING, false);
				break;
			case PhaseState.PHASE_NAP_TIME:
				napTime_ -= Time.deltaTime;
				if (napTime_ <= 0f)
				{
					state_ = PhaseState.PHASE_READY;
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "CheckEvolution");
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SLEEPING, false);
				}
				break;
			}
		}

		private void UpdateCookingTime()
		{
			if (cookParams_.Count == 0)
			{
				return;
			}
			cookParams_[0].time_ -= Time.deltaTime;
			if (cookParams_[0].time_ <= 0f)
			{
				if (Utility.SuccessRandom(cookParams_[0].probability_))
				{
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetCookable", true);
					Debug.Log("special cook command is success.");
				}
				else
				{
					Debug.Log("special cook command is failed.");
				}
				cookParams_.RemoveAt(0);
			}
		}

		private void UpdateSingTime()
		{
			if (singParams_.Count == 0)
			{
				return;
			}
			singParams_[0].time_ -= Time.deltaTime;
			if (singParams_[0].time_ <= 0f)
			{
				if (Utility.SuccessRandom(singParams_[0].probability_))
				{
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetSingable", true);
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "ResetSingTimer");
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "CheckStatus");
					Debug.Log("sing command is success.");
				}
				else
				{
					Debug.Log("sing command is failed.");
				}
				singParams_.RemoveAt(0);
			}
		}

		private void UpdateSickTime()
		{
			if (sickParams_.Count == 0)
			{
				return;
			}
			sickParams_[0].time_ -= Time.deltaTime;
			if (sickParams_[0].time_ <= 0f)
			{
				if (Utility.SuccessRandom(sickParams_[0].probability_))
				{
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SICK, true);
					Debug.Log("player is sicked.");
				}
				else
				{
					Debug.Log("sick event is failed.");
				}
				sickParams_.RemoveAt(0);
			}
		}

		private void UpdateDiaperTime()
		{
			if (diaperParams_.Count == 0)
			{
				return;
			}
			diaperParams_[0].time_ -= Time.deltaTime;
			if (diaperParams_[0].time_ <= 0f)
			{
				if (Utility.SuccessRandom(diaperParams_[0].probability_))
				{
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_DIAPER, true);
					Debug.Log("diaper event is success.");
				}
				else
				{
					Debug.Log("diaper event is failed.");
				}
				diaperParams_.RemoveAt(0);
			}
		}
	}

	private struct RuleTime
	{
		public int hour_;

		public int minute_;

		public int second_;

		public int totalSec_;

		public int nextHour_;

		public int nextMinute_;

		public int nextSecond_;

		public int totalNextSec_;

		public void Set(int h, int m, int s)
		{
			hour_ = h;
			minute_ = m;
			second_ = s;
			totalSec_ = Utility.ConvertHMStoSec(h, m, s);
		}

		public void SetNext(int h, int m, int s)
		{
			nextHour_ = h;
			nextMinute_ = m;
			nextSecond_ = s;
			totalNextSec_ = Utility.ConvertHMStoSec(h, m, s);
		}

		public bool InnerTime(int h, int m, int s)
		{
			int num = Utility.ConvertHMStoSec(h, m, s);
			if (totalNextSec_ < totalSec_)
			{
				if (num >= totalNextSec_ && num < totalSec_)
				{
					return false;
				}
				return true;
			}
			if (num >= totalSec_ && num < totalNextSec_)
			{
				return true;
			}
			return false;
		}
	}

	public enum DaysState
	{
		STATE_DAYTIME = 0,
		STATE_SLEEPABLE = 1,
		STATE_NIGHT = 2
	}

	private struct GameOverLimit
	{
		public bool flag_;

		public float timer_;

		public GameOverLimit(bool b, float time)
		{
			flag_ = b;
			timer_ = time;
		}
	}

	private readonly string NOTIFICATION_TITLE = "SYS_game_title";

	private readonly string NOTIFICATION_MESSAGE = "SYS_MENU_10_notification_message_00";

	private float satietyTimer_;

	private float feelTimer_;

	private float sickTimer_;

	private float singTimer_;

	private GameOverLimit satietyLimit_;

	private GameOverLimit feelLimit_;

	private GameOverLimit sickLimit_;

	private DaysState daysState_;

	private RuleTime SLEEPABLE_TIME = default(RuleTime);

	private RuleTime FORCE_SLEEP_TIME = default(RuleTime);

	private RuleTime WAKEUP_TIME = default(RuleTime);

	private PhaseEventInfo currentPhase_ = new PhaseEventInfo();

	private DateTimeObserber dateTimeObserber_;

	private bool isRuned;

	private bool isSetup;

	private bool isResume;

	private bool endSetupPhase_;

	private string notificationTitle_;

	private string notificationMessage_;

	private bool isDebugOverWriteDayState_;

	private void Awake()
	{
	}

	private void Start()
	{
		if (PortDebug.Enabled)
		{
			UnityEngine.Object.FindObjectOfType<DebugMenu>().SendMessage("SetupLog");
			UnityEngine.Object.FindObjectOfType<DebugMenu>().SendMessage("AddResumeLog", "App Booted.");
		}
		CancellNotification();
		dateTimeObserber_ = GameObject.FindWithTag(Define.GetTagName(Define.TagId.TAG_DATE_TIME_OBSERBER)).GetComponent<DateTimeObserber>();
		Initialize();
		if (SingletonMonoBehaviour<GameController>.Instance.IsGameOver())
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "GameOvered");
		}
	}

	private void Update()
	{
		if (!currentPhase_.isNew_)
		{
			if (!IsPhazeZero())
			{
				CheckDaysTime();
			}
			UpdateSatiety();
			UpdateFeel();
			UpdateSick();
			UpdateSing();
			UpdatePhase();
			UpdateGameOver();
			if (PortDebug.Enabled)
			{
				DebugInput();
			}
		}
	}

	private void SetupEventData()
	{
		if (isSetup)
		{
			return;
		}
		isRuned = true;
		isSetup = true;
		isResume = false;
		if (SingletonMonoBehaviour<GrowthDatabase>.Instance != null)
		{
			currentPhase_ = SaveData.GetClass("CurrentPhase", new PhaseEventInfo());
			endSetupPhase_ = true;
			InitializeDayTimeRule();
			if (!IsPhazeZero())
			{
				InitDayState();
			}
			satietyTimer_ = SaveData.GetFloat("SatietyTimer", SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSatietyDefuseInterval());
			feelTimer_ = SaveData.GetFloat("FeelTimer", SingletonMonoBehaviour<GrowthDatabase>.Instance.GetFeelDefuseInterval());
			sickTimer_ = SaveData.GetFloat("SickTimer", SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSickedInterval());
			singTimer_ = SaveData.GetFloat("SingTimer", SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSingInterval());
			satietyLimit_.flag_ = false;
			feelLimit_.flag_ = false;
			sickLimit_.flag_ = false;
			satietyLimit_.timer_ = SaveData.GetFloat("SatietyLimit", SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSatietyGameOverLimit());
			feelLimit_.timer_ = SaveData.GetFloat("FeelLimit", SingletonMonoBehaviour<GrowthDatabase>.Instance.GetFeelGameOverLimit());
			sickLimit_.timer_ = SaveData.GetFloat("SickLimit", SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSickGameOverLimit());
			if (!currentPhase_.isNew_)
			{
				CalcCostFromLastPlay();
				CalcPhaseFromLastPlay();
			}
			if (!IsPhazeZero())
			{
				SetCurrentDaysState();
			}
		}
		else
		{
			Debug.LogError("GrowthDatabase has not created yet.");
		}
	}

	private void Initialize()
	{
		SetupEventData();
		notificationTitle_ = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(NOTIFICATION_TITLE);
		notificationMessage_ = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(NOTIFICATION_MESSAGE);
	}

	private void InitializeDayTimeRule()
	{
		GrowthDatabase.NightTimeInfo nightTimeInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindNightInfo("SLEEPABLE_TIME");
		WAKEUP_TIME.SetNext(nightTimeInfo.GetHour(), nightTimeInfo.GetMinute(), nightTimeInfo.GetSecond());
		SLEEPABLE_TIME.Set(nightTimeInfo.GetHour(), nightTimeInfo.GetMinute(), nightTimeInfo.GetSecond());
		nightTimeInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindNightInfo("FORCE_SLEEP_TIME");
		SLEEPABLE_TIME.SetNext(nightTimeInfo.GetHour(), nightTimeInfo.GetMinute(), nightTimeInfo.GetSecond());
		FORCE_SLEEP_TIME.Set(nightTimeInfo.GetHour(), nightTimeInfo.GetMinute(), nightTimeInfo.GetSecond());
		nightTimeInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindNightInfo("WAKEUP_TIME");
		FORCE_SLEEP_TIME.SetNext(nightTimeInfo.GetHour(), nightTimeInfo.GetMinute(), nightTimeInfo.GetSecond());
		WAKEUP_TIME.Set(nightTimeInfo.GetHour(), nightTimeInfo.GetMinute(), nightTimeInfo.GetSecond());
	}

	private void OnApplicationPause(bool isPause)
	{
		if (isPause)
		{
			if (!isResume)
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "WriteSaveData");
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_DATE_TIME_OBSERBER), "WriteSaveData");
				PushNotification();
				isSetup = false;
				isResume = true;
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_BGM_CONTROLLER), "StopBgm");
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "StopSE");
				if (PortDebug.Enabled)
				{
					UnityEngine.Object.FindObjectOfType<DebugMenu>().SendMessage("AddResumeLog", "Pause On.");
				}
			}
		}
		else if (isRuned && !isSetup)
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_DATE_TIME_OBSERBER), "GetDiffTimeFromLastPlay");
			CancellNotification();
			SetupEventData();
			isSetup = true;
			isResume = false;
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_BGM_CONTROLLER), "StartFadeInBgm");
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "ResetReactionCount");
			if (PortDebug.Enabled)
			{
				UnityEngine.Object.FindObjectOfType<DebugMenu>().SendMessage("AddResumeLog", "Pause Off.");
			}
		}
	}

	private void OnApplicationQuit()
	{
		if (!isResume)
		{
			if (endSetupPhase_)
			{
				WriteSaveData();
				PushNotification();
			}
			isRuned = false;
			isResume = false;
			isSetup = false;
			if (PortDebug.Enabled)
			{
				UnityEngine.Object.FindObjectOfType<DebugMenu>().SendMessage("AddResumeLog", "App Quited.");
			}
		}
	}

	private void PushNotification()
	{
		if (!(SingletonMonoBehaviour<GameController>.Instance != null) || SingletonMonoBehaviour<GameController>.Instance.IsGameClear() || SingletonMonoBehaviour<GameController>.Instance.IsGameOver() || currentPhase_.isNew_)
		{
			return;
		}
		if (PortDebug.Enabled)
		{
			UnityEngine.Object.FindObjectOfType<DebugMenu>().SendMessage("AddResumeLog", "Try Push Notification.");
		}
		int num = FindNearCallingEvent();
		if (PortDebug.Enabled)
		{
			UnityEngine.Object.FindObjectOfType<DebugMenu>().SendMessage("AddResumeLog", "Found Delay Time ___ : " + num);
		}
		if (num > 0)
		{
			if (PortDebug.Enabled)
			{
				UnityEngine.Object.FindObjectOfType<DebugMenu>().SendMessage("AddResumeLog", "Register Notification.");
			}
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_LOCAL_PUSH), "RegisterLocalNotification", notificationTitle_, notificationMessage_, num, -1);
		}
	}

	private void CancellNotification()
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_LOCAL_PUSH), "CancelLocalNotification");
	}

	private int FindNearCallingEvent()
	{
		float num = 0f;
		if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsSleeping())
		{
			TimeSpan timeSpan = new TimeSpan(WAKEUP_TIME.hour_, WAKEUP_TIME.minute_, WAKEUP_TIME.second_);
			TimeSpan timeSpan2 = new TimeSpan(24, 0, 0);
			TimeSpan timeSpan3 = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
			float num2 = (float)timeSpan.TotalSeconds;
			float num3 = (float)timeSpan2.TotalSeconds;
			float num4 = (float)timeSpan3.TotalSeconds;
			if (IsNightSleeping())
			{
				num = ((!(num4 < num2)) ? (num3 - num4 + num2) : (num2 - num4));
			}
			else
			{
				float napTime_ = currentPhase_.napTime_;
				TimeSpan timeSpan4 = timeSpan3.Add(new TimeSpan(0, 0, (int)napTime_));
				TimeSpan timeSpan5 = new TimeSpan(SLEEPABLE_TIME.hour_, SLEEPABLE_TIME.minute_, SLEEPABLE_TIME.second_);
				num = ((!(timeSpan4.TotalSeconds >= timeSpan5.TotalSeconds)) ? napTime_ : (num3 - num4 + num2));
			}
			List<float> list = new List<float>();
			float num5 = FindNextSatietyEmptyTime() + num;
			if (num5 > 0f)
			{
				list.Add(num5);
			}
			float num6 = FindNextFeelEmptyTime() + num;
			if (num6 > 0f)
			{
				list.Add(num6);
			}
			if (list.Count > 0)
			{
				return (int)list.Min() + 10;
			}
			return 0;
		}
		TimeSpan timeSpan6 = new TimeSpan(FORCE_SLEEP_TIME.hour_, FORCE_SLEEP_TIME.minute_, FORCE_SLEEP_TIME.second_);
		TimeSpan timeSpan7 = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
		float num7 = (float)(timeSpan6 - timeSpan7).TotalSeconds;
		List<float> list2 = new List<float>();
		float num8 = FindNextSatietyEmptyTime();
		if (num8 > 0f && num8 < num7)
		{
			list2.Add(num8);
		}
		float num9 = FindNextFeelEmptyTime();
		if (num9 > 0f && num9 < num7)
		{
			list2.Add(num9);
		}
		float num10 = FindNextSingTime();
		if (num10 > 0f && num10 < num7)
		{
			list2.Add(num10);
		}
		float num11 = FindNextDiaperTime();
		if (num11 > 0f && num11 < num7)
		{
			list2.Add(num11);
		}
		float num12 = FindNextNapTime();
		if (num12 > 0f && num12 < num7)
		{
			list2.Add(num12);
		}
		float num13 = FindNextSleepTime();
		if (num13 > 0f && num13 < num7)
		{
			list2.Add(num13);
		}
		if (list2.Count > 0)
		{
			return (int)list2.Min() + 10;
		}
		return 0;
	}

	private float FindNextSatietyEmptyTime()
	{
		int num = Mathf.Max(SingletonMonoBehaviour<PlayerStatusManager>.Instance.GetSatiety() - 1, 0);
		return satietyTimer_ + (float)num * SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSatietyDefuseInterval();
	}

	private float FindNextFeelEmptyTime()
	{
		int num = Mathf.Max(SingletonMonoBehaviour<PlayerStatusManager>.Instance.GetFeel() - 1, 0);
		return feelTimer_ + (float)num * SingletonMonoBehaviour<GrowthDatabase>.Instance.GetFeelDefuseInterval();
	}

	private float FindNextSingTime()
	{
		if (currentPhase_.singParams_.Count == 0)
		{
			return float.MaxValue;
		}
		return currentPhase_.singParams_[0].time_;
	}

	private float FindNextDiaperTime()
	{
		if (currentPhase_.diaperParams_.Count == 0)
		{
			return float.MaxValue;
		}
		return currentPhase_.diaperParams_[0].time_;
	}

	private float FindNextNapTime()
	{
		if (currentPhase_.IsActiveTime())
		{
			return currentPhase_.activeTime_;
		}
		return float.MaxValue;
	}

	private float FindNextSleepTime()
	{
		if (daysState_ == DaysState.STATE_DAYTIME)
		{
			DateTime dateTime = new DateTime(2018, 6, 8, SLEEPABLE_TIME.hour_, SLEEPABLE_TIME.minute_, SLEEPABLE_TIME.second_);
			return (float)(dateTime - new DateTime(2018, 6, 8, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)).TotalSeconds;
		}
		return float.MaxValue;
	}

	private void WriteSaveData()
	{
		SaveData.SetFloat("SatietyTimer", satietyTimer_);
		SaveData.SetFloat("FeelTimer", feelTimer_);
		SaveData.SetFloat("SickTimer", sickTimer_);
		SaveData.SetFloat("SingTimer", singTimer_);
		SaveData.SetFloat("SatietyLimit", satietyLimit_.timer_);
		SaveData.SetFloat("FeelLimit", feelLimit_.timer_);
		SaveData.SetFloat("SickLimit", sickLimit_.timer_);
		SaveData.SetClass("CurrentPhase", currentPhase_);
		SaveData.Save();
	}

	private void InitDayState()
	{
		int hour = DateTime.Now.Hour;
		int minute = DateTime.Now.Minute;
		int second = DateTime.Now.Second;
		if (WAKEUP_TIME.InnerTime(hour, minute, second))
		{
			daysState_ = DaysState.STATE_DAYTIME;
		}
		if (SLEEPABLE_TIME.InnerTime(hour, minute, second))
		{
			daysState_ = DaysState.STATE_SLEEPABLE;
		}
		if (FORCE_SLEEP_TIME.InnerTime(hour, minute, second))
		{
			daysState_ = DaysState.STATE_NIGHT;
		}
	}

	private void SetCurrentDaysState()
	{
		int hour = DateTime.Now.Hour;
		int minute = DateTime.Now.Minute;
		int second = DateTime.Now.Second;
		if (WAKEUP_TIME.InnerTime(hour, minute, second))
		{
			daysState_ = DaysState.STATE_DAYTIME;
			if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsSleeping() && currentPhase_.IsNightSleeping())
			{
				currentPhase_.SetNightSleep(false);
				currentPhase_.state_ = PhaseEventInfo.PhaseState.PHASE_READY;
				CheckEvolution();
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SLEEPING, false);
			}
		}
		if (SLEEPABLE_TIME.InnerTime(hour, minute, second))
		{
			daysState_ = DaysState.STATE_SLEEPABLE;
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SLEEPY, true);
		}
		if (FORCE_SLEEP_TIME.InnerTime(hour, minute, second))
		{
			daysState_ = DaysState.STATE_NIGHT;
			currentPhase_.SetNightSleep(true);
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SLEEPING, true);
		}
	}

	public void CheckDaysTime()
	{
		if (SingletonMonoBehaviour<GrowthDatabase>.Instance == null || (PortDebug.Enabled && isDebugOverWriteDayState_))
		{
			return;
		}
		switch (daysState_)
		{
		case DaysState.STATE_DAYTIME:
			if (!WAKEUP_TIME.InnerTime(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second))
			{
				SetCurrentDaysState();
			}
			break;
		case DaysState.STATE_SLEEPABLE:
			if (!SLEEPABLE_TIME.InnerTime(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second))
			{
				SetCurrentDaysState();
			}
			break;
		case DaysState.STATE_NIGHT:
			if (!FORCE_SLEEP_TIME.InnerTime(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second))
			{
				SetCurrentDaysState();
			}
			break;
		}
	}

	private void UpdateSatiety()
	{
		if (CanUpdate())
		{
			satietyTimer_ -= Time.deltaTime;
			if (satietyTimer_ <= 0f)
			{
				reflectStatus(PlayerStatusManager.PlayerStatusType.TYPE_SATIETY, 1);
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "CheckStatus");
			}
		}
	}

	private void UpdateFeel()
	{
		if (CanUpdate())
		{
			feelTimer_ -= Time.deltaTime;
			if (feelTimer_ <= 0f)
			{
				reflectStatus(PlayerStatusManager.PlayerStatusType.TYPE_FEEL, 1);
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "CheckStatus");
			}
		}
	}

	private void UpdateSick()
	{
		if (CanUpdate() && SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsDiaper() && !SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsSick())
		{
			sickTimer_ -= Time.deltaTime;
			if (sickTimer_ <= 0f)
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SICK, true);
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "CheckStatus");
			}
		}
	}

	private void ResetSickTimer()
	{
		sickTimer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSickedInterval();
	}

	private void UpdateSing()
	{
		if (CanUpdate() && SingletonMonoBehaviour<GameController>.Instance.CanSing())
		{
			singTimer_ -= Time.deltaTime;
			if (singTimer_ <= 0f)
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetSingable", false);
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "CheckStatus");
			}
		}
	}

	private void ResetSingTimer()
	{
		singTimer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSingInterval();
	}

	private void UpdatePhase()
	{
		if (!CanUpdate())
		{
			if (!IsNight() && !IsSleepable() && currentPhase_.IsSleepingTime())
			{
				currentPhase_.UpdatePhaseTime();
			}
		}
		else
		{
			currentPhase_.UpdatePhase();
		}
	}

	private void UpdateGameOver()
	{
		if (CanUpdate() && !(SingletonMonoBehaviour<PlayerStatusManager>.Instance == null))
		{
			UpdateSatietyLimit();
			UpdateFeelLimit();
			UpdateSickLimit();
		}
	}

	private void UpdateSatietyLimit()
	{
		if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsEmptySatiety())
		{
			satietyLimit_.flag_ = true;
			satietyLimit_.timer_ -= Time.deltaTime;
			if (satietyLimit_.timer_ <= 0f)
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "OnGameEnd", GameController.GameState.GAME_STATE_GAMEOVER_BY_SATIETY);
			}
		}
		else if (satietyLimit_.flag_)
		{
			satietyLimit_.flag_ = false;
			satietyLimit_.timer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSatietyGameOverLimit();
		}
	}

	private void UpdateFeelLimit()
	{
		if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsEmptyFeel())
		{
			feelLimit_.flag_ = true;
			feelLimit_.timer_ -= Time.deltaTime;
			if (feelLimit_.timer_ <= 0f)
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "OnGameEnd", GameController.GameState.GAME_STATE_GAMEOVER_BY_FEEL);
			}
		}
		else if (feelLimit_.flag_)
		{
			feelLimit_.flag_ = false;
			feelLimit_.timer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetFeelGameOverLimit();
		}
	}

	private void UpdateSickLimit()
	{
		if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsSick())
		{
			sickLimit_.flag_ = true;
			sickLimit_.timer_ -= Time.deltaTime;
			if (sickLimit_.timer_ <= 0f)
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "OnGameEnd", GameController.GameState.GAME_STATE_GAMEOVER_BY_SICK);
			}
		}
		else if (sickLimit_.flag_)
		{
			sickLimit_.flag_ = false;
			sickLimit_.timer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSickGameOverLimit();
		}
	}

	private void reflectStatus(PlayerStatusManager.PlayerStatusType type, int cost)
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "AddPlayerStatus", type, -cost);
		ResetTimer(type);
	}

	private int CalcTimeInSleepable()
	{
		int result = 0;
		int num = Utility.ConvertHMStoSec(dateTimeObserber_.LastDateTime.Hour, dateTimeObserber_.LastDateTime.Minute, dateTimeObserber_.LastDateTime.Second);
		int totalSec_ = SLEEPABLE_TIME.totalSec_;
		int totalSec_2 = FORCE_SLEEP_TIME.totalSec_;
		if (num >= totalSec_ && num < totalSec_2)
		{
			result = totalSec_2 - num;
		}
		return result;
	}

	private void CalcCostFromLastPlay()
	{
		float num = 0f;
		if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsSleeping())
		{
			num = ((!IsNightSleeping()) ? currentPhase_.napTime_ : ((float)CalcTimeInSleepable()));
		}
		if (SingletonMonoBehaviour<GameController>.Instance.CanSing())
		{
			float num2 = Mathf.Max((float)dateTimeObserber_.DiffLastTime - num, 0f);
			if (num2 >= singTimer_)
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetSingable", false);
			}
			else
			{
				singTimer_ -= num2;
			}
		}
		float num3 = Mathf.Max((float)dateTimeObserber_.DiffLastTime - num, 0f);
		if (num3 >= satietyTimer_)
		{
			int cost = GetCost(num3, satietyTimer_, SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSatietyDefuseInterval());
			cost = Mathf.Min(cost, SingletonMonoBehaviour<PlayerStatusManager>.Instance.GetSatiety());
			num3 -= satietyTimer_ + (float)Mathf.Max(cost - 1, 0) * SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSatietyDefuseInterval();
			reflectStatus(PlayerStatusManager.PlayerStatusType.TYPE_SATIETY, cost);
		}
		else
		{
			satietyTimer_ -= num3;
		}
		float num4 = Mathf.Max((float)dateTimeObserber_.DiffLastTime - num, 0f);
		if (num4 >= feelTimer_)
		{
			int cost2 = GetCost(num4, feelTimer_, SingletonMonoBehaviour<GrowthDatabase>.Instance.GetFeelDefuseInterval());
			cost2 = Mathf.Min(cost2, SingletonMonoBehaviour<PlayerStatusManager>.Instance.GetFeel());
			num4 -= feelTimer_ + (float)Mathf.Max(cost2 - 1, 0) * SingletonMonoBehaviour<GrowthDatabase>.Instance.GetFeelDefuseInterval();
			reflectStatus(PlayerStatusManager.PlayerStatusType.TYPE_FEEL, cost2);
		}
		else
		{
			feelTimer_ -= num4;
		}
		if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsEmptySatiety() && num3 >= 0f && (!SingletonMonoBehaviour<GameController>.Instance.IsGameOver() || !SingletonMonoBehaviour<GameController>.Instance.IsGameClear()))
		{
			CalcLimitFromLastPlay(ref satietyLimit_.timer_, num3, GameController.GameState.GAME_STATE_GAMEOVER_BY_SATIETY);
		}
		else if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsEmptyFeel() && num4 >= 0f && (!SingletonMonoBehaviour<GameController>.Instance.IsGameOver() || !SingletonMonoBehaviour<GameController>.Instance.IsGameClear()))
		{
			CalcLimitFromLastPlay(ref feelLimit_.timer_, num4, GameController.GameState.GAME_STATE_GAMEOVER_BY_FEEL);
		}
		else if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsSick())
		{
			float num5 = Mathf.Max((float)dateTimeObserber_.DiffLastTime - num, 0f);
			if (num5 >= sickLimit_.timer_)
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "OnGameEnd", GameController.GameState.GAME_STATE_GAMEOVER_BY_SICK);
			}
			else
			{
				sickLimit_.timer_ -= num5;
			}
		}
	}

	private int GetCost(float diff, float timer, float Interval)
	{
		int num = 1;
		float num2 = diff - timer;
		int num3 = (int)(num2 / Interval);
		return num + num3;
	}

	private void CalcLimitFromLastPlay(ref float timer, float diff, GameController.GameState type)
	{
		timer -= diff;
		if (timer <= 0f)
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "OnGameEnd", type);
		}
	}

	private void CalcPhaseFromLastPlay()
	{
		if (currentPhase_.isNew_ || SingletonMonoBehaviour<GameController>.Instance.IsGameOver() || SingletonMonoBehaviour<GameController>.Instance.IsGameClear())
		{
			return;
		}
		if (dateTimeObserber_.IsOverNight() && IsDaytime())
		{
			ResetLimitTimer();
			currentPhase_.state_ = PhaseEventInfo.PhaseState.PHASE_READY;
			CheckEvolution();
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SLEEPING, false);
			if (SingletonMonoBehaviour<GameController>.Instance.IsGameClear())
			{
				if (Define.IsActiveScene(Define.SceneId.SCENE_MAIN))
				{
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "GameClearJoy");
				}
				return;
			}
		}
		bool flag = false;
		if ((IsSleepable() || IsNight()) && IsNightSleeping())
		{
			flag = true;
		}
		if (!flag)
		{
			float num = (float)dateTimeObserber_.DiffLastTime;
			while (num > 0f)
			{
				if (currentPhase_.state_ == PhaseEventInfo.PhaseState.PHASE_ACTIVE_TIME)
				{
					currentPhase_.activeTime_ -= num;
					if (currentPhase_.activeTime_ <= 0f)
					{
						num = 0f - currentPhase_.activeTime_;
						currentPhase_.state_ = PhaseEventInfo.PhaseState.PHASE_NAPABLE_TIME;
					}
					else
					{
						num = -1f;
					}
					continue;
				}
				if (currentPhase_.state_ == PhaseEventInfo.PhaseState.PHASE_NAPABLE_TIME)
				{
					currentPhase_.napableTime_ -= num;
					if (currentPhase_.napableTime_ <= 0f)
					{
						num = 0f - currentPhase_.napableTime_;
						currentPhase_.state_ = PhaseEventInfo.PhaseState.PHASE_ACTIVE_TIME;
						SetNextPhase();
						if (SingletonMonoBehaviour<GameController>.Instance.IsGameClear())
						{
							if (Define.IsActiveScene(Define.SceneId.SCENE_MAIN))
							{
								MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "GameClearJoy");
							}
							break;
						}
					}
					else
					{
						num = -1f;
					}
					continue;
				}
				if (currentPhase_.state_ != PhaseEventInfo.PhaseState.PHASE_NAP_TIME)
				{
					break;
				}
				currentPhase_.napTime_ -= num;
				if (currentPhase_.napTime_ <= 0f)
				{
					num = 0f - currentPhase_.napTime_;
					currentPhase_.state_ = PhaseEventInfo.PhaseState.PHASE_READY;
					CheckEvolution();
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SLEEPING, false);
					if (currentPhase_.IsNightSleeping())
					{
						currentPhase_.SetNightSleep(false);
					}
					if (SingletonMonoBehaviour<GameController>.Instance.IsGameClear())
					{
						if (Define.IsActiveScene(Define.SceneId.SCENE_MAIN))
						{
							MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "GameClearJoy");
						}
						break;
					}
				}
				else
				{
					num = -1f;
				}
			}
		}
		switch (currentPhase_.state_)
		{
		case PhaseEventInfo.PhaseState.PHASE_ACTIVE_TIME:
			if (IsDaytime())
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SLEEPING, false);
			}
			break;
		case PhaseEventInfo.PhaseState.PHASE_NAPABLE_TIME:
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SLEEPY, true);
			break;
		case PhaseEventInfo.PhaseState.PHASE_NAP_TIME:
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SLEEPING, true);
			break;
		}
		if (!flag)
		{
			if (CalcDiffTimeEvent(ref currentPhase_.cookParams_))
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetCookable", true);
			}
			if (CalcDiffTimeEvent(ref currentPhase_.singParams_))
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetSingable", true);
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "CheckStatus");
			}
			if (CalcDiffTimeEvent(ref currentPhase_.sickParams_))
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SICK, true);
			}
			if (CalcDiffTimeEvent(ref currentPhase_.diaperParams_))
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_DIAPER, true);
			}
		}
	}

	private bool CalcDiffTimeEvent(ref List<PhaseEventInfo.Matter> list)
	{
		bool flag = false;
		float num = (float)dateTimeObserber_.DiffLastTime;
		while (list.Count > 0)
		{
			list[0].time_ -= num;
			if (list[0].time_ <= 0f)
			{
				bool flag2 = Utility.SuccessRandom(list[0].probability_);
				flag = ((!flag2) ? flag : flag2);
				num = 0f - list[0].time_;
				list.RemoveAt(0);
				continue;
			}
			return flag;
		}
		return flag;
	}

	private void ResetTimer(PlayerStatusManager.PlayerStatusType type)
	{
		switch (type)
		{
		case PlayerStatusManager.PlayerStatusType.TYPE_SATIETY:
			satietyTimer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSatietyDefuseInterval();
			break;
		case PlayerStatusManager.PlayerStatusType.TYPE_FEEL:
			feelTimer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetFeelDefuseInterval();
			break;
		default:
			Debug.LogError("[EventScheduler::ResetTimer] Invalid type.");
			break;
		}
	}

	private void CheckEvolution()
	{
		List<Define.CharaId> list = SingletonMonoBehaviour<GrowthDatabase>.Instance.CollectGetableCostume();
		if (list.Count > 0)
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "RaiseGameFlag", GameController.GameFlag.FLAG_GET_COSTUME);
			if (Define.IsActiveScene(Define.SceneId.SCENE_MAIN))
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CONTROLLER), "OpenCostumeGetMenu", list);
			}
		}
		else
		{
			SetNextPhase();
		}
	}

	private void SetNextPhase()
	{
		if (currentPhase_.activeTime_ > 0f)
		{
			GrowthDatabase.PhaseInfo phaseInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindPhaseInfo(currentPhase_.id_);
			currentPhase_.state_ = PhaseEventInfo.PhaseState.PHASE_ACTIVE_TIME;
			currentPhase_.napableTime_ = phaseInfo.GetNapableTime();
			currentPhase_.napTime_ = phaseInfo.GetNapTime();
			return;
		}
		if (PortDebug.Enabled)
		{
			SavePhaseFinalState();
		}
		currentPhase_.ForwardPhase();
		SetupPhase();
	}

	private void SetupPhase()
	{
		if (!(SingletonMonoBehaviour<GrowthDatabase>.Instance == null) && currentPhase_.id_ < SingletonMonoBehaviour<GrowthDatabase>.Instance.GetTotalPhaseNum())
		{
			GrowthDatabase.PhaseInfo phaseInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindPhaseInfo(currentPhase_.id_);
			currentPhase_.state_ = PhaseEventInfo.PhaseState.PHASE_ACTIVE_TIME;
			currentPhase_.activeTime_ = phaseInfo.GetActiveTime();
			currentPhase_.napableTime_ = phaseInfo.GetNapableTime();
			currentPhase_.napTime_ = phaseInfo.GetNapTime();
			SetEventTime(ref currentPhase_.cookParams_, phaseInfo.GetActiveTime(), phaseInfo.GetCookNum(), phaseInfo.GetCookProbability());
			SetEventTime(ref currentPhase_.singParams_, phaseInfo.GetActiveTime(), phaseInfo.GetSingNum(), phaseInfo.GetSingProbability());
			SetEventTime(ref currentPhase_.sickParams_, phaseInfo.GetActiveTime(), phaseInfo.GetSickNum(), phaseInfo.GetSickProbability());
			SetEventTime(ref currentPhase_.diaperParams_, phaseInfo.GetActiveTime(), phaseInfo.GetDiaperNum(), phaseInfo.GetDiaperProbability());
		}
	}

	private void InitializePhase()
	{
		currentPhase_ = SaveData.GetClass("CurrentPhase", new PhaseEventInfo());
		if (currentPhase_.isNew_)
		{
			SetupPhase();
			currentPhase_.isNew_ = false;
			SaveData.SetClass("CurrentPhase", currentPhase_);
		}
	}

	private void SetEventTime(ref List<PhaseEventInfo.Matter> list, float activeTime, int num, int probability)
	{
		list.Clear();
		float t = activeTime / (float)(num + 1);
		for (int i = 0; i < num; i++)
		{
			list.Add(new PhaseEventInfo.Matter(t, probability));
		}
	}

	private void OnSleep()
	{
		ResetLimitTimer();
		currentPhase_.OnSleep();
		if (IsSleepable() || IsNight())
		{
			currentPhase_.SetNightSleep(true);
			return;
		}
		DateTime dateTime = DateTime.Now.AddSeconds(currentPhase_.napTime_);
		if (SLEEPABLE_TIME.InnerTime(dateTime.Hour, dateTime.Minute, dateTime.Second))
		{
			currentPhase_.SetNightSleep(true);
		}
	}

	private bool CanUpdate()
	{
		if (SingletonMonoBehaviour<GameController>.Instance.IsGameClear() || SingletonMonoBehaviour<GameController>.Instance.IsGameOver())
		{
			return false;
		}
		if (IsPhazeZero())
		{
			return true;
		}
		if (IsNight())
		{
			return false;
		}
		if (currentPhase_.IsSleepingTime())
		{
			return false;
		}
		return true;
	}

	private bool IsPhazeZero()
	{
		if (currentPhase_.id_ == 0 && (currentPhase_.state_ == PhaseEventInfo.PhaseState.PHASE_ACTIVE_TIME || currentPhase_.state_ == PhaseEventInfo.PhaseState.PHASE_NAPABLE_TIME))
		{
			return true;
		}
		return false;
	}

	private void ResetGame()
	{
		if (SingletonMonoBehaviour<GrowthDatabase>.Instance != null)
		{
			daysState_ = DaysState.STATE_DAYTIME;
			satietyTimer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSatietyDefuseInterval();
			feelTimer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetFeelDefuseInterval();
			sickTimer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSickedInterval();
			singTimer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSingInterval();
			satietyLimit_.flag_ = false;
			satietyLimit_.timer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSatietyGameOverLimit();
			feelLimit_.flag_ = false;
			feelLimit_.timer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetFeelGameOverLimit();
			sickLimit_.flag_ = false;
			sickLimit_.timer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSickGameOverLimit();
			currentPhase_.Initialize();
			WriteSaveData();
		}
	}

	private void ResetLimitTimer()
	{
		satietyLimit_.timer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSatietyGameOverLimit();
		feelLimit_.timer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetFeelGameOverLimit();
		sickLimit_.timer_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSickGameOverLimit();
	}

	public bool IsNightSleeping()
	{
		return currentPhase_.IsNightSleeping();
	}

	public bool IsDaytime()
	{
		return daysState_ == DaysState.STATE_DAYTIME;
	}

	public bool IsSleepable()
	{
		return daysState_ == DaysState.STATE_SLEEPABLE;
	}

	public bool IsNight()
	{
		return daysState_ == DaysState.STATE_NIGHT;
	}

	public string GetCurrentDayStateString()
	{
		return daysState_.ToString();
	}

	public bool GetCurrentPhaseNew()
	{
		return currentPhase_.isNew_;
	}

	public int GetCurrentPhaseIndex()
	{
		return currentPhase_.id_;
	}

	public float GetNextSatietyEventTIme()
	{
		return satietyTimer_;
	}

	public float GetNextFeelEventTime()
	{
		return feelTimer_;
	}

	public float GetNextSickedTime()
	{
		return sickTimer_;
	}

	public float GetNextUnSingTime()
	{
		return singTimer_;
	}

	public float GetSatietyLimit()
	{
		return satietyLimit_.timer_;
	}

	public float GetFeelLimit()
	{
		return feelLimit_.timer_;
	}

	public float GetSickLimit()
	{
		return sickLimit_.timer_;
	}

	public PhaseEventInfo.PhaseState GetCurrentPhaseState()
	{
		return currentPhase_.state_;
	}

	public float GetPhaseTimer()
	{
		if (currentPhase_.state_ == PhaseEventInfo.PhaseState.PHASE_ACTIVE_TIME)
		{
			return currentPhase_.activeTime_;
		}
		if (currentPhase_.state_ == PhaseEventInfo.PhaseState.PHASE_NAPABLE_TIME)
		{
			return currentPhase_.napableTime_;
		}
		if (currentPhase_.state_ == PhaseEventInfo.PhaseState.PHASE_NAP_TIME)
		{
			return currentPhase_.napTime_;
		}
		return 0f;
	}

	public float GetPhaseActiveTime()
	{
		return currentPhase_.activeTime_;
	}

	public float GetPhaseNapableTime()
	{
		return currentPhase_.napableTime_;
	}

	public float GetPhaseNapTime()
	{
		return currentPhase_.napTime_;
	}

	public float GetCookTime()
	{
		if (currentPhase_.cookParams_.Count == 0)
		{
			return 0f;
		}
		return currentPhase_.cookParams_[0].time_;
	}

	public float GetSingTime()
	{
		if (currentPhase_.singParams_.Count == 0)
		{
			return 0f;
		}
		return currentPhase_.singParams_[0].time_;
	}

	public float GetSickTime()
	{
		if (currentPhase_.sickParams_.Count == 0)
		{
			return 0f;
		}
		return currentPhase_.sickParams_[0].time_;
	}

	public float GetDiaperTime()
	{
		if (currentPhase_.diaperParams_.Count == 0)
		{
			return 0f;
		}
		return currentPhase_.diaperParams_[0].time_;
	}

	private void DebugInput()
	{
		if (PortDebug.Enabled && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.UpArrow))
		{
			ForwardTimer(1000f);
		}
	}

	private void ForwardTimer(float diff)
	{
		satietyTimer_ -= diff;
		feelTimer_ -= diff;
		if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsDiaper())
		{
			sickTimer_ -= diff;
		}
		if (SingletonMonoBehaviour<GameController>.Instance.CanSing())
		{
			singTimer_ -= diff;
		}
		if (currentPhase_.state_ == PhaseEventInfo.PhaseState.PHASE_ACTIVE_TIME)
		{
			currentPhase_.activeTime_ -= diff;
		}
		else if (currentPhase_.state_ == PhaseEventInfo.PhaseState.PHASE_NAPABLE_TIME)
		{
			currentPhase_.napableTime_ -= diff;
		}
		else if (currentPhase_.state_ == PhaseEventInfo.PhaseState.PHASE_NAP_TIME)
		{
			currentPhase_.napTime_ -= diff;
		}
		if (currentPhase_.cookParams_.Count > 0)
		{
			currentPhase_.cookParams_[0].time_ -= diff;
		}
		if (currentPhase_.singParams_.Count > 0)
		{
			currentPhase_.singParams_[0].time_ -= diff;
		}
		if (currentPhase_.sickParams_.Count > 0)
		{
			currentPhase_.sickParams_[0].time_ -= diff;
		}
		if (currentPhase_.diaperParams_.Count > 0)
		{
			currentPhase_.diaperParams_[0].time_ -= diff;
		}
		if (satietyLimit_.flag_)
		{
			satietyLimit_.timer_ -= diff;
		}
		if (feelLimit_.flag_)
		{
			feelLimit_.timer_ -= diff;
		}
		if (sickLimit_.flag_)
		{
			sickLimit_.timer_ -= diff;
		}
	}

	private void DebugOverWriteDayState(DaysState ds)
	{
		if (PortDebug.Enabled)
		{
			OverWriteCurrentDaysState(ds);
			isDebugOverWriteDayState_ = true;
		}
	}

	private void EndOverWriteDayState()
	{
		isDebugOverWriteDayState_ = false;
	}

	private void OverWriteCurrentDaysState(DaysState ds)
	{
		daysState_ = ds;
		switch (ds)
		{
		case DaysState.STATE_DAYTIME:
			if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsSleeping() && currentPhase_.IsNightSleeping())
			{
				currentPhase_.SetNightSleep(false);
				currentPhase_.state_ = PhaseEventInfo.PhaseState.PHASE_READY;
				CheckEvolution();
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SLEEPING, false);
			}
			break;
		case DaysState.STATE_SLEEPABLE:
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SLEEPY, true);
			break;
		case DaysState.STATE_NIGHT:
			currentPhase_.SetNightSleep(true);
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SLEEPING, true);
			break;
		}
	}

	private void SavePhaseFinalState()
	{
		if (PortDebug.Enabled)
		{
			PlayerStatusManager.PlayerStatus playerStatus = SaveData.GetClass("PlayerStatus", new PlayerStatusManager.PlayerStatus());
			DateTimeObserber dateTimeObserber = UnityEngine.Object.FindObjectOfType<DateTimeObserber>();
			string text = "Phase" + currentPhase_.id_;
			SaveData.SetFloat(text + "Weight", playerStatus.weight_);
			SaveData.SetInt(text + "Hour", dateTimeObserber.GetHoursFromGameStart());
			SaveData.SetInt(text + "Minute", dateTimeObserber.GetMinutesFromGameStart());
			SaveData.SetInt(text + "Second", dateTimeObserber.GetSecondsFromGameStart());
			SaveData.SetFloat(text + "Strength", playerStatus.strength_);
			SaveData.SetFloat(text + "Intelligence", playerStatus.intelligence_);
			SaveData.SetFloat(text + "Magic", playerStatus.magic_);
		}
	}
}
