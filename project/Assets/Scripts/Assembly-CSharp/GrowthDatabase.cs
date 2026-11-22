using System;
using System.Collections.Generic;
using UnityEngine;

public class GrowthDatabase : SingletonMonoBehaviour<GrowthDatabase>
{
	public enum EvolParamType
	{
		EVOL_PARAM_STR = 0,
		EVOL_PARAM_INT = 1,
		EVOL_PARAM_MAGIC = 2,
		EVOL_PARAM_MAX = 3
	}

	[Serializable]
	public class DataInfoBase
	{
		[SerializeField]
		protected string key_;

		protected DataInfoBase()
		{
			key_ = string.Empty;
		}

		public bool IsMatch(string key)
		{
			return key_.GetHashCode() == key.GetHashCode();
		}
	}

	[Serializable]
	public class EvolutionInfo : DataInfoBase
	{
		[SerializeField]
		private int str_;

		[SerializeField]
		private int int_;

		[SerializeField]
		private int magic_;

		[SerializeField]
		private bool isMaleMasterWearable_;

		[SerializeField]
		private bool isFemaleMasterWearable_;

		[SerializeField]
		private string costumeId_;

		[SerializeField]
		private int sortIndex_;

		public EvolutionInfo()
		{
			str_ = 0;
			int_ = 0;
			magic_ = 0;
			isMaleMasterWearable_ = false;
			isFemaleMasterWearable_ = false;
			costumeId_ = "DUMMY_COSTUME";
			sortIndex_ = 0;
		}

		public string GetCharaId()
		{
			return key_;
		}

		public int GetStrength()
		{
			return str_;
		}

		public int GetIntelligence()
		{
			return int_;
		}

		public int GetMagic()
		{
			return magic_;
		}

		public bool CanWearableMale()
		{
			return isMaleMasterWearable_;
		}

		public bool CanWearableFeMale()
		{
			return isFemaleMasterWearable_;
		}

		public string GetCostumeId()
		{
			return costumeId_;
		}

		public int GetSortIndex()
		{
			return sortIndex_;
		}

		public bool isClear(float s, float i, float m)
		{
			return (str_.Equals(s) || s >= (float)str_) && (int_.Equals(i) || i >= (float)int_) && (magic_.Equals(m) || m >= (float)magic_);
		}
	}

	[Serializable]
	public class StatusInfo : DataInfoBase
	{
		[SerializeField]
		private float init_;

		[SerializeField]
		private float lower_;

		[SerializeField]
		private float upper_;

		public StatusInfo()
		{
			init_ = 0f;
			lower_ = 0f;
			upper_ = 0f;
		}

		public float GetInitValue()
		{
			return init_;
		}

		public float GetLowerValue()
		{
			return lower_;
		}

		public float GetUpperValue()
		{
			return upper_;
		}
	}

	[Serializable]
	public class RuleInfo : DataInfoBase
	{
		[SerializeField]
		private float interval_;

		public RuleInfo()
		{
			interval_ = 0f;
		}

		public float GetInterval()
		{
			return interval_;
		}
	}

	[Serializable]
	public class NightTimeInfo : DataInfoBase
	{
		[SerializeField]
		private int hour_;

		[SerializeField]
		private int minute_;

		[SerializeField]
		private int second_;

		public NightTimeInfo()
		{
			hour_ = 0;
			minute_ = 0;
			second_ = 0;
		}

		public int GetHour()
		{
			return hour_;
		}

		public int GetMinute()
		{
			return minute_;
		}

		public int GetSecond()
		{
			return second_;
		}
	}

	[Serializable]
	public class PhaseInfo : DataInfoBase
	{
		[SerializeField]
		private float activeTime_;

		[SerializeField]
		private float napableTime_;

		[SerializeField]
		private float napTime_;

		[SerializeField]
		private int cookNum_;

		[SerializeField]
		private int cookProbability_;

		[SerializeField]
		private int singNum_;

