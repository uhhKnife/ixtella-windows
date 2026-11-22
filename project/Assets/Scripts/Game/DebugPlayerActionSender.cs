using System;
using UnityEngine;

public class DebugPlayerActionSender : MonoBehaviour
{
	private void Start()
	{
	}

	public void PlayAnimation(string anim)
	{
		PlayerController.PlayerAnimationType arg = (PlayerController.PlayerAnimationType)Enum.Parse(typeof(PlayerController.PlayerAnimationType), anim);
		MessageSender.SendWithTag("Player", "PlayAnimation", arg);
	}

	public void PlayeEmotion(string emote)
	{
		PlayerController.EmoteIconType arg = (PlayerController.EmoteIconType)Enum.Parse(typeof(PlayerController.EmoteIconType), emote);
		MessageSender.SendWithTag("Player", "PlayEmotion", arg);
	}

	public void PlayItem(string item)
	{
		PlayerController.ItemIconType arg = (PlayerController.ItemIconType)Enum.Parse(typeof(PlayerController.ItemIconType), item);
		MessageSender.SendWithTag("Player", "PlayItem", arg);
	}

	public void PlayAction(string action)
	{
		PlayerController.PlayerActionType arg = (PlayerController.PlayerActionType)Enum.Parse(typeof(PlayerController.PlayerActionType), action);
		MessageSender.SendWithTag("Player", "DebugPlayAction", arg);
	}

	public void AddTimeRate(float diff)
	{
		float timeScale = Time.timeScale;
		timeScale = Mathf.Clamp(timeScale + diff, 1f, 100f);
		Time.timeScale = timeScale;
	}

	public void MaxTimeRate()
	{
		Time.timeScale = 100f;
	}

	public void MinTimeRate()
	{
		Time.timeScale = 1f;
	}

	public void ForwadTimeSec(float sec)
	{
		MessageSender.Send(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "ForwardTimer", sec);
	}

	public void OverWriteDayState(string s)
	{
		EventScheduler.DaysState arg = (EventScheduler.DaysState)Enum.Parse(typeof(EventScheduler.DaysState), s);
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "DebugOverWriteDayState", arg);
	}

	public void EndOverWriteDayState()
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "EndOverWriteDayState");
	}

	public void AddStatus(string status)
	{
		PlayerStatusManager.PlayerStatusType type = (PlayerStatusManager.PlayerStatusType)Enum.Parse(typeof(PlayerStatusManager.PlayerStatusType), status);
		MessageSender.SendWithTag("Player", "AddPlayerStatus", type, 1f);
	}

	public void SubStatus(string status)
	{
		PlayerStatusManager.PlayerStatusType type = (PlayerStatusManager.PlayerStatusType)Enum.Parse(typeof(PlayerStatusManager.PlayerStatusType), status);
		MessageSender.SendWithTag("Player", "AddPlayerStatus", type, -1f);
	}

	public void ChangeCostume(string cosId)
	{
		Define.CharaId arg = (Define.CharaId)Enum.Parse(typeof(Define.CharaId), cosId);
		MessageSender.SendWithTag("Player", "ChangeCostume", arg);
	}

	public void SetCharacterBackground(bool isMale)
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_CHARACTER_BACKGROUND), "ChangeBackground", (!isMale) ? PlayerStatusManager.Gender.GENDER_FEMALE : PlayerStatusManager.Gender.GENDER_MALE);
	}

	public void LockAllCostume()
	{
		for (int i = 0; i < 12; i++)
		{
			Define.CharaId charaId = (Define.CharaId)i;
			if (!Define.IsMasterId(charaId))
			{
				GalleryData.CostumeUnlockData data = new GalleryData.CostumeUnlockData(charaId, GalleryData.UnlockFlag.FLAG_LOCKED);
				SaveData.SetClass(GalleryData.GetUnlockSaveDataKey(charaId), data);
			}
		}
	}

	public void UnlockAllCostume()
	{
		for (int i = 0; i < 12; i++)
		{
			Define.CharaId charaId = (Define.CharaId)i;
			if (!Define.IsMasterId(charaId))
			{
				GalleryData.CostumeUnlockData data = new GalleryData.CostumeUnlockData(charaId, GalleryData.UnlockFlag.FLAG_UNLOCKED);
				SaveData.SetClass(GalleryData.GetUnlockSaveDataKey(charaId), data);
				Debug.Log("[" + GetType().ToString() + "] Unlock costume " + charaId);
			}
		}
	}

	public void RaiseCookableFlag()
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetCookable", true);
	}

	public void GameReset()
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "FinalizeGame");
	}

	public void ClearLog()
	{
		DebugMenu debugMenu = UnityEngine.Object.FindAnyObjectByType<DebugMenu>();
		if (debugMenu != null)
		{
			debugMenu.SendMessage("ClearResumeLog");
		}
	}
}
