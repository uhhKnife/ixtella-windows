public class BookTool : ToyBase
{
	private BookTool()
	{
		identify_ = "BOOK";
	}

	protected override void OnEndAnimationCallback()
	{
		PlayerController.PlayerActionType arg = PlayerController.PlayerActionType.ACT_GOOD_FEEL;
		MessageSender.SendWithTag("Player", "PlayAction", arg);
	}
}
