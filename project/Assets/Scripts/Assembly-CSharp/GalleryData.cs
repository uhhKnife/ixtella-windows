using System;
using UnityEngine;

public class GalleryData : MonoBehaviour
{
	public enum UnlockFlag
	{
		FLAG_LOCKED = 0,
		FLAG_NEW_UNLOCKED = 1,
		FLAG_UNLOCKED = 2
	}

	[Serializable]
	public class CostumeUnlockData
	{
		[SerializeField]
		private Define.CharaId charaId_;

		[SerializeField]
		private UnlockFlag unlockFlag_;

		public CostumeUnlockData()
		{
			clear();
		}

		public CostumeUnlockData(Define.CharaId charaId, UnlockFlag unlockFlag)
		{
			charaId_ = charaId;
			unlockFlag_ = unlockFlag;
		}

		public void clear()
		{
			charaId_ = Define.CharaId.CHID_NONE;
			unlockFlag_ = UnlockFlag.FLAG_LOCKED;
		}

		public Define.CharaId GetCharaId()
		{
			return charaId_;
		}

		public UnlockFlag GetUnlockFlag()
		{
			return unlockFlag_;
		}
	}

	private static readonly string SAVE_DATA_KEY_COMMON = "UnlockData";

	public static string GetUnlockSaveDataKey(Define.CharaId charaId)
	{
		return SAVE_DATA_KEY_COMMON + Define.GetCharaName(charaId);
	}
}
