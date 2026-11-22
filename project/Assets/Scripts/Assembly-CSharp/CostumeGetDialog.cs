using UnityEngine;

public class CostumeGetDialog : ContainingServantNameDialogBase
{
	private readonly string BODY_TEXT_ID = "SYS_MENU_7-1_dialog_00";

	protected override string GetBodyTextId()
	{
		return BODY_TEXT_ID;
	}

	protected override void OnLeftButtonClick()
	{
		base.OnLeftButtonClick();
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetNextPhase");
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RegisterCollectCostumeCharacter", GetCharaId());
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "ChangeCostume", GetCharaId());
		GalleryData.CostumeUnlockData costumeUnlockData = SaveData.GetClass(GalleryData.GetUnlockSaveDataKey(GetCharaId()), new GalleryData.CostumeUnlockData());
		if (costumeUnlockData.GetUnlockFlag() == GalleryData.UnlockFlag.FLAG_LOCKED)
		{
			GalleryData.CostumeUnlockData obj = new GalleryData.CostumeUnlockData(GetCharaId(), GalleryData.UnlockFlag.FLAG_UNLOCKED);
			SaveData.SetClass(GalleryData.GetUnlockSaveDataKey(GetCharaId()), obj);
			Debug.Log("[" + GetType().ToString() + "] Get costume " + GetCharaId());
		}
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "PlayAction", PlayerController.PlayerActionType.ACT_ACHIEVE);
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "DropGameFlag", GameController.GameFlag.FLAG_GET_COSTUME);
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CONTROLLER), "CloseMenu");
	}
}
