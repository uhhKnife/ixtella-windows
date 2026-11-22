public class TitleButton : SceneTransitionButtonController
{
	private readonly string BUTTON_TEXT_ID = "SYS_MENU_1_button_title";

	protected override void Start()
	{
		base.Start();
		SetButtonMainText(BUTTON_TEXT_ID);
	}
}
