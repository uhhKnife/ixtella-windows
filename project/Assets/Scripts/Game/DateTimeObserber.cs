using System;
using System.Collections.Generic;
using UnityEngine;

public class DateTimeObserber : MonoBehaviour
{
	private DateTime startRealTime_;

	private float startTime_;

	public float timeRate_ = 1f;

	private float elapseSecond_;

	private DateTime lastRealTime_;

	private double diffLastTime_;

	public bool isTimePause_;

	private bool isOverNight_;

	private TimeSpan elapseRealTimeSpan_ = default(TimeSpan);

	public float TimeRate
	{
		get
		{
			return timeRate_;
		}
	}

	public float ElapseTime
	{
		get
		{
			return elapseSecond_;
		}
	}

	public DateTime LastDateTime
	{
		get
		{
			return lastRealTime_;
		}
	}

	public string LastPlayedTime
	{
		get
		{
			return lastRealTime_.ToString();
		}
	}

	public double DiffLastTime
	{
		get
		{
			return diffLastTime_;
		}
	}

	private void Awake()
	{
		timeRate_ = 1f;
	}

	private void Start()
	{
		startTime_ = Time.time;
		startRealTime_ = FromList(SaveData.GetList("StartGameTime", ToList(DateTime.Now)));
		calcRealTime();
		GetDiffTimeFromLastPlay();
	}

	private void Update()
	{
		if (PortDebug.Enabled)
		{
			input();
		}
		if (!SingletonMonoBehaviour<GameController>.Instance.IsGameClear() && !SingletonMonoBehaviour<GameController>.Instance.IsGameOver())
		{
			calcIngameTime();
			calcRealTime();
		}
	}

	private void OnApplicationQuit()
	{
		WriteSaveData();
	}

	private void WriteSaveData()
	{
		SaveData.SetList("lastTime", ToList(DateTime.Now));
		SaveData.Save();
	}

	private static List<int> ToList(DateTime time)
	{
		return new List<int> { time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second };
	}

	private static DateTime FromList(List<int> list)
	{
		return new DateTime(list[0], list[1], list[2], list[3], list[4], list[5]);
	}

	private static TimeSpan ClockTime(int hour, int minute, int second)
	{
		return new TimeSpan(hour, minute, second);
	}

	// Time away is capped at a day, and the night (FORCE_SLEEP_TIME to WAKEUP_TIME) doesn't count.
	private void GetDiffTimeFromLastPlay()
	{
		DateTime now = DateTime.Now;
		lastRealTime_ = FromList(SaveData.GetList("lastTime", ToList(now)));
		TimeSpan diff = now - lastRealTime_;
		if (diff.Days >= 1)
		{
			diff = new TimeSpan(24, 0, 0);
		}
		else
		{
			GrowthDatabase.NightTimeInfo sleep = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindNightInfo("FORCE_SLEEP_TIME");
			GrowthDatabase.NightTimeInfo wakeup = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindNightInfo("WAKEUP_TIME");
			TimeSpan midnight = new TimeSpan(24, 0, 0);
			TimeSpan lastClock = ClockTime(lastRealTime_.Hour, lastRealTime_.Minute, lastRealTime_.Second);
			TimeSpan nowClock = ClockTime(now.Hour, now.Minute, now.Second);
			TimeSpan sleepClock = ClockTime(sleep.GetHour(), sleep.GetMinute(), sleep.GetSecond());
			TimeSpan wakeupClock = ClockTime(wakeup.GetHour(), wakeup.GetMinute(), wakeup.GetSecond());
			if (now.Hour < lastRealTime_.Hour)
			{
				diff -= midnight - ((lastRealTime_.Hour > sleep.GetHour()) ? lastClock : sleepClock);
				diff -= (now.Hour > wakeup.GetHour()) ? wakeupClock : nowClock;
				isOverNight_ = true;
			}
			else
			{
				if (lastRealTime_.Hour >= sleep.GetHour() || now.Hour < wakeup.GetHour())
				{
					diff = TimeSpan.Zero;
				}
				else
				{
					if (now.Hour >= sleep.GetHour())
					{
						diff -= nowClock - sleepClock;
					}
					if (lastRealTime_.Hour < wakeup.GetHour())
					{
						diff -= wakeupClock - lastClock;
					}
				}
				isOverNight_ = false;
			}
		}
		diffLastTime_ = diff.TotalSeconds;
	}

	private void calcIngameTime()
	{
		elapseSecond_ = Time.time - startTime_;
	}

	private void calcRealTime()
	{
		elapseRealTimeSpan_ = DateTime.Now - startRealTime_;
	}

	private void ResetGame()
	{
		timeRate_ = 1f;
		elapseSecond_ = 0f;
		startRealTime_ = DateTime.Now;
		lastRealTime_ = DateTime.Now;
		startTime_ = 0f;
		diffLastTime_ = 0.0;
		elapseRealTimeSpan_ = TimeSpan.Zero;
		isTimePause_ = false;
		SaveData.SetList("lastTime", ToList(DateTime.Now));
		SaveData.SetList("StartGameTime", ToList(startRealTime_));
	}

	public void SetGameStartTime(DateTime time)
	{
		startRealTime_ = time;
		SaveData.SetList("StartGameTime", ToList(startRealTime_));
	}

	public TimeSpan GetElapseRealTimeSpan()
	{
		return elapseRealTimeSpan_;
	}

	public int GetHoursFromGameStart()
	{
		return elapseRealTimeSpan_.Days * 24 + elapseRealTimeSpan_.Hours;
	}

	public int GetMinutesFromGameStart()
	{
		return elapseRealTimeSpan_.Minutes;
	}

	public int GetSecondsFromGameStart()
	{
		return elapseRealTimeSpan_.Seconds;
	}

	public bool IsOverNight()
	{
		return isOverNight_;
	}

	private void input()
	{
		if (Input.GetKeyDown(KeyCode.PageUp))
		{
			timeRate_ = Mathf.Min(timeRate_ + 5f, 100f);
			Time.timeScale = timeRate_;
		}
		else if (Input.GetKeyDown(KeyCode.PageDown))
		{
			timeRate_ = Mathf.Max(timeRate_ - 5f, 1f);
			Time.timeScale = timeRate_;
		}
		else if (Input.GetKeyDown(KeyCode.Home))
		{
			timeRate_ = 100f;
			Time.timeScale = timeRate_;
		}
		else if (Input.GetKeyDown(KeyCode.End))
		{
			timeRate_ = 1f;
			Time.timeScale = timeRate_;
		}
		else if (Input.GetKeyDown(KeyCode.Pause))
		{
			isTimePause_ = !isTimePause_;
			Time.timeScale = isTimePause_ ? 0f : 1f;
		}
	}
}
