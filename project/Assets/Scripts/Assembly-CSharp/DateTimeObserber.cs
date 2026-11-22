using System;
using System.Collections.Generic;
using UnityEngine;

public class DateTimeObserber : MonoBehaviour
{
	private DateTime startRealTime_;

	private float startTime_;

	private float timeRate_ = 1f;

	private float elapseSecond_;

	private DateTime lastRealTime_;

	private double diffLastTime_;

	private bool isTimePause_;

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
		List<int> list = SaveData.GetList("StartGameTime", new List<int>
		{
			DateTime.Now.Year,
			DateTime.Now.Month,
			DateTime.Now.Day,
			DateTime.Now.Hour,
			DateTime.Now.Minute,
			DateTime.Now.Second
		});
		startRealTime_ = new DateTime(list[0], list[1], list[2], list[3], list[4], list[5]);
		calcRealTime();
		GetDiffTimeFromLastPlay();
	}

	private void Update()
	{
		if (Debug.isDebugBuild)
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
		SaveData.SetList("lastTime", new List<int>
		{
			DateTime.Now.Year,
			DateTime.Now.Month,
			DateTime.Now.Day,
			DateTime.Now.Hour,
			DateTime.Now.Minute,
			DateTime.Now.Second
		});
		SaveData.Save();
	}

	private void GetDiffTimeFromLastPlay()
	{
		List<int> list = SaveData.GetList("lastTime", new List<int>
		{
			DateTime.Now.Year,
			DateTime.Now.Month,
			DateTime.Now.Day,
			DateTime.Now.Hour,
			DateTime.Now.Minute,
			DateTime.Now.Second
		});
		lastRealTime_ = new DateTime(list[0], list[1], list[2], list[3], list[4], list[5]);
		TimeSpan timeSpan = DateTime.Now - lastRealTime_;
		if (timeSpan.Days >= 1)
		{
			timeSpan = new TimeSpan(24, 0, 0);
		}
		else
		{
			TimeSpan timeSpan2 = default(TimeSpan);
			GrowthDatabase.NightTimeInfo nightTimeInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindNightInfo("FORCE_SLEEP_TIME");
			GrowthDatabase.NightTimeInfo nightTimeInfo2 = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindNightInfo("WAKEUP_TIME");
			if (DateTime.Now.Hour < lastRealTime_.Hour)
			{
				if (lastRealTime_.Hour > nightTimeInfo.GetHour())
				{
					DateTime dateTime = new DateTime(2018, 6, 8, lastRealTime_.Hour, lastRealTime_.Minute, lastRealTime_.Second);
					DateTime dateTime2 = new DateTime(2018, 6, 9, 0, 0, 0);
					timeSpan2 = dateTime2 - dateTime;
					timeSpan -= timeSpan2;
				}
				else
				{
					DateTime dateTime3 = new DateTime(2018, 6, 8, nightTimeInfo.GetHour(), nightTimeInfo.GetMinute(), nightTimeInfo.GetSecond());
					DateTime dateTime4 = new DateTime(2018, 6, 9, 0, 0, 0);
					timeSpan2 = dateTime4 - dateTime3;
					timeSpan -= timeSpan2;
				}
				if (DateTime.Now.Hour > nightTimeInfo2.GetHour())
				{
					DateTime dateTime5 = new DateTime(2018, 6, 8, 0, 0, 0);
					DateTime dateTime6 = new DateTime(2018, 6, 8, nightTimeInfo2.GetHour(), nightTimeInfo2.GetMinute(), nightTimeInfo2.GetSecond());
					timeSpan2 = dateTime6 - dateTime5;
					timeSpan -= timeSpan2;
				}
				else
				{
					DateTime dateTime7 = new DateTime(2018, 6, 8, 0, 0, 0);
					DateTime dateTime8 = new DateTime(2018, 6, 8, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
					timeSpan2 = dateTime8 - dateTime7;
					timeSpan -= timeSpan2;
				}
				isOverNight_ = true;
			}
			else
			{
				if (lastRealTime_.Hour >= nightTimeInfo.GetHour())
				{
					timeSpan = TimeSpan.Zero;
				}
				else if (DateTime.Now.Hour < nightTimeInfo2.GetHour())
				{
					timeSpan = TimeSpan.Zero;
				}
				else
				{
					if (DateTime.Now.Hour >= nightTimeInfo.GetHour())
					{
						DateTime dateTime9 = new DateTime(2018, 6, 8, nightTimeInfo.GetHour(), nightTimeInfo.GetMinute(), nightTimeInfo.GetSecond());
						DateTime dateTime10 = new DateTime(2018, 6, 8, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
						timeSpan2 = dateTime10 - dateTime9;
						timeSpan -= timeSpan2;
					}
					if (lastRealTime_.Hour < nightTimeInfo2.GetHour())
					{
						DateTime dateTime11 = new DateTime(2018, 6, 8, lastRealTime_.Hour, lastRealTime_.Minute, lastRealTime_.Second);
						DateTime dateTime12 = new DateTime(2018, 6, 8, nightTimeInfo2.GetHour(), nightTimeInfo2.GetMinute(), nightTimeInfo2.GetSecond());
						timeSpan2 = dateTime12 - dateTime11;
						timeSpan -= timeSpan2;
					}
				}
				isOverNight_ = false;
			}
		}
		diffLastTime_ = timeSpan.TotalSeconds;
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
		SaveData.SetList("lastTime", new List<int>
		{
			DateTime.Now.Year,
			DateTime.Now.Month,
			DateTime.Now.Day,
			DateTime.Now.Hour,
			DateTime.Now.Minute,
			DateTime.Now.Second
		});
		SaveData.SetList("StartGameTime", new List<int> { startRealTime_.Year, startRealTime_.Month, startRealTime_.Day, startRealTime_.Hour, startRealTime_.Minute, startRealTime_.Second });
	}

	public void SetGameStartTime(DateTime time)
	{
		startRealTime_ = time;
		SaveData.SetList("StartGameTime", new List<int> { startRealTime_.Year, startRealTime_.Month, startRealTime_.Day, startRealTime_.Hour, startRealTime_.Minute, startRealTime_.Second });
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
			Time.timeScale = ((!isTimePause_) ? 1f : 0f);
		}
	}
}
