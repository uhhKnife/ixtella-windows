public class PlaySongButton : CareCommandButtonBase
{
	protected PlaySongButton()
		: base(MenuCommonData.ButtonIconType.PLAY_SONG, "SYS_MENU_1-2_play_02")
	{
	}

	protected override void OnClick()
	{
		base.OnClick();
		MessageSender.SendWithTag("Player", "PlayAction", PlayerController.PlayerActionType.ACT_SING);
	}
}
