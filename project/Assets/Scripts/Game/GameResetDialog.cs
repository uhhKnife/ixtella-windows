using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameResetDialog : MessageDialogBase
{
	private readonly string bodyTextId_ = "SYS_MENU_0-1-2_dialog_00";

	private ResourceRequest dialogPrefabResource_;

	protected override void Start()
	{
		base.Start();
		SetAllButtonsText();
		SetOnClickFunctions();
		dialogPrefabResource_ = Resources.LoadAsync(DialogCommonData.GetDialogPrefabName(DialogCommonData.DialogType.DIALOG_MESSAGE));
	}

	private void OnDestroy()
	{
		if (GetButtonSelectState() == ButtonSelectState.LEFT_SELECTED)
		{
			GameObject dialog = DialogCommonData.InstantiateDialog(dialogPrefabResource_, gameObject.scene.name);
			dialog.SetActive(true);
			dialog.AddComponent<GameResetComfirmDialog>();
			DialogCommonData.SetAnimeTriggerFlag(gameObject.scene.name, true);
			DialogCommonData.SetCanvasComponentActive(gameObject.scene.name, true);
		}
	}

	protected override void OnLeftButtonClick()
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "FinalizeGame");
		StartCoroutine(LoadDialogController());
	}

	protected override string GetBodyTextId()
	{
		return bodyTextId_;
	}

	private IEnumerator LoadDialogController()
	{
		while (!dialogPrefabResource_.isDone)
		{
			yield return null;
		}
		_003COnLeftButtonClick_003E__BaseCallProxy0();
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private void _003COnLeftButtonClick_003E__BaseCallProxy0()
	{
		base.OnLeftButtonClick();
	}
}
