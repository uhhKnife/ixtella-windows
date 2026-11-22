public class SnackFood : FoodBase
{
	private SnackFood()
	{
		identify_ = "SNACK";
	}

	protected override void OnEndAnimationCallback()
	{
		PlayerController.PlayerActionType arg = PlayerController.PlayerActionType.ACT_GOOD_FEEL;
		MessageSender.SendWithTag("Player", "PlayAction", arg);
	}
}
