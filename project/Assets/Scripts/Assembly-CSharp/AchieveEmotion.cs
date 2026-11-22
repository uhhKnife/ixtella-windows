public class AchieveEmotion : EmotionBase
{
	protected override void OnEndStatusFlucCallback()
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "SetAllButtonInteractive", true);
	}

	protected override void OnEndAnimationCallback()
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_CHARACTER_BACKGROUND), "DestroyCostumeGetBackground");
		if (SingletonMonoBehaviour<GameController>.Instance.IsGameClear())
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "GameClearJoy");
		}
		else
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "ResetPosition");
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "PlayAction", PlayerController.PlayerActionType.ACT_DEFAULT);
		}
		base.OnEndAnimationCallback();
	}
}
