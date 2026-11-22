public class MilkFood : FoodBase
{
	private MilkFood()
	{
		identify_ = "MILK";
	}

	protected override void OnEndAnimationCallback()
	{
		MessageSender.SendWithTag("Player", "PlayAction", PlayerController.PlayerActionType.ACT_HAPPINESS);
	}
}
