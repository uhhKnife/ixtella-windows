public class CureEmotion : EmotionBase
{
	protected override void OnEndAnimationCallback()
	{
		PlayerController.PlayerActionType arg = PlayerController.PlayerActionType.ACT_HAPPINESS;
		MessageSender.SendWithTag("Player", "PlayAction", arg);
	}
}
