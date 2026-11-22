using System.Collections;
using UnityEngine;

public abstract class OpenDialogButtonBase : ButtonBase
{
	private DialogCommonData.DialogType dialogType_;

	protected override void Start()
	{
		base.Start();
		SetupOnClickFunction();
	}

	public void OpenDialog()
	{
		if (dialogType_ == DialogCommonData.DialogType.DIALOG_NONE)
		{
			Debug.LogWarning("[" + GetType().ToString() + "] Warning: at OpenDialog(), dialogType_ is " + dialogType_);
		}
		else
		{
			StartCoroutine(ControlOpenDialog());
		}
	}

	private IEnumerator ControlOpenDialog()
	{
		DialogCommonData.SetCanvasComponentActive(base.gameObject.scene.name, true);
		ResourceRequest prefabAsync = Resources.LoadAsync(DialogCommonData.GetDialogPrefabName(dialogType_));
		while (!prefabAsync.isDone)
		{
			yield return null;
		}
		GameObject dialogObject = DialogCommonData.InstantiateDialog(prefabAsync, base.gameObject.scene.name);
		dialogObject.SetActive(true);
		AddScriptToDialog(ref dialogObject);
		DialogCommonData.SetAnimeTriggerFlag(base.gameObject.scene.name, true);
	}

	protected virtual void AddScriptToDialog(ref GameObject dialogObject)
	{
	}

	protected virtual void OnClickException()
	{
	}

	protected virtual bool ShouldOpenDialog()
	{
		return true;
	}

	protected void SetDialogType(DialogCommonData.DialogType dialogType)
	{
		dialogType_ = dialogType;
	}

	protected override void OnClick()
	{
		base.OnClick();
		if (ShouldOpenDialog())
		{
			OpenDialog();
		}
		else
		{
			OnClickException();
		}
	}
}
