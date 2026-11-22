public class SpecialFood : FoodBase
{
	private SpecialFood()
	{
		identify_ = "SPECIAL_NERO";
	}

	protected override void OnEndAnimationCallback()
	{
		PlayerController.PlayerActionType arg = (isSuccess_ ? PlayerController.PlayerActionType.ACT_HAPPINESS : PlayerController.PlayerActionType.ACT_COMPLAIN_1SHOT);
		MessageSender.SendWithTag("Player", "PlayAction", arg);
	}
}
