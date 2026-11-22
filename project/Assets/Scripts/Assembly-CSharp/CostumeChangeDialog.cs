public class CostumeChangeDialog : ContainingServantNameDialogBase
{
	private readonly string BODY_SUCCESS_TEXT_ID = "SYS_MENU_4_dialog_00";

	private readonly string BODY_FAILUAR_TEXT_ID = "SYS_MENU_4_dialog_01";

	private readonly string DEFAULT_COSTUME_TEXT_ID = "SYS_MENU_4_default_costume";

	private bool canChangeCostume_;

	protected override void Start()
	{
		PlayerStatusManager.Gender gender = ((!Define.IsMaleCharacter(GetCharaId())) ? PlayerStatusManager.Gender.GENDER_FEMALE : PlayerStatusManager.Gender.GENDER_MALE);
		canChangeCostume_ = gender == SingletonMonoBehaviour<GameController>.Instance.GetPlayerGender();
		base.Start();
		if (!canChangeCostume_)
		{
			SetButtonVisible(ButtonKind.BUTTON_RIGHT, false);
		}
	}

	protected override string GetBodyTextId()
	{
		return (!canChangeCostume_) ? BODY_FAILUAR_TEXT_ID : BODY_SUCCESS_TEXT_ID;
	}

	protected override void SetAllButtonsText()
	{
		if (canChangeCostume_)
		{
			base.SetAllButtonsText();
		}
		else
		{
			SetButtonText(ButtonKind.BUTTON_LEFT, GetOkTextId());
		}
	}

	protected override void OnLeftButtonClick()
	{
		base.OnLeftButtonClick();
		if (canChangeCostume_)
		{
			MessageSender.SendWithTag("Player", "ChangeCostume", GetCharaId());
			MessageSender.SendWithObject(MenuCommonData.GetMenuScreenObject(), "SetFrame", GetCharaId());
		}
		else
		{
			CloseDialog();
		}
	}

	protected override string GetServantNameReplaceText()
	{
		if (Define.IsMasterId(GetCharaId()))
		{
			return SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(DEFAULT_COSTUME_TEXT_ID);
		}
		return base.GetServantNameReplaceText();
	}
}