		[SerializeField]
		private int singProbability_;

		[SerializeField]
		private int sickNum_;

		[SerializeField]
		private int sickProbability_;

		[SerializeField]
		private int diaperNum_;

		[SerializeField]
		private int diaperProbability_;

		public PhaseInfo()
		{
			activeTime_ = 0f;
			napableTime_ = 0f;
			napTime_ = 0f;
			cookNum_ = 0;
			cookProbability_ = 100;
			singNum_ = 0;
			singProbability_ = 100;
			sickNum_ = 0;
			sickProbability_ = 100;
			diaperNum_ = 0;
			diaperProbability_ = 100;
		}

		public float GetActiveTime()
		{
			return activeTime_;
		}

		public float GetNapableTime()
		{
			return napableTime_;
		}

		public float GetNapTime()
		{
			return napTime_;
		}

		public int GetCookNum()
		{
			return cookNum_;
		}

		public int GetCookProbability()
		{
			return cookProbability_;
		}

		public int GetSingNum()
		{
			return singNum_;
		}

		public int GetSingProbability()
		{
			return singProbability_;
		}

		public int GetSickNum()
		{
			return sickNum_;
		}

		public int GetSickProbability()
		{
			return sickProbability_;
		}

		public int GetDiaperNum()
		{
			return diaperNum_;
		}

		public int GetDiaperProbability()
		{
			return diaperProbability_;
		}
	}

	[Serializable]
	public class CommandInfo : DataInfoBase
	{
		[SerializeField]
		private int satiety_;

		[SerializeField]
		private int feel_;

		[SerializeField]
		private float weight_;

		[SerializeField]
		private float str_;

		[SerializeField]
		private float int_;

		[SerializeField]
		private float magic_;

		[SerializeField]
		private int probability_;

		public CommandInfo()
		{
			satiety_ = 0;
			feel_ = 0;
			weight_ = 0f;
			str_ = 0f;
			int_ = 0f;
			magic_ = 0f;
			probability_ = 100;
		}

		public int GetSatietyValue()
		{
			return satiety_;
		}

		public int GetFeelValue()
		{
			return feel_;
		}

		public float GetWeightValue()
		{
			return weight_;
		}

		public float GetStrengthValue()
		{
			return str_;
		}

		public float GetIntelligenceValue()
		{
			return int_;
		}

		public float GetMagicValue()
		{
			return magic_;
		}

		public int GetProbabilityValue()
		{
			return probability_;
		}
	}

	[Serializable]
	public class MasterAnimInfo : DataInfoBase
	{
		[SerializeField]
		private int count_;

		public MasterAnimInfo()
		{
			count_ = 0;
		}

		public int GetCount()
		{
			return count_;
		}
	}

	[Serializable]
	private class Wrapper<T>
	{
		public List<T> list_ = new List<T>();
	}

	private readonly string EVOLUTION_DATA_FILE_PATH = "ixtella_data_evolution";

	private readonly string STATUS_DATA_FILE_PATH = "ixtella_data_status";

	private readonly string RULE_DATA_FILE_PATH = "ixtella_data_rule";

	private readonly string NIGHT_TIME_DATA_FILE_PATH = "ixtella_data_night_time";

	private readonly string PHASE_DATA_FILE_PATH = "ixtella_data_phase";

	private readonly string COMMAND_DATA_FILE_PATH = "ixtella_data_command";

	private readonly string MASTER_DATA_FILE_PATH = "ixtella_data_master";

	private List<EvolutionInfo> evolutionDataList_ = new List<EvolutionInfo>();

	private List<StatusInfo> statusDataList_ = new List<StatusInfo>();

	private List<RuleInfo> ruleDataList_ = new List<RuleInfo>();

	private List<NightTimeInfo> nightDataList_ = new List<NightTimeInfo>();

	private List<PhaseInfo> phaseDataList_ = new List<PhaseInfo>();

