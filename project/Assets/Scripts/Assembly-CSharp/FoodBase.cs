using System;
using UnityEngine;

public class FoodBase : OneshotObjectBase
{
	protected bool isSuccess_ = true;

	private void SetSeccess(bool flag)
	{
		isSuccess_ = flag;
	}

	protected override void OnEndStatusFlucCallback()
	{
		if (SingletonMonoBehaviour<GrowthDatabase>.Instance == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(identify_))
		{
			Debug.LogError("[OnEndStatusFlucCallback] invalid identify. id -> " + identify_);
			return;
		}
		GrowthDatabase.CommandInfo commandInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindCommandInfo(identify_);
		foreach (PlayerStatusManager.PlayerStatusType value in Enum.GetValues(typeof(PlayerStatusManager.PlayerStatusType)))
		{
			switch (value)
			{
			case PlayerStatusManager.PlayerStatusType.TYPE_WEIGHT:
			{
				float weightValue = commandInfo.GetWeightValue();
				if (!weightValue.Equals(0f))
				{
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "AddPlayerStatus", value, weightValue);
				}
				break;
			}
			case PlayerStatusManager.PlayerStatusType.TYPE_SATIETY:
			{
				int satietyValue = commandInfo.GetSatietyValue();
				if (satietyValue != 0)
				{
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "AddPlayerStatus", value, satietyValue);
				}
				break;
			}
			case PlayerStatusManager.PlayerStatusType.TYPE_FEEL:
			{
				int feelValue = commandInfo.GetFeelValue();
				if (feelValue != 0)
				{
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "AddPlayerStatus", value, feelValue);
				}
				break;
			}
			case PlayerStatusManager.PlayerStatusType.TYPE_STRENGTH:
			{
				float strengthValue = commandInfo.GetStrengthValue();
				if (!strengthValue.Equals(0f))
				{
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "AddPlayerStatus", value, strengthValue);
				}
				break;
			}
			case PlayerStatusManager.PlayerStatusType.TYPE_INTELLIGENCE:
			{
				float intelligenceValue = commandInfo.GetIntelligenceValue();
				if (!intelligenceValue.Equals(0f))
				{
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "AddPlayerStatus", value, intelligenceValue);
				}
				break;
			}
			case PlayerStatusManager.PlayerStatusType.TYPE_MAGIC:
			{
				float magicValue = commandInfo.GetMagicValue();
				if (!magicValue.Equals(0f))
				{
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "AddPlayerStatus", value, magicValue);
				}
				break;
			}
			}
		}
	}
}
