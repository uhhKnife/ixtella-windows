using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusUIController : MonoBehaviour
{
	private readonly Dictionary<PlayerStatusManager.PlayerStatusType, string> STATUS_OBJECT_NAME_LIST = new Dictionary<PlayerStatusManager.PlayerStatusType, string>
	{
		{
			PlayerStatusManager.PlayerStatusType.TYPE_SATIETY,
			"ItemHunger"
		},
		{
			PlayerStatusManager.PlayerStatusType.TYPE_FEEL,
			"ItemMood"
		}
	};

	private readonly Dictionary<PlayerStatusManager.PlayerStatusType, string> STATUS_NAME_TEXT_ID_LIST = new Dictionary<PlayerStatusManager.PlayerStatusType, string>
	{
		{
			PlayerStatusManager.PlayerStatusType.TYPE_SATIETY,
			"SYS_master_parameter_satiety"
		},
		{
			PlayerStatusManager.PlayerStatusType.TYPE_FEEL,
			"SYS_master_parameter_feel"
		}
	};

	private Dictionary<PlayerStatusManager.PlayerStatusType, StatusUICommon> statusList_;

	private void Start()
	{
		RegisterStatusObjects();
		StartCoroutine(InitializeHeart());
	}

	private void SetStatus(object[] args)
	{
		if (args.Length < 2)
		{
			Debug.LogError("[" + GetType().ToString() + "] Error: received invalid parameter args " + args);
		}
		else
		{
			statusList_[(PlayerStatusManager.PlayerStatusType)args[0]].SetStatus((int)args[1]);
		}
	}

	private void RegisterStatusObjects()
	{
		statusList_ = new Dictionary<PlayerStatusManager.PlayerStatusType, StatusUICommon>();
		InitializeStatusList(PlayerStatusManager.PlayerStatusType.TYPE_SATIETY);
		InitializeStatusList(PlayerStatusManager.PlayerStatusType.TYPE_FEEL);
	}

	private void InitializeStatusList(PlayerStatusManager.PlayerStatusType statusType)
	{
		GameObject statusObject = transform.Find(STATUS_OBJECT_NAME_LIST[statusType]).gameObject;
		statusList_.Add(statusType, statusObject.AddComponent<StatusUICommon>());
		statusList_[statusType].SetStatusNameText(STATUS_NAME_TEXT_ID_LIST[statusType]);
	}

	private IEnumerator InitializeHeart()
	{
		while (!IsEnabledToInitializeHeart())
		{
			yield return null;
		}
		PlayerStatusManager.PlayerStatus playerStatus = SaveData.GetClass("PlayerStatus", new PlayerStatusManager.PlayerStatus());
		statusList_[PlayerStatusManager.PlayerStatusType.TYPE_SATIETY].SetStatus(playerStatus.satiety_);
		statusList_[PlayerStatusManager.PlayerStatusType.TYPE_FEEL].SetStatus(playerStatus.feel_);
	}

	private bool IsEnabledToInitializeHeart()
	{
		foreach (StatusUICommon value in statusList_.Values)
		{
			if (!value.IsEnabledToInitializeHeart())
			{
				return false;
			}
		}
		return true;
	}
}
