using System.Collections.Generic;
using UnityEngine;

public class DialogCommonData : MonoBehaviour
{
	public enum DialogType
	{
		DIALOG_NONE = 0,
		DIALOG_MESSAGE = 1,
		DIALOG_BUTTON = 2,
		DIALOG_CHARA_SELECT = 3
	}

	private static readonly Dictionary<DialogType, string> DIALOG_PREFAB_NAME_LIST = new Dictionary<DialogType, string>
	{
		{
			DialogType.DIALOG_MESSAGE,
			"MessageDialog"
		},
		{
			DialogType.DIALOG_BUTTON,
			"ChoiceDialog"
		},
		{
			DialogType.DIALOG_CHARA_SELECT,
			"CharaSelectDialogTmp"
		}
	};

	private static readonly string DIALOG_CANVAS_TAG_NAME = "DialogCanvas";

	private static readonly string DIALOG_SCREEN_OBJECT_NAME = "ScreenDialog";

	private static readonly string DIALOG_BG_OBJECT_NAME = "ScreenDialog/AreaObject/BgDialog";

	private static readonly string DIALOG_PARENT_OBJECT_NAME = "ScreenDialog/AreaObject/AreaDialog";

	private static readonly string DIALOG_ANIME_TRIGGER_NAME = "DialogOpenClose";

	private static readonly string DIALOG_OPENING_ANIME_NAME = "Base Layer.DialogCloseAnim";

	private static readonly string DIALOG_CLOSING_ANIME_NAME = "Base Layer.DialogOpenAnim";

	private static readonly string DIALOG_CLOSED_ANIME_NAME = "Base Layer.DialogClose";

	public static string GetDialogPrefabName(DialogType dialogType)
	{
		return DIALOG_PREFAB_NAME_LIST[dialogType];
	}

	public static string GetDialogScreenObjectName()
	{
		return DIALOG_SCREEN_OBJECT_NAME;
	}

	public static string GetDialogParentObjectName()
	{
		return DIALOG_PARENT_OBJECT_NAME;
	}

	public static string GetDialogAnimeTriggerName()
	{
		return DIALOG_ANIME_TRIGGER_NAME;
	}

	public static GameObject GetDialogCanvas(string sceneName)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(DIALOG_CANVAS_TAG_NAME);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject.scene.name == sceneName)
			{
				return gameObject;
			}
		}
		return null;
	}

	public static GameObject GetDialogScreenObject(string sceneName)
	{
		GameObject dialogCanvas = GetDialogCanvas(sceneName);
		if (!dialogCanvas)
		{
			Debug.LogError("[DialogCommonData] Error: cannot find DialogCanvas");
			return null;
		}
		Transform transform = dialogCanvas.transform.Find(DIALOG_SCREEN_OBJECT_NAME);
		return (!(transform != null)) ? null : transform.gameObject;
	}

	public static GameObject GetDialogParentObject(string sceneName)
	{
		GameObject dialogCanvas = GetDialogCanvas(sceneName);
		if (!dialogCanvas)
		{
			Debug.LogError("[DialogCommonData] Error: cannot find DialogCanvas");
			return null;
		}
		Transform transform = dialogCanvas.transform.Find(DIALOG_PARENT_OBJECT_NAME);
		return (!(transform != null)) ? null : transform.gameObject;
	}

	public static bool IsPlayingDialogOpenCloseAnime(string sceneName)
	{
		int num = Animator.StringToHash(DIALOG_OPENING_ANIME_NAME);
		int num2 = Animator.StringToHash(DIALOG_CLOSING_ANIME_NAME);
		AnimatorStateInfo currentAnimatorStateInfo = GetAnimator(sceneName).GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.fullPathHash == num)
		{
			return true;
		}
		if (currentAnimatorStateInfo.fullPathHash == num2)
		{
			return true;
		}
		return false;
	}

	public static bool IsDialogActive(string sceneName)
	{
		int num = Animator.StringToHash(DIALOG_CLOSED_ANIME_NAME);
		if (GetAnimator(sceneName).GetCurrentAnimatorStateInfo(0).fullPathHash == num)
		{
			return false;
		}
		return true;
	}

	public static bool ExistsDialogObject(string sceneName)
	{
		GameObject dialogParentObject = GetDialogParentObject(sceneName);
		return dialogParentObject.transform.childCount != 0;
	}

	public static void SetCanvasComponentActive(string sceneName, bool flag)
	{
		GetDialogCanvas(sceneName).GetComponent<Canvas>().enabled = flag;
	}

	public static void SetAnimeTriggerFlag(string sceneName, bool flag)
	{
		GetAnimator(sceneName).SetBool(GetDialogAnimeTriggerName(), flag);
		SetColliderActive(sceneName, flag);
	}

	public static Animator GetAnimator(string sceneName)
	{
		return GetDialogScreenObject(sceneName).GetComponent<Animator>();
	}

	public static GameObject InstantiateDialog(ResourceRequest prefabRequest, string sceneName)
	{
		return Object.Instantiate((GameObject)prefabRequest.asset, GetDialogParentObject(sceneName).transform);
	}

	private static void SetColliderActive(string sceneName, bool flag)
	{
		GameObject gameObject = GetDialogCanvas(sceneName).transform.Find(DIALOG_BG_OBJECT_NAME).gameObject;
		gameObject.GetComponent<BoxCollider2D>().enabled = flag;
	}
}
