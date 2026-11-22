public class CommonTextDialog : MessageDialogBase
{
	private string bodyTextId_;

	protected override void Start()
	{
		base.Start();
		SetButtonVisible(ButtonKind.BUTTON_LEFT, false);
		SetButtonVisible(ButtonKind.BUTTON_RIGHT, false);
	}

	protected override int GetTotalPageNum()
	{
		return bodyTextId_.Length;
	}

	protected override string GetBodyTextId()
	{
		return bodyTextId_;
	}

	public void SetBodyTextIdList(string bodyTextId)
	{
		bodyTextId_ = bodyTextId;
	}
}
