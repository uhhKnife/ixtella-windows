using System;
using System.Linq;
using UnityEngine;

namespace Assets.SimpleAndroidNotifications
{
	// Plugin calls are Android-only; elsewhere this just tracks ids.
	public static class NotificationManager
	{
		public const string FullClassName = "com.hippogames.simpleandroidnotifications.Controller";

		public const string MainActivityClassName = "com.unity3d.player.UnityPlayerActivity";

		public static int Send(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = NotificationIcon.Bell, bool silent = false)
		{
			NotificationParams notificationParams = new NotificationParams();
			notificationParams.Id = NotificationIdHandler.GetNotificationId();
			notificationParams.Delay = delay;
			notificationParams.Title = title;
			notificationParams.Message = message;
			notificationParams.Ticker = message;
			notificationParams.Sound = !silent;
			notificationParams.Vibrate = !silent;
			notificationParams.Light = true;
			notificationParams.SmallIcon = smallIcon;
			notificationParams.SmallIconColor = smallIconColor;
			notificationParams.LargeIcon = string.Empty;
			notificationParams.ExecuteMode = NotificationExecuteMode.Inexact;
			return SendCustom(notificationParams);
		}

		public static int SendWithAppIcon(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = NotificationIcon.Bell, bool silent = false)
		{
			NotificationParams notificationParams = new NotificationParams();
			notificationParams.Id = NotificationIdHandler.GetNotificationId();
			notificationParams.Delay = delay;
			notificationParams.Title = title;
			notificationParams.Message = message;
			notificationParams.Ticker = message;
			notificationParams.Sound = !silent;
			notificationParams.Vibrate = !silent;
			notificationParams.Light = true;
			notificationParams.SmallIcon = smallIcon;
			notificationParams.SmallIconColor = smallIconColor;
			notificationParams.LargeIcon = "app_icon";
			notificationParams.ExecuteMode = NotificationExecuteMode.Inexact;
			return SendCustom(notificationParams);
		}

		public static int SendCustom(NotificationParams notificationParams)
		{
			long delayMs = (long)notificationParams.Delay.TotalMilliseconds;
			long repeatMs = (notificationParams.Repeat ? ((long)notificationParams.RepeatInterval.TotalMilliseconds) : 0);
#if UNITY_ANDROID && !UNITY_EDITOR
			string vibration = string.Join(",", notificationParams.Vibration.Select((int i) => i.ToString()).ToArray());
			new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller").CallStatic("SetNotification", notificationParams.Id, notificationParams.GroupName ?? string.Empty, notificationParams.GroupSummary ?? string.Empty, notificationParams.ChannelId, notificationParams.ChannelName, delayMs, Convert.ToInt32(notificationParams.Repeat), repeatMs, notificationParams.Title, notificationParams.Message, notificationParams.Ticker, Convert.ToInt32(notificationParams.Multiline), Convert.ToInt32(notificationParams.Sound), notificationParams.CustomSound ?? string.Empty, Convert.ToInt32(notificationParams.Vibrate), vibration, Convert.ToInt32(notificationParams.Light), notificationParams.LightOnMs, notificationParams.LightOffMs, ColotToInt(notificationParams.LightColor), notificationParams.LargeIcon ?? string.Empty, GetSmallIconName(notificationParams.SmallIcon), ColotToInt(notificationParams.SmallIconColor), (int)notificationParams.ExecuteMode, notificationParams.CallbackData, "com.unity3d.player.UnityPlayerActivity");
#endif
			NotificationIdHandler.AddScheduledNotificaion(notificationParams.Id);
			return notificationParams.Id;
		}

		public static void Cancel(int id)
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller").CallStatic("CancelNotification", id);
#endif
			NotificationIdHandler.RemoveScheduledNotificaion(id);
		}

		public static void CancelAll()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller").CallStatic("CancelAllNotifications");
#endif
			NotificationIdHandler.RemoveAllScheduledNotificaions();
		}

		public static void CancelAllDisplayed()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller").CallStatic("CancelAllDisplayedNotifications");
#endif
			NotificationIdHandler.RemoveAllScheduledNotificaions();
		}

		public static NotificationCallback GetNotificationCallback()
		{
#if !UNITY_ANDROID || UNITY_EDITOR
			return null;
#else
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject intent = activity.Call<AndroidJavaObject>("getIntent", new object[0]);
			if (intent.Call<bool>("hasExtra", new object[1] { "Notification.Id" }))
			{
				AndroidJavaObject extras = intent.Call<AndroidJavaObject>("getExtras", new object[0]);
				NotificationCallback notificationCallback = new NotificationCallback();
				notificationCallback.Id = extras.Call<int>("getInt", new object[1] { "Notification.Id" });
				notificationCallback.Data = extras.Call<string>("getString", new object[1] { "Notification.CallbackData" });
				return notificationCallback;
			}
			return null;
#endif
		}

		private static int ColotToInt(Color color)
		{
			Color32 c = color;
			return c.r * 65536 + c.g * 256 + c.b;
		}

		private static string GetSmallIconName(NotificationIcon icon)
		{
			return "anp_" + icon.ToString().ToLower();
		}
	}
}
