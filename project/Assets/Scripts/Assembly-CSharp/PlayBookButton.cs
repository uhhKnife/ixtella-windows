public class PlayBookButton : CareCommandButtonBase
{
	protected PlayBookButton()
		: base(MenuCommonData.ButtonIconType.PLAY_BOOK, "SYS_MENU_1-2_play_01")
	{
	}

	protected override void OnClick()
	{
		base.OnClick();
		MessageSender.SendWithTag("Player", "PlayAction", PlayerController.PlayerActionType.ACT_READ_BOOK);
	}
}
