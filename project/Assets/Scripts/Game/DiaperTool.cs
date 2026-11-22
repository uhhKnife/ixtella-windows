public class DiaperTool : ToolBase
{
	protected override void OnEndAnimationCallback()
	{
		PlayerController.PlayerActionType arg = PlayerController.PlayerActionType.ACT_HAPPINESS;
		MessageSender.SendWithTag("Player", "PlayAction", arg);
	}
}
