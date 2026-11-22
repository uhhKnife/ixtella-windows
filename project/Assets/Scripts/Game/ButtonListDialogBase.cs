public abstract class ButtonListDialogBase : ButtonDialogBase
{
	private readonly int PAGE_MAX_BUTTON_NUM = 3;

	protected override int GetPageMaxButtonNum()
	{
		return PAGE_MAX_BUTTON_NUM;
	}
}
