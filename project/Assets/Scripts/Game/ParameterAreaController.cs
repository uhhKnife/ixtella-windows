using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParameterAreaController : MonoBehaviour
{
	public enum ParameterType
	{
		NONE = -1,
		WEIGHT = 0,
		TIME = 1,
		STRENGTH = 2,
		INTELLIGENCE = 3,
		MAGIC = 4,
		MAX = 5
	}

	private readonly Dictionary<ParameterType, string> PARAMTER_LABEL_TEXT_LIST = new Dictionary<ParameterType, string>
	{
		{
			ParameterType.WEIGHT,
			"SYS_master_parameter_weight"
		},
		{
			ParameterType.TIME,
			"SYS_master_parameter_time"
		},
		{
			ParameterType.STRENGTH,
			"SYS_master_parameter_strength"
		},
		{
			ParameterType.INTELLIGENCE,
			"SYS_master_parameter_intelligence"
		},
		{
			ParameterType.MAGIC,
			"SYS_master_parameter_magic"
		}
	};

	private readonly string CHARA_TEXTURE_OBJECT_NAME = "AreaChara/ImgChara";

	private readonly string PARAMETER_UPPER_OBJECT_NAME_COMMON = "AreaInfo/AreaInfoUpper/ItemInfo_";

	private readonly string PARAMETER_LOWER_OBJECT_NAME_COMMON = "AreaInfo/AreaInfoLower/ItemInfo_";

	private readonly string PARAMETER_LABEL_OBJECT_NAME = "AreaParam/BgParam/LabelParam";

	private readonly string PARAMETER_VALUE_OBJECT_NAME = "AreaLabelNum/LabelNum";

	private readonly string KILO_LABEL_OBJECT_NAME = "AreaLabelNum/LabelKg";

	private readonly string WEIGHT_NAME = "Weight";

	private readonly string COLON_TEXT_ID = "SYS_common_colon";

	private readonly string WEIGHT_UNIT_TEXT_ID = "SYS_master_parameter_weight_unit";

	private Dictionary<ParameterType, GameObject> parameterObjectList_;

	private DateTimeObserber timeObserber_;

	private PlayerStatusManager.PlayerStatus playerStatus_;

	private int second_;

	private string colonText_;

	protected void Start()
	{
		colonText_ = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(COLON_TEXT_ID);
		SetMasterTexture(SingletonMonoBehaviour<PlayerStatusManager>.Instance.GetCurrentCharaId());
		for (int i = 0; i < 5; i++)
		{
			ParameterType parameterType = (ParameterType)i;
			SetParameterLabelText(parameterType);
			SetParameterValue(parameterType);
		}
		SetWeightUnitText();
	}

	protected void Update()
	{
		if (second_ != timeObserber_.GetSecondsFromGameStart())
		{
			SetParameterValue(ParameterType.TIME);
			second_ = timeObserber_.GetSecondsFromGameStart();
		}
	}

	protected void Awake()
	{
		RegisterGameObjects();
	}

	protected void RegisterGameObjects()
	{
		timeObserber_ = Object.FindAnyObjectByType<DateTimeObserber>();
		RegisterParameterObjects();
	}

	private void RegisterParameterObjects()
	{
		parameterObjectList_ = new Dictionary<ParameterType, GameObject>();
		for (int i = 0; i < 5; i++)
		{
			string objectName = string.Empty;
			ParameterType parameterType = (ParameterType)i;
			switch (parameterType)
			{
			case ParameterType.WEIGHT:
			case ParameterType.TIME:
				objectName = PARAMETER_UPPER_OBJECT_NAME_COMMON + (i + 1);
				break;
			case ParameterType.STRENGTH:
			case ParameterType.INTELLIGENCE:
			case ParameterType.MAGIC:
				objectName = PARAMETER_LOWER_OBJECT_NAME_COMMON + (i + 1);
				break;
			}
			parameterObjectList_.Add(parameterType, transform.Find(objectName).gameObject);
		}
	}

	private void SetMasterTexture(Define.CharaId charaId)
	{
		GameObject texture = transform.Find(CHARA_TEXTURE_OBJECT_NAME).gameObject;
		Image image = texture.GetComponent<Image>();
		image.sprite = PortSprites.Load(MenuCommonData.GetCharaSpriteFilename(charaId));
	}

	private void SetParameterLabelText(ParameterType type)
	{
		GameObject label = parameterObjectList_[type].transform.Find(PARAMETER_LABEL_OBJECT_NAME).gameObject;
		label.GetComponent<Text>().text = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(PARAMTER_LABEL_TEXT_LIST[type]);
	}

	private void SetWeightUnitText()
	{
		GameObject label = parameterObjectList_[ParameterType.WEIGHT].transform.Find(KILO_LABEL_OBJECT_NAME).gameObject;
		label.GetComponent<Text>().text = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(WEIGHT_UNIT_TEXT_ID);
	}

	public void SetParameterValue(ParameterType type)
	{
		playerStatus_ = SaveData.GetClass("PlayerStatus", new PlayerStatusManager.PlayerStatus());
		string valueText = string.Empty;
		switch (type)
		{
		case ParameterType.WEIGHT:
			valueText = playerStatus_.weight_.ToString("F1");
			break;
		case ParameterType.TIME:
			valueText = timeObserber_.GetHoursFromGameStart().ToString("00") + colonText_ + timeObserber_.GetMinutesFromGameStart().ToString("00") + colonText_ + timeObserber_.GetSecondsFromGameStart().ToString("00");
			break;
		case ParameterType.STRENGTH:
			valueText = playerStatus_.strength_.ToString();
			break;
		case ParameterType.INTELLIGENCE:
			valueText = playerStatus_.intelligence_.ToString();
			break;
		case ParameterType.MAGIC:
			valueText = playerStatus_.magic_.ToString();
			break;
		}
		SetParameterValueText(type, valueText);
	}

	private void SetParameterValueText(ParameterType type, string valueText)
	{
		string objectName = PARAMETER_VALUE_OBJECT_NAME;
		if (type == ParameterType.WEIGHT)
		{
			objectName += WEIGHT_NAME;
		}
		parameterObjectList_[type].transform.Find(objectName).GetComponent<Text>().text = valueText;
	}
}
