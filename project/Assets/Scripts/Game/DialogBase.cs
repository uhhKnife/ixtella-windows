using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBase : MonoBehaviour
{
	private readonly string TEXT_OBJECT_NAME = "AreaVertical/AreaLabel/LabelBox";

	private GameObject bodyTextObject_;

	private GameObject seController_;

	private GameObject dialogCanvas_;

	private Text bodyTextComponent_;

	private DetectTouchEvent touchEvent_;

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
	}

	protected virtual void Awake()
	{
		RegisterGameObjects();
	}

	protected virtual void RegisterGameObjects()
	{
		bodyTextObject_ = GetChildObject(TEXT_OBJECT_NAME);
		seController_ = GameObject.FindWithTag("SeController");
		dialogCanvas_ = DialogCommonData.GetDialogCanvas(gameObject.scene.name);
		if (bodyTextObject_ != null)
		{
			bodyTextComponent_ = bodyTextObject_.GetComponent<Text>();
		}
	}

	protected GameObject GetChildObject(string objectName)
	{
		Transform child = transform.Find(objectName);
		if (child != null)
		{
			return child.gameObject;
		}
		return null;
	}

	protected virtual void UpdateCurrentPageContents()
	{
	}

	protected void SetBodyText(string textId)
	{
		if (!string.IsNullOrEmpty(textId))
		{
			bodyTextComponent_.text = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(textId);
		}
	}

	protected void SetBodyTextDirectly(string text)
	{
		bodyTextComponent_.text = text;
	}

	protected void ReplaceBodyTextTag(string tagText, string replaceText)
	{
		bodyTextComponent_.text = bodyTextComponent_.text.Replace(tagText, replaceText);
	}

	protected void ReplaceBodyTextTag<T>(string tagText, T replaceNum)
	{
		string replaceText = replaceNum.ToString();
		ReplaceBodyTextTag(tagText, replaceText);
	}

	protected virtual void CloseDialog()
	{
		StartCoroutine(ControlCloseDialog());
	}

	private IEnumerator ControlCloseDialog()
	{
		DialogCommonData.SetAnimeTriggerFlag(gameObject.scene.name, false);
		while (DialogCommonData.IsDialogActive(gameObject.scene.name))
		{
			yield return null;
		}
		dialogCanvas_.GetComponent<Canvas>().enabled = false;
		Object.Destroy(gameObject);
	}

	private bool IsEndDialogCloseAnime(ref Animator animator, int closeAnimeNameHash, int openStateNameHash)
	{
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.fullPathHash == closeAnimeNameHash)
		{
			return false;
		}
		return currentAnimatorStateInfo.fullPathHash != openStateNameHash;
	}

	protected void PlaySelectSE()
	{
		seController_.SendMessage("PlaySE", SeController.SeType.SE_SELECT_BUTTON);
	}

	protected void MonitorOutsideTouchEvent()
	{
		if (touchEvent_ == null)
		{
			Debug.LogError("[" + GetType().ToString() + "] Error: please call InitializeTouchEventController() at Awake()");
		}
		else if (touchEvent_.IsTouchedNoColliderSpace() && !DialogCommonData.IsPlayingDialogOpenCloseAnime(gameObject.scene.name))
		{
			CloseDialog();
		}
	}

	protected void InitializeTouchEventController()
	{
		touchEvent_ = gameObject.AddComponent<DetectTouchEvent>();
	}

	protected void CheckClearState()
	{
		if (SingletonMonoBehaviour<GameController>.Instance.CheckGameFlag(GameController.GameFlag.FLAG_GET_COSTUME))
		{
			List<Define.CharaId> arg = SingletonMonoBehaviour<GrowthDatabase>.Instance.CollectGetableCostume();
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CONTROLLER), "OpenCostumeGetMenu", arg);
		}
		else if (SingletonMonoBehaviour<GameController>.Instance.IsGameClear())
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "GameClearJoy");
		}
	}
}
