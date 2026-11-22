using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : SingletonMonoBehaviour<MenuController>
{
	private MenuBase currentMenu_;

	private bool enablesTransition_;

	private MenuCommonData.MenuType currentMenuType_;

	private List<Define.CharaId> getCostumeList_;

	private bool isChangingMenu_;

	private void Awake()
	{
		if (this != SingletonMonoBehaviour<MenuController>.Instance)
		{
			Object.Destroy(gameObject);
		}
	}

	public MenuCommonData.MenuType GetCurrentMenuType()
	{
		return currentMenuType_;
	}

	public bool IsChangingMenu()
	{
		return isChangingMenu_;
	}

	private void OpenMenu(MenuCommonData.MenuType menuType)
	{
		StartCoroutine(ControlTransition(menuType));
	}

	private void OpenCostumeGetMenu(List<Define.CharaId> costumeList)
	{
		getCostumeList_ = costumeList;
		OpenMenu(MenuCommonData.MenuType.MENU_GET_COSTUME);
	}

	private IEnumerator ControlTransition(MenuCommonData.MenuType nextMenuType)
	{
		isChangingMenu_ = true;
		Debug.Log("[" + GetType().ToString() + "] change menu " + currentMenuType_.ToString() + " -> " + nextMenuType);
		bool isValidNextMenu = MenuCommonData.IsValidMenu(nextMenuType);
		MenuCommonData.MenuType prevMenuType = currentMenuType_;
		MenuBase prevMenu = currentMenu_;
		StartFadeOut(currentMenuType_, nextMenuType);
		if (isValidNextMenu)
		{
			enablesTransition_ = false;
			CreateMenuComponent(nextMenuType);
			while (!enablesTransition_)
			{
				yield return null;
			}
		}
		MenuScreenFader menuFader = GameObject.FindWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CANVAS)).GetComponent<MenuScreenFader>();
		while (menuFader.IsFading())
		{
			yield return null;
		}
		MainScreenFader mainFader = GameObject.FindWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS)).GetComponent<MainScreenFader>();
		while (mainFader.IsFading())
		{
			yield return null;
		}
		if (prevMenu != null)
		{
			Object.Destroy(prevMenu);
		}
		if (isValidNextMenu)
		{
			currentMenu_.SetAllObjectsActive(true);
			if (nextMenuType == MenuCommonData.MenuType.MENU_GET_COSTUME)
			{
				currentMenu_.SendMessage("SetupCharacter", getCostumeList_);
				getCostumeList_.Clear();
			}
		}
		currentMenuType_ = nextMenuType;
		StartFadeIn(prevMenuType, currentMenuType_);
		menuFader = GameObject.FindWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CANVAS)).GetComponent<MenuScreenFader>();
		while (menuFader.IsFading())
		{
			yield return null;
		}
		mainFader = GameObject.FindWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS)).GetComponent<MainScreenFader>();
		while (mainFader.IsFading())
		{
			yield return null;
		}
		isChangingMenu_ = false;
	}

	private void CloseMenu()
	{
		StartCoroutine(ControlTransition(MenuCommonData.MenuType.MENU_NONE));
	}

	private void CreateMenuComponent(MenuCommonData.MenuType menuType)
	{
		currentMenu_ = AddMenuScriptToObject(menuType);
		currentMenu_.SetMenuType(menuType);
	}

	private MenuBase AddMenuScriptToObject(MenuCommonData.MenuType menuType)
	{
		GameObject menuScreenObject = MenuCommonData.GetMenuScreenObject();
		switch (menuType)
		{
		case MenuCommonData.MenuType.MENU_OPTION_TOP:
			return menuScreenObject.AddComponent<OptionMenu>();
		case MenuCommonData.MenuType.MENU_MASTER_SELECT:
			return menuScreenObject.AddComponent<MasterSelectMenu>();
		case MenuCommonData.MenuType.MENU_HOW_TO_PLAY_TOP:
			return menuScreenObject.AddComponent<HowToPlayTopMenu>();
		case MenuCommonData.MenuType.MENU_HOW_TO_PLAY_1:
		case MenuCommonData.MenuType.MENU_HOW_TO_PLAY_2:
		case MenuCommonData.MenuType.MENU_HOW_TO_PLAY_4:
		case MenuCommonData.MenuType.MENU_HOW_TO_PLAY_5:
		case MenuCommonData.MenuType.MENU_HOW_TO_PLAY_6:
		case MenuCommonData.MenuType.MENU_HOW_TO_PLAY_7:
		case MenuCommonData.MenuType.MENU_HELP_MEAL_COMMAND:
		case MenuCommonData.MenuType.MENU_HELP_PLAY_COMMAND:
		case MenuCommonData.MenuType.MENU_HELP_DIAPER_COMMAND:
		case MenuCommonData.MenuType.MENU_HELP_SLEEP_COMMAND:
		case MenuCommonData.MenuType.MENU_HELP_TREATMENT:
		case MenuCommonData.MenuType.MENU_HELP_PARAMETER:
		case MenuCommonData.MenuType.MENU_HELP_COSTUME:
			return menuScreenObject.AddComponent<HowToPlayCommonMenu>();
		case MenuCommonData.MenuType.MENU_HOW_TO_CARE_COMMAND:
			return menuScreenObject.AddComponent<CommandHelpTopMenu>();
		case MenuCommonData.MenuType.MENU_USER_POLICY:
			return menuScreenObject.AddComponent<UserPolicyMenu>();
		case MenuCommonData.MenuType.MENU_CREDIT:
			return menuScreenObject.AddComponent<CreditMenu>();
		case MenuCommonData.MenuType.MENU_PARAMETER_CHECK:
			return menuScreenObject.AddComponent<ParameterCheckMenu>();
		case MenuCommonData.MenuType.MENU_GALLERY:
			return menuScreenObject.AddComponent<CostumeChangeMenu>();
		case MenuCommonData.MenuType.MENU_GET_COSTUME:
			return menuScreenObject.AddComponent<CostumeGetMenu>();
		case MenuCommonData.MenuType.MENU_GAME_OVER:
			return menuScreenObject.AddComponent<GameOverMenu>();
		case MenuCommonData.MenuType.MENU_GAME_CLEAR:
			return menuScreenObject.AddComponent<GameClearMenu>();
		default:
			Debug.LogError("[" + GetType().ToString() + "] AddScriptToMenu() Error: invaid menuType" + menuType);
			return null;
		}
	}

	private void SetEnableTransitionFlag(bool flag)
	{
		enablesTransition_ = flag;
	}

	private Canvas GetCanvas(Define.TagId tagId)
	{
		return GameObject.FindWithTag(Define.GetTagName(tagId)).GetComponent<Canvas>();
	}

	private void StartFadeIn(MenuCommonData.MenuType prevMenuType, MenuCommonData.MenuType currentMenuType)
	{
		if (currentMenuType == MenuCommonData.MenuType.MENU_NONE)
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "StartFadeIn", GetFadeFlag(prevMenuType, currentMenuType));
			if (Define.IsActiveScene(Define.SceneId.SCENE_TITLE))
			{
				MessageSender.SendWithTagMulti(Define.GetTagName(Define.TagId.TAG_TITLE_BG_CANVAS), "StartFadeIn");
			}
			else if (Define.IsActiveScene(Define.SceneId.SCENE_MAIN))
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER_CANVAS), "StartFadeIn");
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER_BG_CANVAS), "StartFadeIn");
			}
		}
		else
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CANVAS), "StartFadeIn");
		}
	}

	private void StartFadeOut(MenuCommonData.MenuType currentMenuType, MenuCommonData.MenuType nextMenuType)
	{
		if (currentMenuType == MenuCommonData.MenuType.MENU_NONE)
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "StartFadeOut", GetFadeFlag(currentMenuType, nextMenuType));
			if (Define.IsActiveScene(Define.SceneId.SCENE_TITLE))
			{
				MessageSender.SendWithTagMulti(Define.GetTagName(Define.TagId.TAG_TITLE_BG_CANVAS), "StartFadeOut");
			}
			else if (Define.IsActiveScene(Define.SceneId.SCENE_MAIN))
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER_CANVAS), "StartFadeOut");
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER_BG_CANVAS), "StartFadeOut");
			}
		}
		else
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CANVAS), "StartFadeOut");
		}
	}

	private uint GetFadeFlag(MenuCommonData.MenuType prevMenuType, MenuCommonData.MenuType nextMenuType)
	{
		if (ExistsStatus(prevMenuType) && ExistsStatus(nextMenuType))
		{
			return 6u;
		}
		return 7u;
	}

	private bool ExistsStatus(MenuCommonData.MenuType menuType)
	{
		switch (menuType)
		{
		case MenuCommonData.MenuType.MENU_NONE:
			return Define.IsActiveScene(Define.SceneId.SCENE_MAIN);
		case MenuCommonData.MenuType.MENU_PARAMETER_CHECK:
		case MenuCommonData.MenuType.MENU_GALLERY:
		case MenuCommonData.MenuType.MENU_GET_COSTUME:
			return true;
		default:
			return false;
		}
	}

	private void ClearMenuType()
	{
		currentMenuType_ = MenuCommonData.MenuType.MENU_NONE;
	}
}
