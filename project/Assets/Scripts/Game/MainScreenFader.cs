using System.Collections.Generic;
using UnityEngine;

public class MainScreenFader : FaderBase<MainScreenFader>
{
	public enum AreaType
	{
		TOP = 1,
		MIDDLE = 2,
		BOTTOM = 4,
		ALL = 7
	}

	private readonly Dictionary<AreaType, string> AREA_OBJECT_NAME_LIST = new Dictionary<AreaType, string>
	{
		{
			AreaType.TOP,
			"AreaObject/AreaTop"
		},
		{
			AreaType.MIDDLE,
			"AreaObject/AreaMid"
		},
		{
			AreaType.BOTTOM,
			"AreaObject/AreaBottom"
		}
	};

	private readonly string MAIN_SCREEN_OBJECT_NAME = "ScreenMain";

	private Dictionary<AreaType, CanvasGroup> canvasGroupList_;

	private uint fadeAreaFlag_ = 7u;

	protected override void Initialize()
	{
		fadeAreaFlag_ = 7u;
		RegisterFadeObjects();
		base.Initialize();
		if (Define.IsActiveScene(Define.SceneId.SCENE_TITLE))
		{
			SetCanvasActive(false);
		}
	}

	protected override void ApplyColor()
	{
		if (GetAreaTypeFlag(fadeAreaFlag_, AreaType.ALL))
		{
			foreach (AreaType area in canvasGroupList_.Keys)
			{
				canvasGroupList_[area].alpha = GetFaderAlpha();
			}
			return;
		}
		foreach (AreaType area in canvasGroupList_.Keys)
		{
			if (area != AreaType.ALL && GetAreaTypeFlag(fadeAreaFlag_, area))
			{
				canvasGroupList_[area].alpha = GetFaderAlpha();
			}
		}
	}

	protected override void SetCanvasActive(bool flag)
	{
		if (GetAreaTypeFlag(fadeAreaFlag_, AreaType.ALL))
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "SetAllButtonActive", flag);
		}
		else
		{
			if (!Define.IsActiveScene(Define.SceneId.SCENE_MAIN))
			{
				return;
			}
			if (GetAreaTypeFlag(fadeAreaFlag_, AreaType.TOP))
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "SetButtonActive", 7, flag);
			}
			if (GetAreaTypeFlag(fadeAreaFlag_, AreaType.BOTTOM))
			{
				for (int i = 0; i <= 6; i++)
				{
					MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "SetButtonActive", i, flag);
				}
			}
		}
	}

	protected override float GetInitialAlpha()
	{
		return 0f;
	}

	protected override bool GetInitialCanvasActiveFlag()
	{
		return true;
	}

	protected override void RegisterFadeObjects()
	{
		base.RegisterFadeObjects();
		canvasGroupList_ = new Dictionary<AreaType, CanvasGroup>();
		GameObject screen = transform.Find(MAIN_SCREEN_OBJECT_NAME).gameObject;
		canvasGroupList_.Add(AreaType.ALL, screen.GetComponent<CanvasGroup>());
		if (!Define.IsActiveScene(Define.SceneId.SCENE_MAIN))
		{
			return;
		}
		foreach (KeyValuePair<AreaType, string> pair in AREA_OBJECT_NAME_LIST)
		{
			canvasGroupList_.Add(pair.Key, screen.transform.Find(pair.Value).gameObject.GetComponent<CanvasGroup>());
		}
	}

	protected override bool NeedsFadeIn()
	{
		foreach (AreaType key in canvasGroupList_.Keys)
		{
			if (canvasGroupList_[key].alpha < 1f)
			{
				return true;
			}
		}
		return false;
	}

	protected override bool NeedsFadeOut()
	{
		foreach (AreaType key in canvasGroupList_.Keys)
		{
			if (canvasGroupList_[key].alpha > 0f)
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnFadeInEnd()
	{
		base.OnFadeInEnd();
		if (Define.IsActiveScene(Define.SceneId.SCENE_TITLE))
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_TOUCH_SCREEN), "SetActive", true);
		}
		else if (Define.IsActiveScene(Define.SceneId.SCENE_MAIN))
		{
			if (SingletonMonoBehaviour<GameController>.Instance.CheckGameFlag(GameController.GameFlag.FLAG_GET_COSTUME))
			{
				List<Define.CharaId> arg = SingletonMonoBehaviour<GrowthDatabase>.Instance.CollectGetableCostume();
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CONTROLLER), "OpenCostumeGetMenu", arg);
			}
			else if (SingletonMonoBehaviour<GameController>.Instance.IsGameClear() && !SingletonMonoBehaviour<PlayerController>.Instance.IsPlayingAchieve())
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "GameClearJoy");
			}
		}
	}

	private bool GetAreaTypeFlag(uint flag, AreaType areaType)
	{
		return (flag & (uint)areaType) == (uint)areaType;
	}

	private void StartFadeIn(uint fadeAreaFlag)
	{
		fadeAreaFlag_ = fadeAreaFlag;
		if (!GetAreaTypeFlag(fadeAreaFlag_, AreaType.ALL))
		{
			foreach (AreaType key in canvasGroupList_.Keys)
			{
				if (key != AreaType.ALL && canvasGroupList_[key].alpha > 0f)
				{
					fadeAreaFlag_ &= (uint)(~key);
				}
			}
		}
		base.StartFadeIn();
		SetAllButtonsState();
	}

	private void StartFadeOut(uint fadeAreaFlag)
	{
		fadeAreaFlag_ = fadeAreaFlag;
		foreach (AreaType key in canvasGroupList_.Keys)
		{
			if (key != AreaType.ALL && canvasGroupList_[key].alpha < 1f)
			{
				fadeAreaFlag_ &= (uint)(~key);
			}
		}
		base.StartFadeOut();
		if (Define.IsActiveScene(Define.SceneId.SCENE_TITLE))
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_TOUCH_SCREEN), "SetActive", false);
		}
	}

	private void SetAllButtonsState()
	{
		ResetAllButtonsAnimeState();
		if (Define.IsActiveScene(Define.SceneId.SCENE_TITLE))
		{
			if (MenuCommonData.IsAgreedUserPolicy())
			{
				SetMainButtonInteractive(0, true);
			}
			else
			{
				SetMainButtonInteractive(0, false);
			}
		}
		else if (Define.IsActiveScene(Define.SceneId.SCENE_MAIN))
		{
			if (SingletonMonoBehaviour<GameController>.Instance.IsGameOver())
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "SetButtonsGameOverState");
			}
			else if (SingletonMonoBehaviour<GameController>.Instance.IsGameClear())
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "SetAllButtonInteractive", false);
			}
		}
	}

	private void SetMainButtonInteractive(int buttonIndex, bool flag)
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "SetButtonInteractive", buttonIndex, flag);
	}

	private void ResetAllButtonsAnimeState()
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "SetAllButtonAnimeNormal");
	}
}
