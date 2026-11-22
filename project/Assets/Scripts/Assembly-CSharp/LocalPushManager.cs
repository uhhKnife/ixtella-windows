using System;
using Assets.SimpleAndroidNotifications;
using UnityEngine;

public class LocalPushManager : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void RegisterLocalNotification(object[] args)
	{
		if (args.Length >= 4)
		{
			string title = (string)args[0];
			string text = (string)args[1];
			int num = (int)args[2];
			NotificationParams notificationParams = new NotificationParams();
			notificationParams.Id = 0;
			notificationParams.Delay = TimeSpan.FromSeconds(num);
			notificationParams.Title = title;
			notificationParams.Message = text;
			notificationParams.Ticker = text;
			notificationParams.Sound = true;
			notificationParams.Vibrate = true;
			notificationParams.Light = true;
			notificationParams.SmallIcon = NotificationIcon.Fixl;
			notificationParams.SmallIconColor = new Color(0f, 0.5f, 0f);
			notificationParams.LargeIcon = "app_icon";
			NotificationParams notificationParams2 = notificationParams;
			NotificationManager.SendCustom(notificationParams2);
		}
	}

	private void CancelLocalNotification()
	{
		NotificationManager.CancelAll();
	}
}
