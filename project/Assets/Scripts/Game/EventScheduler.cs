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
			int sec = Utility.ConvertHMStoSec(h, m, s);
			if (totalNextSec_ < totalSec_)
			{
				return sec < totalNextSec_ || sec >= totalSec_;
			}
			return sec >= totalSec_ && sec < totalNextSec_;
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

	private void Start()
	{
		if (DebugMode.Enabled)
		{
			UnityEngine.Object.FindAnyObjectByType<DebugMenu>().SendMessage("SetupLog");
			UnityEngine.Object.FindAnyObjectByType<DebugMenu>().SendMessage("AddResumeLog", "App Booted.");
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
			if (DebugMode.Enabled)
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
				if (DebugMode.Enabled)
				{
					UnityEngine.Object.FindAnyObjectByType<DebugMenu>().SendMessage("AddResumeLog", "Pause On.");
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
			if (DebugMode.Enabled)
			{
				UnityEngine.Object.FindAnyObjectByType<DebugMenu>().SendMessage("AddResumeLog", "Pause Off.");
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
			if (DebugMode.Enabled)
			{
				UnityEngine.Object.FindAnyObjectByType<DebugMenu>().SendMessage("AddResumeLog", "App Quited.");
			}
		}
	}

	private void PushNotification()
	{
		if (SingletonMonoBehaviour<GameController>.Instance == null || SingletonMonoBehaviour<GameController>.Instance.IsGameClear() || SingletonMonoBehaviour<GameController>.Instance.IsGameOver() || currentPhase_.isNew_)
		{
			return;
		}
		if (DebugMode.Enabled)
		{
			UnityEngine.Object.FindAnyObjectByType<DebugMenu>().SendMessage("AddResumeLog", "Try Push Notification.");
		}
		int delaySeconds = FindNearCallingEvent();
		if (DebugMode.Enabled)
		{
			UnityEngine.Object.FindAnyObjectByType<DebugMenu>().SendMessage("AddResumeLog", "Found Delay Time ___ : " + delaySeconds);
		}
		if (delaySeconds > 0)
		{
			if (DebugMode.Enabled)
			{
				UnityEngine.Object.FindAnyObjectByType<DebugMenu>().SendMessage("AddResumeLog", "Register Notification.");
			}
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_LOCAL_PUSH), "RegisterLocalNotification", notificationTitle_, notificationMessage_, delaySeconds, -1);
		}
	}

	private void CancellNotification()
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_LOCAL_PUSH), "CancelLocalNotification");
	}

	private int FindNearCallingEvent()
	{
		float sleepRemaining = 0f;
		if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsSleeping())
		{
			TimeSpan wakeupTime = new TimeSpan(WAKEUP_TIME.hour_, WAKEUP_TIME.minute_, WAKEUP_TIME.second_);
			TimeSpan oneDay = new TimeSpan(24, 0, 0);
			TimeSpan now = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
			float wakeupSec = (float)wakeupTime.TotalSeconds;
			float daySec = (float)oneDay.TotalSeconds;
			float nowSec = (float)now.TotalSeconds;
			if (IsNightSleeping())
			{
				sleepRemaining = (nowSec < wakeupSec) ? (wakeupSec - nowSec) : (daySec - nowSec + wakeupSec);
			}
			else
			{
				float napTime = currentPhase_.napTime_;
				TimeSpan napEnd = now.Add(new TimeSpan(0, 0, (int)napTime));
				TimeSpan sleepableTime = new TimeSpan(SLEEPABLE_TIME.hour_, SLEEPABLE_TIME.minute_, SLEEPABLE_TIME.second_);
				sleepRemaining = (napEnd.TotalSeconds >= sleepableTime.TotalSeconds) ? (daySec - nowSec + wakeupSec) : napTime;
			}
			List<float> candidates = new List<float>();
			float satietyEmpty = FindNextSatietyEmptyTime() + sleepRemaining;
			if (satietyEmpty > 0f)
			{
				candidates.Add(satietyEmpty);
			}
			float feelEmpty = FindNextFeelEmptyTime() + sleepRemaining;
			if (feelEmpty > 0f)
			{
				candidates.Add(feelEmpty);
			}
			if (candidates.Count > 0)
			{
				return (int)candidates.Min() + 10;
			}
			return 0;
		}
		TimeSpan forceSleepTime = new TimeSpan(FORCE_SLEEP_TIME.hour_, FORCE_SLEEP_TIME.minute_, FORCE_SLEEP_TIME.second_);
		TimeSpan nowAwake = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
		float untilForceSleep = (float)(forceSleepTime - nowAwake).TotalSeconds;
		List<float> awakeCandidates = new List<float>();
		float nextSatietyEmpty = FindNextSatietyEmptyTime();
		if (nextSatietyEmpty > 0f && nextSatietyEmpty < untilForceSleep)
		{
			awakeCandidates.Add(nextSatietyEmpty);
		}
		float nextFeelEmpty = FindNextFeelEmptyTime();
		if (nextFeelEmpty > 0f && nextFeelEmpty < untilForceSleep)
		{
			awakeCandidates.Add(nextFeelEmpty);
		}
		float nextSing = FindNextSingTime();
		if (nextSing > 0f && nextSing < untilForceSleep)
		{
			awakeCandidates.Add(nextSing);
		}
		float nextDiaper = FindNextDiaperTime();
		if (nextDiaper > 0f && nextDiaper < untilForceSleep)
		{
			awakeCandidates.Add(nextDiaper);
		}
		float nextNap = FindNextNapTime();
		if (nextNap > 0f && nextNap < untilForceSleep)
		{
			awakeCandidates.Add(nextNap);
		}
		float nextSleep = FindNextSleepTime();
		if (nextSleep > 0f && nextSleep < untilForceSleep)
		{
			awakeCandidates.Add(nextSleep);
		}
		if (awakeCandidates.Count > 0)
		{
			return (int)awakeCandidates.Min() + 10;
		}
		return 0;
	}

	private float FindNextSatietyEmptyTime()
	{
		int heartsLeft = Mathf.Max(SingletonMonoBehaviour<PlayerStatusManager>.Instance.GetSatiety() - 1, 0);
		return satietyTimer_ + (float)heartsLeft * SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSatietyDefuseInterval();
	}

	private float FindNextFeelEmptyTime()
	{
		int heartsLeft = Mathf.Max(SingletonMonoBehaviour<PlayerStatusManager>.Instance.GetFeel() - 1, 0);
		return feelTimer_ + (float)heartsLeft * SingletonMonoBehaviour<GrowthDatabase>.Instance.GetFeelDefuseInterval();
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
		if (SingletonMonoBehaviour<GrowthDatabase>.Instance == null || (DebugMode.Enabled && isDebugOverWriteDayState_))
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
		if (CanUpdate() && SingletonMonoBehaviour<PlayerStatusManager>.Instance != null)
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
		int lastPlaySec = Utility.ConvertHMStoSec(dateTimeObserber_.LastDateTime.Hour, dateTimeObserber_.LastDateTime.Minute, dateTimeObserber_.LastDateTime.Second);
		int sleepableStart = SLEEPABLE_TIME.totalSec_;
		int forceSleepStart = FORCE_SLEEP_TIME.totalSec_;
		if (lastPlaySec >= sleepableStart && lastPlaySec < forceSleepStart)
		{
			result = forceSleepStart - lastPlaySec;
		}
		return result;
	}

	private void CalcCostFromLastPlay()
	{
		float sleepTime = 0f;
		if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsSleeping())
		{
			sleepTime = IsNightSleeping() ? CalcTimeInSleepable() : currentPhase_.napTime_;
		}
		if (SingletonMonoBehaviour<GameController>.Instance.CanSing())
		{
			float awakeTime = Mathf.Max((float)dateTimeObserber_.DiffLastTime - sleepTime, 0f);
			if (awakeTime >= singTimer_)
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetSingable", false);
			}
			else
			{
				singTimer_ -= awakeTime;
			}
		}
		float satietyElapsed = Mathf.Max((float)dateTimeObserber_.DiffLastTime - sleepTime, 0f);
		if (satietyElapsed >= satietyTimer_)
		{
			int satietyCost = GetCost(satietyElapsed, satietyTimer_, SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSatietyDefuseInterval());
			satietyCost = Mathf.Min(satietyCost, SingletonMonoBehaviour<PlayerStatusManager>.Instance.GetSatiety());
			satietyElapsed -= satietyTimer_ + (float)Mathf.Max(satietyCost - 1, 0) * SingletonMonoBehaviour<GrowthDatabase>.Instance.GetSatietyDefuseInterval();
			reflectStatus(PlayerStatusManager.PlayerStatusType.TYPE_SATIETY, satietyCost);
		}
		else
		{
			satietyTimer_ -= satietyElapsed;
		}
		float feelElapsed = Mathf.Max((float)dateTimeObserber_.DiffLastTime - sleepTime, 0f);
		if (feelElapsed >= feelTimer_)
		{
			int feelCost = GetCost(feelElapsed, feelTimer_, SingletonMonoBehaviour<GrowthDatabase>.Instance.GetFeelDefuseInterval());
			feelCost = Mathf.Min(feelCost, SingletonMonoBehaviour<PlayerStatusManager>.Instance.GetFeel());
			feelElapsed -= feelTimer_ + (float)Mathf.Max(feelCost - 1, 0) * SingletonMonoBehaviour<GrowthDatabase>.Instance.GetFeelDefuseInterval();
			reflectStatus(PlayerStatusManager.PlayerStatusType.TYPE_FEEL, feelCost);
		}
		else
		{
			feelTimer_ -= feelElapsed;
		}
		if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsEmptySatiety() && satietyElapsed >= 0f && (!SingletonMonoBehaviour<GameController>.Instance.IsGameOver() || !SingletonMonoBehaviour<GameController>.Instance.IsGameClear()))
		{
			CalcLimitFromLastPlay(ref satietyLimit_.timer_, satietyElapsed, GameController.GameState.GAME_STATE_GAMEOVER_BY_SATIETY);
		}
		else if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsEmptyFeel() && feelElapsed >= 0f && (!SingletonMonoBehaviour<GameController>.Instance.IsGameOver() || !SingletonMonoBehaviour<GameController>.Instance.IsGameClear()))
		{
			CalcLimitFromLastPlay(ref feelLimit_.timer_, feelElapsed, GameController.GameState.GAME_STATE_GAMEOVER_BY_FEEL);
		}
		else if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsSick())
		{
			float sickElapsed = Mathf.Max((float)dateTimeObserber_.DiffLastTime - sleepTime, 0f);
			if (sickElapsed >= sickLimit_.timer_)
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "OnGameEnd", GameController.GameState.GAME_STATE_GAMEOVER_BY_SICK);
			}
			else
			{
				sickLimit_.timer_ -= sickElapsed;
			}
		}
	}

	private int GetCost(float diff, float timer, float interval)
	{
		return 1 + (int)((diff - timer) / interval);
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
		bool sleptAtNight = false;
		if ((IsSleepable() || IsNight()) && IsNightSleeping())
		{
			sleptAtNight = true;
		}
		if (!sleptAtNight)
		{
			float remaining = (float)dateTimeObserber_.DiffLastTime;
			while (remaining > 0f)
			{
				if (currentPhase_.state_ == PhaseEventInfo.PhaseState.PHASE_ACTIVE_TIME)
				{
					currentPhase_.activeTime_ -= remaining;
					if (currentPhase_.activeTime_ <= 0f)
					{
						remaining = -currentPhase_.activeTime_;
						currentPhase_.state_ = PhaseEventInfo.PhaseState.PHASE_NAPABLE_TIME;
					}
					else
					{
						remaining = -1f;
					}
					continue;
				}
				if (currentPhase_.state_ == PhaseEventInfo.PhaseState.PHASE_NAPABLE_TIME)
				{
					currentPhase_.napableTime_ -= remaining;
					if (currentPhase_.napableTime_ <= 0f)
					{
						remaining = -currentPhase_.napableTime_;
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
						remaining = -1f;
					}
					continue;
				}
				if (currentPhase_.state_ != PhaseEventInfo.PhaseState.PHASE_NAP_TIME)
				{
					break;
				}
				currentPhase_.napTime_ -= remaining;
				if (currentPhase_.napTime_ <= 0f)
				{
					remaining = -currentPhase_.napTime_;
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
					remaining = -1f;
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
		if (!sleptAtNight)
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
		bool triggered = false;
		float elapsed = (float)dateTimeObserber_.DiffLastTime;
		while (list.Count > 0)
		{
			list[0].time_ -= elapsed;
			if (list[0].time_ <= 0f)
			{
				if (Utility.SuccessRandom(list[0].probability_))
				{
					triggered = true;
				}
				elapsed = -list[0].time_;
				list.RemoveAt(0);
				continue;
			}
			return triggered;
		}
		return triggered;
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
		List<Define.CharaId> costumes = SingletonMonoBehaviour<GrowthDatabase>.Instance.CollectGetableCostume();
		if (costumes.Count > 0)
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "RaiseGameFlag", GameController.GameFlag.FLAG_GET_COSTUME);
			if (Define.IsActiveScene(Define.SceneId.SCENE_MAIN))
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CONTROLLER), "OpenCostumeGetMenu", costumes);
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
		if (DebugMode.Enabled)
		{
			SavePhaseFinalState();
		}
		currentPhase_.ForwardPhase();
		SetupPhase();
	}

	private void SetupPhase()
	{
		if (SingletonMonoBehaviour<GrowthDatabase>.Instance != null && currentPhase_.id_ < SingletonMonoBehaviour<GrowthDatabase>.Instance.GetTotalPhaseNum())
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

	private void SetEventTime(ref List<PhaseEventInfo.Matter> list, float activeTime, int count, int probability)
	{
		list.Clear();
		float spacing = activeTime / (float)(count + 1);
		for (int i = 0; i < count; i++)
		{
			list.Add(new PhaseEventInfo.Matter(spacing, probability));
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
		return !currentPhase_.IsSleepingTime();
	}

	private bool IsPhazeZero()
	{
		return currentPhase_.id_ == 0 && (currentPhase_.state_ == PhaseEventInfo.PhaseState.PHASE_ACTIVE_TIME || currentPhase_.state_ == PhaseEventInfo.PhaseState.PHASE_NAPABLE_TIME);
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
		if (DebugMode.Enabled && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.UpArrow))
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
		if (DebugMode.Enabled)
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
		if (DebugMode.Enabled)
		{
			PlayerStatusManager.PlayerStatus playerStatus = SaveData.GetClass("PlayerStatus", new PlayerStatusManager.PlayerStatus());
			DateTimeObserber observer = UnityEngine.Object.FindAnyObjectByType<DateTimeObserber>();
			string prefix = "Phase" + currentPhase_.id_;
			SaveData.SetFloat(prefix + "Weight", playerStatus.weight_);
			SaveData.SetInt(prefix + "Hour", observer.GetHoursFromGameStart());
			SaveData.SetInt(prefix + "Minute", observer.GetMinutesFromGameStart());
			SaveData.SetInt(prefix + "Second", observer.GetSecondsFromGameStart());
			SaveData.SetFloat(prefix + "Strength", playerStatus.strength_);
			SaveData.SetFloat(prefix + "Intelligence", playerStatus.intelligence_);
			SaveData.SetFloat(prefix + "Magic", playerStatus.magic_);
		}
	}
}
