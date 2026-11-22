using System;
using Assets.SimpleAndroidNotifications;
using UnityEngine;

public class LocalPushManager : MonoBehaviour
{
	private void Start()
	{
	}

	private void RegisterLocalNotification(object[] args)
	{
		if (args.Length >= 4)
		{
			string title = (string)args[0];
			string message = (string)args[1];
			int delaySeconds = (int)args[2];
			NotificationManager.SendCustom(new NotificationParams
			{
				Id = 0,
				Delay = TimeSpan.FromSeconds(delaySeconds),
				Title = title,
				Message = message,
				Ticker = message,
				Sound = true,
				Vibrate = true,
				Light = true,
				SmallIcon = NotificationIcon.Fixl,
				SmallIconColor = new Color(0f, 0.5f, 0f),
				LargeIcon = "app_icon"
			});
		}
	}

	private void CancelLocalNotification()
	{
		NotificationManager.CancelAll();
	}
}
