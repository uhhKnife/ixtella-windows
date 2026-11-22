using System.Collections.Generic;

public class MasterSelectDialog : MessageDialogBase
{
	private readonly string BODY_TEXT_ID = "SYS_MENU_0-2_dialog_00";

	private readonly string GENDER_TAG = "[gender]";

	private PlayerStatusManager.Gender gender_;

	private Dictionary<PlayerStatusManager.Gender, string> GENDER_TEXT_ID = new Dictionary<PlayerStatusManager.Gender, string>
	{
		{
			PlayerStatusManager.Gender.GENDER_MALE,
			"SYS_master_male"
		},
		{
			PlayerStatusManager.Gender.GENDER_FEMALE,
			"SYS_master_female"
		}
	};

	protected override void Start()
	{
		base.Start();
		SetAllButtonsText();
		SetOnClickFunctions();
		ReplaceBodyTextTag(GENDER_TAG, GetGenderText(gender_));
		SetDisplaytCharaId(Define.GetMasterCharaId(gender_));
	}

	protected override void Update()
	{
		base.Update();
	}

	public void SetMasterGender(PlayerStatusManager.Gender gender)
	{
		gender_ = gender;
	}

	protected override string GetBodyTextId()
	{
		return BODY_TEXT_ID;
	}

	protected override void OnLeftButtonClick()
	{
		base.OnLeftButtonClick();
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetPlayerGender", gender_);
		MessageSender.SendWithTag("SceneController", "ChangeScene", Define.SceneId.SCENE_MAIN);
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "InitializeGame");
	}

	protected override bool IsCharaImageActive()
	{
		return true;
	}

	private string GetGenderText(PlayerStatusManager.Gender gender)
	{
		return SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(GENDER_TEXT_ID[gender]);
	}
}