	private List<CommandInfo> commandDataList_ = new List<CommandInfo>();

	private List<MasterAnimInfo> masterDataList_ = new List<MasterAnimInfo>();

	private void Awake()
	{
		if (this != SingletonMonoBehaviour<GrowthDatabase>.Instance)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		LoadEvolutionData();
		LoadStatusData();
		LoadRuleData();
		LoadNightData();
		LoadPhaseData();
		LoadCommandData();
		LoadMasterAnimData();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private List<T> ListFromJson<T>(string json)
	{
		string json2 = "{ \"list_\": " + json + "}";
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json2);
		return wrapper.list_;
	}

	private void LoadEvolutionData()
	{
		TextAsset textAsset = Resources.Load(EVOLUTION_DATA_FILE_PATH) as TextAsset;
		if (textAsset != null)
		{
			string text = textAsset.text;
			evolutionDataList_ = ListFromJson<EvolutionInfo>(text);
			Resources.UnloadAsset(textAsset);
		}
		else
		{
			Debug.Log("Fileが存在しません。 ->" + EVOLUTION_DATA_FILE_PATH);
		}
	}

	private void LoadStatusData()
	{
		TextAsset textAsset = Resources.Load(STATUS_DATA_FILE_PATH) as TextAsset;
		if (textAsset != null)
		{
			string text = textAsset.text;
			statusDataList_ = ListFromJson<StatusInfo>(text);
			Resources.UnloadAsset(textAsset);
		}
		else
		{
			Debug.Log("Fileが存在しません。 ->" + STATUS_DATA_FILE_PATH);
		}
	}

	private void LoadRuleData()
	{
		TextAsset textAsset = Resources.Load(RULE_DATA_FILE_PATH) as TextAsset;
		if (textAsset != null)
		{
			string text = textAsset.text;
			ruleDataList_ = ListFromJson<RuleInfo>(text);
			Resources.UnloadAsset(textAsset);
		}
		else
		{
			Debug.Log("Fileが存在しません。 ->" + RULE_DATA_FILE_PATH);
		}
	}

	private void LoadNightData()
	{
		TextAsset textAsset = Resources.Load(NIGHT_TIME_DATA_FILE_PATH) as TextAsset;
		if (textAsset != null)
		{
			string text = textAsset.text;
			nightDataList_ = ListFromJson<NightTimeInfo>(text);
			Resources.UnloadAsset(textAsset);
		}
		else
		{
			Debug.Log("Fileが存在しません。 ->" + NIGHT_TIME_DATA_FILE_PATH);
		}
	}

	private void LoadPhaseData()
	{
		TextAsset textAsset = Resources.Load(PHASE_DATA_FILE_PATH) as TextAsset;
		if (textAsset != null)
		{
			string text = textAsset.text;
			phaseDataList_ = ListFromJson<PhaseInfo>(text);
			Resources.UnloadAsset(textAsset);
		}
		else
		{
			Debug.Log("Fileが存在しません。 ->" + PHASE_DATA_FILE_PATH);
		}
	}

	private void LoadCommandData()
	{
		TextAsset textAsset = Resources.Load(COMMAND_DATA_FILE_PATH) as TextAsset;
		if (textAsset != null)
		{
			string text = textAsset.text;
			commandDataList_ = ListFromJson<CommandInfo>(text);
			Resources.UnloadAsset(textAsset);
		}
		else
		{
			Debug.Log("Fileが存在しません。 ->" + COMMAND_DATA_FILE_PATH);
		}
	}

	private void LoadMasterAnimData()
	{
		TextAsset textAsset = Resources.Load(MASTER_DATA_FILE_PATH) as TextAsset;
		if (textAsset != null)
		{
			string text = textAsset.text;
			masterDataList_ = ListFromJson<MasterAnimInfo>(text);
			Resources.UnloadAsset(textAsset);
		}
		else
		{
			Debug.Log("Fileが存在しません。 ->" + MASTER_DATA_FILE_PATH);
		}
	}

