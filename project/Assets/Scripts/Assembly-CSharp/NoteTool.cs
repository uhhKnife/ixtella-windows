public class NoteTool : ToyBase
{
	private NoteTool()
	{
		identify_ = "SING";
	}

	protected override void OnEndStatusFlucCallback()
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_BGM_CONTROLLER), "StartBgm");
		base.OnEndStatusFlucCallback();
	}

	protected override void OnEndAnimationCallback()
	{
		PlayerController.PlayerActionType arg = PlayerController.PlayerActionType.ACT_GOOD_FEEL;
		MessageSender.SendWithTag("Player", "PlayAction", arg);
	}
}
