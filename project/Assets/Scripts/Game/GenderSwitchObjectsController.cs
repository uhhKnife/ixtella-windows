using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenderSwitchObjectsController : MonoBehaviour
{
	public enum SwitchObjectType
	{
		GENDER_SELECT_MESSAGE = 0,
		COSTUME_GET_MESSAGE = 1,
		COSTUME_COUNT = 2,
		CURRENT_GENDER = 3
	}

	public Sprite maleSelectedSprite_;

	public Sprite femaleSelectedSprite_;

	private readonly Dictionary<SwitchObjectType, string> OBJECT_NAME_LIST = new Dictionary<SwitchObjectType, string>
	{
		{
			SwitchObjectType.GENDER_SELECT_MESSAGE,
			"AreaGenderSelect"
		},
		{
			SwitchObjectType.COSTUME_GET_MESSAGE,
			"AreaCostumeGet"
		},
		{
			SwitchObjectType.COSTUME_COUNT,
			"AreaCostume"
		},
		{
			SwitchObjectType.CURRENT_GENDER,
			"AreaGender"
		}
	};

	private readonly string HAVING_COSTUME_NUM_OBJECT_NAME = "AreaHorizontal/LabelNumerator";

	private readonly string MAX_COSTUME_NUM_OBJECT_NAME = "AreaHorizontal/LabelDenominator";

	private readonly string SHASH_OBJECT_NAME = "AreaHorizontal/Slash";

	private readonly string SLASH_TEXT_ID = "SYS_common_slash";

	private Dictionary<SwitchObjectType, GameObject> objectList_;

	private void Start()
	{
		SetSlashText();
	}

	private void Awake()
	{
		objectList_ = new Dictionary<SwitchObjectType, GameObject>();
		foreach (KeyValuePair<SwitchObjectType, string> pair in OBJECT_NAME_LIST)
		{
			objectList_.Add(pair.Key, transform.Find(OBJECT_NAME_LIST[pair.Key]).gameObject);
		}
	}

	public void SetObjectActive(SwitchObjectType objectType, bool flag)
	{
		objectList_[objectType].SetActive(flag);
	}

	public void SetGenderSelectMessage(string text)
	{
		objectList_[SwitchObjectType.GENDER_SELECT_MESSAGE].GetComponentInChildren<Text>().text = text;
	}

	public void SetCostumeGetMessage(string text)
	{
		objectList_[SwitchObjectType.COSTUME_GET_MESSAGE].GetComponentInChildren<Text>().text = text;
	}

	public void SetHavingCostumeNum(int num)
	{
		SetCostumeChildText(HAVING_COSTUME_NUM_OBJECT_NAME, num.ToString());
	}

	public void SetMaxCostumeNum(int num)
	{
		SetCostumeChildText(MAX_COSTUME_NUM_OBJECT_NAME, num.ToString());
	}

	private void SetSlashText()
	{
		SetCostumeChildText(SHASH_OBJECT_NAME, SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(SLASH_TEXT_ID));
	}

	private void SetCostumeChildText(string objectName, string text)
	{
		objectList_[SwitchObjectType.COSTUME_COUNT].transform.Find(objectName).GetComponent<Text>().text = text;
	}

	public void SetCurrentGender(PlayerStatusManager.Gender gender)
	{
		Image componentInChildren = objectList_[SwitchObjectType.CURRENT_GENDER].GetComponentInChildren<Image>();
		switch (gender)
		{
		case PlayerStatusManager.Gender.GENDER_MALE:
			componentInChildren.sprite = maleSelectedSprite_;
			break;
		case PlayerStatusManager.Gender.GENDER_FEMALE:
			componentInChildren.sprite = femaleSelectedSprite_;
			break;
		}
	}
}