	public EvolutionInfo FindEvolInfo(string key)
	{
		foreach (EvolutionInfo item in evolutionDataList_)
		{
			if (item.IsMatch(key))
			{
				return item;
			}
		}
		Debug.LogError("Not found Evolution Info. key -> " + key);
		return new EvolutionInfo();
	}

	public StatusInfo FindStatusInfo(string key)
	{
		foreach (StatusInfo item in statusDataList_)
		{
			if (item.IsMatch(key))
			{
				return item;
			}
		}
		Debug.LogError("Not found Status Info. key -> " + key);
		return new StatusInfo();
	}

	public RuleInfo FindRuleInfo(string key)
	{
		foreach (RuleInfo item in ruleDataList_)
		{
			if (item.IsMatch(key))
			{
				return item;
			}
		}
		Debug.LogError("Not found Rule Info. key -> " + key);
		return new RuleInfo();
	}

	public NightTimeInfo FindNightInfo(string key)
	{
		foreach (NightTimeInfo item in nightDataList_)
		{
			if (item.IsMatch(key))
			{
				return item;
			}
		}
		Debug.LogError("Not found Night Info. key -> " + key);
		return new NightTimeInfo();
	}

	public PhaseInfo FindPhaseInfo(string key)
	{
		foreach (PhaseInfo item in phaseDataList_)
		{
			if (item.IsMatch(key))
			{
				return item;
			}
		}
		Debug.LogError("Not found Phase Info. key -> " + key);
		return new PhaseInfo();
	}

	public CommandInfo FindCommandInfo(string key)
	{
		foreach (CommandInfo item in commandDataList_)
		{
			if (item.IsMatch(key))
			{
				return item;
			}
		}
		Debug.LogError("Not found Command Info. key -> " + key);
		return new CommandInfo();
	}

	public MasterAnimInfo FindMasterAnimInfo(string key)
	{
		foreach (MasterAnimInfo item in masterDataList_)
		{
			if (item.IsMatch(key))
			{
				return item;
			}
		}
		Debug.LogError("Not found Master Info. key -> " + key);
		return new MasterAnimInfo();
	}

	public int GetEvolConditions(EvolParamType type, Define.CharaId charaId)
	{
		EvolutionInfo evolutionInfo = FindEvolInfo(Define.GetCharaName(charaId));
		switch (type)
		{
		case EvolParamType.EVOL_PARAM_STR:
			return evolutionInfo.GetStrength();
		case EvolParamType.EVOL_PARAM_INT:
			return evolutionInfo.GetIntelligence();
		case EvolParamType.EVOL_PARAM_MAGIC:
			return evolutionInfo.GetMagic();
		default:
			return -1;
		}
	}

	public int GetTotalCostumeNum()
	{
		return evolutionDataList_.Count;
	}

	public string GetCharaId(int index)
	{
		if (0 <= index && index < evolutionDataList_.Count)
		{
			return evolutionDataList_[index].GetCharaId();
		}
		return string.Empty;
	}

	public string GetCharaId(Define.CharaId charaId)
	{
		foreach (EvolutionInfo item in evolutionDataList_)
		{
			if (item.GetCharaId().GetHashCode() == Define.GetCharaName(charaId).GetHashCode())
			{
				return item.GetCharaId();
			}
		}
		Debug.LogError("[GrowthDatabase::GetCostumeId] charaId is not found. id -> " + charaId);
		return string.Empty;
	}

	public string GetCostumeId(Define.CharaId charaId)
	{
		if (Define.IsMasterId(charaId))
		{
			return (charaId != Define.CharaId.CHID_MASTER_M) ? "baby_f_base" : "baby_m_base";
		}
		foreach (EvolutionInfo item in evolutionDataList_)
		{
			if (item.GetCharaId().GetHashCode() == Define.GetCharaName(charaId).GetHashCode())
			{
				return item.GetCostumeId();
			}
		}
		Debug.LogError("[GrowthDatabase::GetCostumeId] charaId is not found. id -> " + charaId);
		return string.Empty;
	}

