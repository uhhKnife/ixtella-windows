public abstract class CharaSelectDialogBase : ButtonDialogBase
{
	private readonly int PAGE_MAX_BUTTON_NUM = 6;

	protected override int GetPageMaxButtonNum()
	{
		return PAGE_MAX_BUTTON_NUM;
	}
}
