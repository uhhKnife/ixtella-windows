public abstract class ContainingServantNameDialogBase : MessageDialogBase
{
	private readonly string SERVANT_NAME_TAG = "[servant_name]";

	private Define.CharaId charaId_;

	protected override void Start()
	{
		base.Start();
		SetAllButtonsText();
		SetOnClickFunctions();
		ReplaceBodyTextTag(SERVANT_NAME_TAG, GetServantNameReplaceText());
		SetDisplaytCharaId(GetCharaId());
	}

	public void SetCharaId(Define.CharaId charaId)
	{
		charaId_ = charaId;
	}

	protected Define.CharaId GetCharaId()
	{
		return charaId_;
	}

	protected virtual string GetServantNameReplaceText()
	{
		return SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetServantNameText(GetCharaId());
	}

	protected override bool IsCharaImageActive()
	{
		return true;
	}
}