	public bool IsMasterWearableCostume(PlayerStatusManager.Gender masterGender, Define.CharaId charaId)
	{
		foreach (EvolutionInfo item in evolutionDataList_)
		{
			if (item.IsMatch(Define.GetCharaName(charaId)))
			{
				switch (masterGender)
				{
				case PlayerStatusManager.Gender.GENDER_MALE:
					return item.CanWearableMale();
				case PlayerStatusManager.Gender.GENDER_FEMALE:
					return item.CanWearableFeMale();
				}
			}
		}
		return false;
	}

	public List<Define.CharaId> CollectGetableCostume()
	{
		List<Define.CharaId> list = new List<Define.CharaId>();
		PlayerStatusManager.PlayerStatus playerStatus = SaveData.GetClass("PlayerStatus", new PlayerStatusManager.PlayerStatus());
		foreach (EvolutionInfo item in evolutionDataList_)
		{
			if (playerStatus.charaId_ == Define.GetCharaIdFromName(item.GetCharaId()) || playerStatus.collectCostume_.Contains(Define.GetCharaIdFromName(item.GetCharaId())))
			{
				continue;
			}
			if (SingletonMonoBehaviour<GameController>.Instance.IsMale())
			{
				if (!item.CanWearableMale())
				{
					continue;
				}
			}
			else if (!item.CanWearableFeMale())
			{
				continue;
			}
			if (item.isClear(playerStatus.strength_, playerStatus.intelligence_, playerStatus.magic_))
			{
				list.Add(Define.GetCharaIdFromName(item.GetCharaId()));
			}
		}
		return list;
	}

	public int GetSortIndex(Define.CharaId charaId)
	{
		foreach (EvolutionInfo item in evolutionDataList_)
		{
			if (item.IsMatch(Define.GetCharaName(charaId)))
			{
				return item.GetSortIndex();
			}
		}
		Debug.LogError("[GrowthDatabase::GetSortIndex] charaId is not found. id -> " + charaId);
		return 0;
	}

	public float GetSatietyDefuseInterval()
	{
		return GetRuleValue("INTERVAL_SATIETY");
	}

	public float GetFeelDefuseInterval()
	{
		return GetRuleValue("INTERVAL_FEEL");
	}

	public float GetSickedInterval()
	{
		return GetRuleValue("INTERVAL_SICK");
	}

	public float GetSingInterval()
	{
		return GetRuleValue("INTERVAL_SING");
	}

	public float GetSatietyGameOverLimit()
	{
		return GetRuleValue("SATIETY_GAME_OVER_LIMIT");
	}

	public float GetFeelGameOverLimit()
	{
		return GetRuleValue("FEEL_GAME_OVER_LIMIT");
	}

	public float GetSickGameOverLimit()
	{
		return GetRuleValue("SICK_GAME_OVER_LIMIT");
	}

	private float GetRuleValue(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			Debug.LogError("[GetRuleValue] invalid key. -> " + key);
			return 0f;
		}
		foreach (RuleInfo item in ruleDataList_)
		{
			if (item.IsMatch(key))
			{
				return item.GetInterval();
			}
		}
		return 0f;
	}

	public PhaseInfo FindPhaseInfo(int index)
	{
		if (index < 0 || index >= phaseDataList_.Count)
		{
			Debug.LogError("invalid index in phase info. index -> " + index);
			return new PhaseInfo();
		}
		return phaseDataList_[index];
	}

	public int GetTotalPhaseNum()
	{
		return phaseDataList_.Count;
	}

	public int GetWaveHandCount()
	{
		foreach (MasterAnimInfo item in masterDataList_)
		{
			if (item.IsMatch("WAVE_HAND_INTERVAL"))
			{
				return item.GetCount();
			}
		}
		return 5;
	}
}
