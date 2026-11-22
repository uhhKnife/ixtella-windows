public class BallTool : ToyBase
{
	private BallTool()
	{
		identify_ = "BALL";
	}

	protected override void OnEndAnimationCallback()
	{
		PlayerController.PlayerActionType arg = PlayerController.PlayerActionType.ACT_GOOD_FEEL;
		MessageSender.SendWithTag("Player", "PlayAction", arg);
	}
}
