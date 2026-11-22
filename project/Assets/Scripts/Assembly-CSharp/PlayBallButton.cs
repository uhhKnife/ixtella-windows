public class PlayBallButton : CareCommandButtonBase
{
	protected PlayBallButton()
		: base(MenuCommonData.ButtonIconType.PLAY_BALL, "SYS_MENU_1-2_play_00")
	{
	}

	protected override void OnClick()
	{
		base.OnClick();
		MessageSender.SendWithTag("Player", "PlayAction", PlayerController.PlayerActionType.ACT_PLAY_BALL);
	}
}
