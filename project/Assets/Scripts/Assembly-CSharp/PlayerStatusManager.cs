using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStatusManager : SingletonMonoBehaviour<PlayerStatusManager>
{
	private struct ParameterInfo<T>
	{
		private T min_;

		private T max_;

		public void Set(T min, T max)
		{
			min_ = min;
			max_ = max;
		}

		public T GetMin()
		{
			return min_;
		}

		public T GetMax()
		{
			return max_;
		}
	}

	public enum PlayerStatusType
	{
		TYPE_WEIGHT = 0,
		TYPE_SATIETY = 1,
		TYPE_FEEL = 2,
		TYPE_STRENGTH = 3,
		TYPE_INTELLIGENCE = 4,
		TYPE_MAGIC = 5,
		TYPE_GENDER = 6,
		TYPE_COMPLAIN_FLAG = 7,
		TYPE_CALLING_FLAG = 8,
		TYPE_SICK_FLAG = 9,
		TYPE_DIAPER_FLAG = 10,
		STATUS_TYPE_MAX = 11
	}

	public enum PlayerConditionState
	{
		TYPE_NONE = 0,
		TYPE_SLEEPY = 1,
		TYPE_SLEEPING = 2,
		TYPE_SICK = 3,
		TYPE_DIAPER = 4
	}

	public enum Gender
	{
		GENDER_MALE = 0,
		GENDER_FEMALE = 1
	}

	[Serializable]
	public class PlayerStatus
	{
		public bool isNew_;

		public float weight_;

		public int satiety_;

		public int feel_;

		public float strength_;

		public float intelligence_;

		public float magic_;

		public Define.CharaId charaId_;

		public List<Define.CharaId> collectCostume_;

		public bool isSleepy_;

		public bool isSleeping_;

		public bool isSick_;

		public bool isDiaper_;

		public PlayerStatus()
		{
			InitializeStatus();
		}

		public void InitializeStatus()
		{
			isNew_ = true;
			weight_ = 0f;
			satiety_ = 2;
			feel_ = 2;
			strength_ = 0f;
			intelligence_ = 0f;
			magic_ = 0f;
			charaId_ = Define.CharaId.CHID_MASTER_M;
			collectCostume_ = new List<Define.CharaId>();
			collectCostume_.Clear();
			isSleepy_ = false;
			isSleeping_ = false;
			isSick_ = false;
			isDiaper_ = false;
		}

		public void SetDefaultParam()
		{
			isNew_ = false;
			GrowthDatabase.StatusInfo statusInfo = ((!SingletonMonoBehaviour<GameController>.Instance.IsMale()) ? SingletonMonoBehaviour<GrowthDatabase>.Instance.FindStatusInfo("WEIGHT_FEMALE") : SingletonMonoBehaviour<GrowthDatabase>.Instance.FindStatusInfo("WEIGHT_MALE"));
			weight_ = statusInfo.GetInitValue();
			statusInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindStatusInfo("SATIETY");
			satiety_ = (int)statusInfo.GetInitValue();
			statusInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindStatusInfo("FEEL");
			feel_ = (int)statusInfo.GetInitValue();
			statusInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindStatusInfo("STRENGTH");
			strength_ = statusInfo.GetInitValue();
			statusInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindStatusInfo("INTELLIGENCE");
			intelligence_ = statusInfo.GetInitValue();
			statusInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindStatusInfo("MAGIC");
			magic_ = statusInfo.GetInitValue();
			charaId_ = ((!SingletonMonoBehaviour<GameController>.Instance.IsMale()) ? Define.CharaId.CHID_MASTER_F : Define.CharaId.CHID_MASTER_M);
		}
	}

	private ParameterInfo<float> WEIGHT_INFO;

	private ParameterInfo<int> SATIETY_INFO;

	private ParameterInfo<int> FEEL_INFO;

	private ParameterInfo<float> STR_INFO;

	private ParameterInfo<float> INT_INFO;

	private ParameterInfo<float> MAG_INFO;

	private PlayerStatus status_ = new PlayerStatus();

	private void Initialize()
	{
		InitStatusDB();
		status_ = SaveData.GetClass("PlayerStatus", new PlayerStatus());
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "ChangeCostume", status_.charaId_);
		CheckStatus();
	}

	private void InitStatusDB()
	{
		if (SingletonMonoBehaviour<GrowthDatabase>.Instance != null)
		{
			GrowthDatabase.StatusInfo statusInfo = ((!SingletonMonoBehaviour<GameController>.Instance.IsMale()) ? SingletonMonoBehaviour<GrowthDatabase>.Instance.FindStatusInfo("WEIGHT_FEMALE") : SingletonMonoBehaviour<GrowthDatabase>.Instance.FindStatusInfo("WEIGHT_MALE"));
			WEIGHT_INFO.Set(statusInfo.GetLowerValue(), statusInfo.GetUpperValue());
			statusInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindStatusInfo("SATIETY");
			SATIETY_INFO.Set((int)statusInfo.GetLowerValue(), (int)statusInfo.GetUpperValue());
			statusInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindStatusInfo("FEEL");
			FEEL_INFO.Set((int)statusInfo.GetLowerValue(), (int)statusInfo.GetUpperValue());
			statusInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindStatusInfo("STRENGTH");
			STR_INFO.Set(statusInfo.GetLowerValue(), statusInfo.GetUpperValue());
			statusInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindStatusInfo("INTELLIGENCE");
			INT_INFO.Set(statusInfo.GetLowerValue(), statusInfo.GetUpperValue());
			statusInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindStatusInfo("MAGIC");
			MAG_INFO.Set(statusInfo.GetLowerValue(), statusInfo.GetUpperValue());
		}
	}

	private void InitializeStatus()
	{
		if (status_.isNew_)
		{
			InitStatusDB();
			status_ = SaveData.GetClass("PlayerStatus", new PlayerStatus());
			if (status_.isNew_)
			{
				status_.SetDefaultParam();
			}
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "ChangeCostume", status_.charaId_);
			CheckStatus();
		}
	}

	private void CheckStatus()
	{
		if (SingletonMonoBehaviour<GameController>.Instance.IsGameClear())
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "OnFarewell");
			return;
		}
		if (IsSleeping())
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "PlayAction", PlayerController.PlayerActionType.ACT_SLEEP);
			return;
		}
		bool flag = false;
		if (SingletonMonoBehaviour<GameController>.Instance.CanSing())
		{
			flag = true;
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "PlayAction", PlayerController.PlayerActionType.ACT_CALLING);
		}
		else if (IsDiaper())
		{
			flag = true;
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "PlayAction", PlayerController.PlayerActionType.ACT_COMPLAIN);
		}
		else if (IsSleepy())
		{
			flag = true;
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "PlayAction", PlayerController.PlayerActionType.ACT_CALLING_WITH_COMPLAIN);
		}
		else if (IsEmptySatiety() || IsEmptyFeel())
		{
			flag = true;
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "PlayAction", PlayerController.PlayerActionType.ACT_COMPLAIN);
		}
		if (IsSick())
		{
			if (flag)
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "AddSick");
			}
			else
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "PlayAction", PlayerController.PlayerActionType.ACT_SICK);
			}
		}
		else if (!flag && GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("stamdby_1"))
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "PlayAction", PlayerController.PlayerActionType.ACT_DEFAULT);
		}
	}

	private void Awake()
	{
		if (this != SingletonMonoBehaviour<PlayerStatusManager>.Instance)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		Initialize();
	}

	private void Update()
	{
		if (PortDebug.Enabled)
		{
			DebugInput();
		}
	}

	private void AddPlayerStatus(object[] args)
	{
		if (args.Length >= 2)
		{
			object obj = args[1];
			object obj2 = args[1];
			PlayerStatusType playerStatusType = (PlayerStatusType)args[0];
			switch (playerStatusType)
			{
			case PlayerStatusType.TYPE_WEIGHT:
				obj = status_.weight_;
				status_.weight_ = Mathf.Clamp(status_.weight_ + (float)args[1], WEIGHT_INFO.GetMin(), WEIGHT_INFO.GetMax());
				obj2 = status_.weight_;
				break;
			case PlayerStatusType.TYPE_SATIETY:
				obj = status_.satiety_;
				status_.satiety_ = Mathf.Clamp(status_.satiety_ + (int)args[1], SATIETY_INFO.GetMin(), SATIETY_INFO.GetMax());
				obj2 = status_.satiety_;
				break;
			case PlayerStatusType.TYPE_FEEL:
				obj = status_.feel_;
				status_.feel_ = Mathf.Clamp(status_.feel_ + (int)args[1], FEEL_INFO.GetMin(), FEEL_INFO.GetMax());
				obj2 = status_.feel_;
				break;
			case PlayerStatusType.TYPE_STRENGTH:
				obj = status_.strength_;
				status_.strength_ = Mathf.Clamp(status_.strength_ + (float)args[1], STR_INFO.GetMin(), STR_INFO.GetMax());
				obj2 = status_.strength_;
				break;
			case PlayerStatusType.TYPE_INTELLIGENCE:
				obj = status_.intelligence_;
				status_.intelligence_ = Mathf.Clamp(status_.intelligence_ + (float)args[1], INT_INFO.GetMin(), INT_INFO.GetMax());
				obj2 = status_.intelligence_;
				break;
			case PlayerStatusType.TYPE_MAGIC:
				obj = status_.magic_;
				status_.magic_ = Mathf.Clamp(status_.magic_ + (float)args[1], MAG_INFO.GetMin(), MAG_INFO.GetMax());
				obj2 = status_.magic_;
				break;
			}
			if (playerStatusType == PlayerStatusType.TYPE_SATIETY || playerStatusType == PlayerStatusType.TYPE_FEEL)
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "ResetTimer", playerStatusType);
				MessageSender.SendWithTag("StatusUI", "SetStatus", playerStatusType, obj2);
			}
			SaveData.SetClass("PlayerStatus", status_);
			Debug.Log("Registered player status data. [type : " + playerStatusType.ToString() + "]  [" + obj.ToString() + " -> " + obj2.ToString() + "]");
			UpdateParameterCheckMenu(playerStatusType);
		}
	}

	private void SetCharaId(Define.CharaId charaId)
	{
		status_.charaId_ = charaId;
		SaveData.SetClass("PlayerStatus", status_);
	}

	private void RegisterCollectCostumeCharacter(Define.CharaId charaId)
	{
		if (!status_.collectCostume_.Contains(charaId))
		{
			status_.collectCostume_.Add(charaId);
		}
	}

	public Define.CharaId GetEquipedCharaID()
	{
		return status_.charaId_;
	}

	private void RaiseCondition(object[] args)
	{
		if (status_.isNew_ || args.Length < 2)
		{
			return;
		}
		PlayerConditionState playerConditionState = (PlayerConditionState)args[0];
		bool flag = (bool)args[1];
		switch (playerConditionState)
		{
		case PlayerConditionState.TYPE_SLEEPY:
			if (!IsSleeping())
			{
				status_.isSleepy_ = flag;
				if (flag)
				{
					CheckStatus();
				}
			}
			break;
		case PlayerConditionState.TYPE_SLEEPING:
		{
			bool isSleeping_ = status_.isSleeping_;
			status_.isSleepy_ = false;
			status_.isSleeping_ = flag;
			if (flag)
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "OnSleep");
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "PlayAction", PlayerController.PlayerActionType.ACT_SLEEP);
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_CHARACTER_BACKGROUND), "ChangeSleepingColor", true);
				break;
			}
			if (isSleeping_)
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "WakeUp");
			}
			else
			{
				CheckStatus();
			}
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_CHARACTER_BACKGROUND), "ChangeSleepingColor", false);
			break;
		}
		case PlayerConditionState.TYPE_SICK:
			status_.isSick_ = flag;
			if (flag)
			{
				CheckStatus();
			}
			else
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "ResetSickTimer");
			}
			break;
		case PlayerConditionState.TYPE_DIAPER:
			status_.isDiaper_ = flag;
			if (flag)
			{
				CheckStatus();
			}
			else
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "ResetSickTimer");
			}
			break;
		}
		SaveData.SetClass("PlayerStatus", status_);
	}

	public bool IsStatusMax(PlayerStatusType type)
	{
		switch (type)
		{
		case PlayerStatusType.TYPE_WEIGHT:
			return status_.weight_ >= WEIGHT_INFO.GetMax();
		case PlayerStatusType.TYPE_SATIETY:
			return status_.satiety_ >= SATIETY_INFO.GetMax();
		case PlayerStatusType.TYPE_FEEL:
			return status_.feel_ >= FEEL_INFO.GetMax();
		case PlayerStatusType.TYPE_STRENGTH:
			return status_.strength_ >= STR_INFO.GetMax();
		case PlayerStatusType.TYPE_INTELLIGENCE:
			return status_.intelligence_ >= INT_INFO.GetMax();
		case PlayerStatusType.TYPE_MAGIC:
			return status_.magic_ >= MAG_INFO.GetMax();
		default:
			return false;
		}
	}

	private void ResetGame()
	{
		status_.InitializeStatus();
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "ChangeCostume", Define.GetMasterCharaId(SingletonMonoBehaviour<GameController>.Instance.GetPlayerGender()));
		SaveData.SetClass("PlayerStatus", status_);
	}

	public bool IsSleepy()
	{
		return status_.isSleepy_;
	}

	public bool IsSleeping()
	{
		return status_.isSleeping_;
	}

	public bool IsSick()
	{
		return status_.isSick_;
	}

	public bool IsDiaper()
	{
		return status_.isDiaper_;
	}

	public bool IsEmptySatiety()
	{
		return status_.satiety_ <= 0;
	}

	public bool IsEmptyFeel()
	{
		return status_.feel_ <= 0;
	}

	public int GetSatiety()
	{
		return status_.satiety_;
	}

	public int GetFeel()
	{
		return status_.feel_;
	}

	public Define.CharaId GetCurrentCharaId()
	{
		return status_.charaId_;
	}

	private void DebugInput()
	{
		if (Input.GetKey(KeyCode.F5))
		{
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				MessageSender.Send("Player", "AddPlayerStatus", PlayerStatusType.TYPE_SATIETY, 1);
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				MessageSender.Send("Player", "AddPlayerStatus", PlayerStatusType.TYPE_SATIETY, -1);
			}
		}
		if (Input.GetKey(KeyCode.F6))
		{
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				MessageSender.Send("Player", "AddPlayerStatus", PlayerStatusType.TYPE_FEEL, 1);
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				MessageSender.Send("Player", "AddPlayerStatus", PlayerStatusType.TYPE_FEEL, -1);
			}
		}
		if (Input.GetKey(KeyCode.F9))
		{
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				MessageSender.Send("Player", "AddPlayerStatus", PlayerStatusType.TYPE_STRENGTH, 1f);
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				MessageSender.Send("Player", "AddPlayerStatus", PlayerStatusType.TYPE_STRENGTH, -1f);
			}
		}
		if (Input.GetKey(KeyCode.F10))
		{
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				MessageSender.Send("Player", "AddPlayerStatus", PlayerStatusType.TYPE_INTELLIGENCE, 1f);
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				MessageSender.Send("Player", "AddPlayerStatus", PlayerStatusType.TYPE_INTELLIGENCE, -1f);
			}
		}
		if (Input.GetKey(KeyCode.F11))
		{
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				MessageSender.Send("Player", "AddPlayerStatus", PlayerStatusType.TYPE_MAGIC, 1f);
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				MessageSender.Send("Player", "AddPlayerStatus", PlayerStatusType.TYPE_MAGIC, -1f);
			}
		}
	}

	private void UpdateParameterCheckMenu(PlayerStatusType type)
	{
		if (!SingletonMonoBehaviour<MenuController>.Instance || SingletonMonoBehaviour<MenuController>.Instance.GetCurrentMenuType() != MenuCommonData.MenuType.MENU_PARAMETER_CHECK)
		{
			return;
		}
		GameObject gameObject = GameObject.FindWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CANVAS));
		if (gameObject.transform.childCount > 0)
		{
			GameObject gameObject2 = gameObject.transform.GetChild(0).gameObject;
			if (!(gameObject2.GetComponent<ParameterCheckMenu>() == null))
			{
				MessageSender.SendWithObject(gameObject2, "SetParameterValue", type);
			}
		}
	}
}
