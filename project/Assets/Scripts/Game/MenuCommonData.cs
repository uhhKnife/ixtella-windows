using System.Collections.Generic;
using UnityEngine;

public class MenuCommonData : MonoBehaviour
{
	public enum MenuType
	{
		MENU_NONE = 0,
		MENU_OPTION_TOP = 1,
		MENU_MASTER_SELECT = 2,
		MENU_HOW_TO_PLAY_TOP = 3,
		MENU_USER_POLICY = 4,
		MENU_CREDIT = 5,
		MENU_PARAMETER_CHECK = 6,
		MENU_GALLERY = 7,
		MENU_GET_COSTUME = 8,
		MENU_GAME_OVER = 9,
		MENU_GAME_CLEAR = 10,
		MENU_HOW_TO_PLAY_1 = 11,
		MENU_HOW_TO_PLAY_2 = 12,
		MENU_HOW_TO_CARE_COMMAND = 13,
		MENU_HOW_TO_PLAY_4 = 14,
		MENU_HOW_TO_PLAY_5 = 15,
		MENU_HOW_TO_PLAY_6 = 16,
		MENU_HOW_TO_PLAY_7 = 17,
		MENU_HELP_MEAL_COMMAND = 18,
		MENU_HELP_PLAY_COMMAND = 19,
		MENU_HELP_DIAPER_COMMAND = 20,
		MENU_HELP_SLEEP_COMMAND = 21,
		MENU_HELP_TREATMENT = 22,
		MENU_HELP_PARAMETER = 23,
		MENU_HELP_COSTUME = 24,
		MENU_MAX = 25
	}

	public enum AreaType
	{
		AREA_TITLE = 0,
		AREA_MIDDLE = 1,
		AREA_BUTTON = 2,
		AREA_BOTTOM = 3
	}

	public enum ChildObjectType
	{
		PAGE_TITLE = 0,
		MESSAGE_BG = 1,
		BOTTOM_BUTTON = 2,
		BOTTOM_BUTTON_SUB = 3,
		OPTION_BUTTONS = 4,
		TEXT_PAGE_CONTROLLER = 5,
		CHARA_PAGE_CONTROLLER = 6,
		PARAMETER_CONTROLLER = 7,
		GENDER_OBJECTS_CONTROLLER = 8,
		SCROLL_TEXT_CONTROLLER = 9,
		GAME_OVER_TITLE = 10,
		GAME_CLEAR_TITLE = 11
	}

	public enum ButtonIconType
	{
		MEAL = 0,
		MEAL_MILK = 1,
		MEAL_SWEET = 2,
		MEAL_SPECIAL = 3,
		PLAY = 4,
		PLAY_BALL = 5,
		PLAY_BOOK = 6,
		PLAY_SONG = 7,
		DIAPER = 8,
		SLEEP = 9,
		TREATMENT = 10,
		PARAMETER = 11,
		COSTUME = 12
	}

	private static readonly Dictionary<MenuType, string> MENU_ID_LIST = new Dictionary<MenuType, string>
	{
		{
			MenuType.MENU_OPTION_TOP,
			"0-1-2"
		},
		{
			MenuType.MENU_MASTER_SELECT,
			"0-2"
		},
		{
			MenuType.MENU_HOW_TO_PLAY_TOP,
			"0-3"
		},
		{
			MenuType.MENU_USER_POLICY,
			"0-5"
		},
		{
			MenuType.MENU_CREDIT,
			"0-6"
		},
		{
			MenuType.MENU_PARAMETER_CHECK,
			"2"
		},
		{
			MenuType.MENU_GALLERY,
			"4"
		},
		{
			MenuType.MENU_GET_COSTUME,
			"7-1"
		},
		{
			MenuType.MENU_GAME_OVER,
			"8"
		},
		{
			MenuType.MENU_GAME_CLEAR,
			"9"
		},
		{
			MenuType.MENU_HOW_TO_PLAY_1,
			"0-4-1"
		},
		{
			MenuType.MENU_HOW_TO_PLAY_2,
			"0-4-2"
		},
		{
			MenuType.MENU_HOW_TO_CARE_COMMAND,
			"0-4-3"
		},
		{
			MenuType.MENU_HOW_TO_PLAY_4,
			"0-4-4"
		},
		{
			MenuType.MENU_HOW_TO_PLAY_5,
			"0-4-5"
		},
		{
			MenuType.MENU_HOW_TO_PLAY_6,
			"0-4-6"
		},
		{
			MenuType.MENU_HOW_TO_PLAY_7,
			"0-4-7"
		},
		{
			MenuType.MENU_HELP_MEAL_COMMAND,
			"0-4-11"
		},
		{
			MenuType.MENU_HELP_PLAY_COMMAND,
			"0-4-12"
		},
		{
			MenuType.MENU_HELP_DIAPER_COMMAND,
			"0-4-13"
		},
		{
			MenuType.MENU_HELP_SLEEP_COMMAND,
			"0-4-14"
		},
		{
			MenuType.MENU_HELP_TREATMENT,
			"0-4-15"
		},
		{
			MenuType.MENU_HELP_PARAMETER,
			"0-4-16"
		},
		{
			MenuType.MENU_HELP_COSTUME,
			"0-4-17"
		}
	};

	private static readonly Dictionary<AreaType, string> AREA_NAME_LIST = new Dictionary<AreaType, string>
	{
		{
			AreaType.AREA_TITLE,
			"AreaTitle"
		},
		{
			AreaType.AREA_MIDDLE,
			"AreaMid"
		},
		{
			AreaType.AREA_BUTTON,
			"AreaButton/AreaHorizontal"
		},
		{
			AreaType.AREA_BOTTOM,
			"AreaBottom"
		}
	};

	private static readonly Dictionary<ChildObjectType, string> PREFAB_NAME_LIST = new Dictionary<ChildObjectType, string>
	{
		{
			ChildObjectType.PAGE_TITLE,
			"BgTitle"
		},
		{
			ChildObjectType.MESSAGE_BG,
			"BgMessage"
		},
		{
			ChildObjectType.BOTTOM_BUTTON,
			"BtnCommon_1"
		},
		{
			ChildObjectType.BOTTOM_BUTTON_SUB,
			"BtnCommon_1"
		},
		{
			ChildObjectType.OPTION_BUTTONS,
			"AreaButtonVertical"
		},
		{
			ChildObjectType.TEXT_PAGE_CONTROLLER,
			"AreaSwipeVertical"
		},
		{
			ChildObjectType.CHARA_PAGE_CONTROLLER,
			"AreaCharaSelectVertical"
		},
		{
			ChildObjectType.PARAMETER_CONTROLLER,
			"AreaParam"
		},
		{
			ChildObjectType.GENDER_OBJECTS_CONTROLLER,
			"AreaSwitchTop"
		},
		{
			ChildObjectType.SCROLL_TEXT_CONTROLLER,
			"AreaScrollVertical"
		},
		{
			ChildObjectType.GAME_OVER_TITLE,
			"BgGameOver"
		},
		{
			ChildObjectType.GAME_CLEAR_TITLE,
			"BgGameClear"
		}
	};

	private static readonly Dictionary<Define.CharaId, string> CHARA_SPRITE_FILENAME_LIST = new Dictionary<Define.CharaId, string>
	{
		{
			Define.CharaId.CHID_NERO,
			"ui_baby_f_nero_0_00"
		},
		{
			Define.CharaId.CHID_TAMAMO,
			"ui_baby_f_tamamo_0_00"
		},
		{
			Define.CharaId.CHID_ATTILA,
			"ui_baby_f_altera_0_00"
		},
		{
			Define.CharaId.CHID_ELIZA,
			"ui_baby_f_eliza_0_00"
		},
		{
			Define.CharaId.CHID_MUMEI,
			"ui_baby_m_mumei_0_00"
		},
		{
			Define.CharaId.CHID_KARNA,
			"ui_baby_m_karna_0_00"
		},
		{
			Define.CharaId.CHID_GILGAMESH,
			"ui_baby_m_gilgamesh_0_00"
		},
		{
			Define.CharaId.CHID_CUCHULAIN,
			"ui_baby_m_cuchulainn_0_00"
		},
		{
			Define.CharaId.CHID_ALTORIA,
			"ui_baby_f_altria_0_00"
		},
		{
			Define.CharaId.CHID_CHARLE,
			"ui_baby_m_charle_0_00"
		},
		{
			Define.CharaId.CHID_MASTER_M,
			"ui_baby_m_base_0_00"
		},
		{
			Define.CharaId.CHID_MASTER_F,
			"ui_baby_f_base_0_00"
		}
	};

	private static readonly Dictionary<ButtonIconType, string> BUTTON_ICON_FILENAME_LIST = new Dictionary<ButtonIconType, string>
	{
		{
			ButtonIconType.MEAL,
			"UiIconCommand_01_00"
		},
		{
			ButtonIconType.MEAL_MILK,
			"UiIconMilk_00_00"
		},
		{
			ButtonIconType.MEAL_SWEET,
			"UiIconCookie_00_00"
		},
		{
			ButtonIconType.MEAL_SPECIAL,
			"UiIconFood_00_00"
		},
		{
			ButtonIconType.PLAY,
			"UiIconCommand_02_00"
		},
		{
			ButtonIconType.PLAY_BALL,
			"UiIconBall_00_00"
		},
		{
			ButtonIconType.PLAY_BOOK,
			"UiIconBook_00_00"
		},
		{
			ButtonIconType.PLAY_SONG,
			"UiIconSong_00_00"
		},
		{
			ButtonIconType.DIAPER,
			"UiIconCommand_03_00"
		},
		{
			ButtonIconType.SLEEP,
			"UiIconCommand_04_00"
		},
		{
			ButtonIconType.TREATMENT,
			"UiIconCommand_05_00"
		},
		{
			ButtonIconType.PARAMETER,
			"UiIconCommand_06_00"
		},
		{
			ButtonIconType.COSTUME,
			"UiIconCommand_07_00"
		}
	};

	private static readonly Dictionary<MenuType, ButtonIconType> MENU_BUTTON_ICON_TABLE = new Dictionary<MenuType, ButtonIconType>
	{
		{
			MenuType.MENU_HELP_MEAL_COMMAND,
			ButtonIconType.MEAL
		},
		{
			MenuType.MENU_HELP_PLAY_COMMAND,
			ButtonIconType.PLAY
		},
		{
			MenuType.MENU_HELP_DIAPER_COMMAND,
			ButtonIconType.DIAPER
		},
		{
			MenuType.MENU_HELP_SLEEP_COMMAND,
			ButtonIconType.SLEEP
		},
		{
			MenuType.MENU_HELP_TREATMENT,
			ButtonIconType.TREATMENT
		},
		{
			MenuType.MENU_HELP_PARAMETER,
			ButtonIconType.PARAMETER
		},
		{
			MenuType.MENU_HELP_COSTUME,
			ButtonIconType.COSTUME
		}
	};

	public static readonly string SCREEN_MENU_NAME = "ScreenMenu";

	public static readonly string AREA_OBJECT_NAME = "AreaObject";

	private static readonly string PAGE_TEXT_ID_FIRST_HALF = "SYS_MENU_";

	private static readonly string PAGE_TITLE_TEXT_ID_LAST_HALF = "_title_";

	private static readonly string PAGE_MAIN_TEXT_ID_LAST_HALF = "_main_";

	public static Transform GetAreaObjectTransform(AreaType areaType, ref GameObject screenMenuObject)
	{
		return screenMenuObject.transform.Find(AREA_NAME_LIST[areaType]);
	}

	public static Transform GetAreaObjectTransform(AreaType areaType)
	{
		return GetMenuScreenObject().transform.Find(AREA_OBJECT_NAME).transform.Find(AREA_NAME_LIST[areaType]);
	}

	public static GameObject GetMenuScreenObject()
	{
		GameObject canvas = GameObject.FindWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CANVAS)).gameObject;
		return canvas.transform.Find(SCREEN_MENU_NAME).gameObject;
	}

	public static bool IsValidMenu(MenuType menuType)
	{
		if (menuType <= MenuType.MENU_NONE)
		{
			return false;
		}
		return menuType < MenuType.MENU_MAX;
	}

	public static string GetMenuId(MenuType menuType)
	{
		return MENU_ID_LIST[menuType];
	}

	public static string GetPrefabName(ChildObjectType objectType)
	{
		return PREFAB_NAME_LIST[objectType];
	}

	public static AreaType GetObjectCreateArea(ChildObjectType objectType)
	{
		switch (objectType)
		{
		case ChildObjectType.PAGE_TITLE:
		case ChildObjectType.GAME_OVER_TITLE:
		case ChildObjectType.GAME_CLEAR_TITLE:
			return AreaType.AREA_TITLE;
		case ChildObjectType.BOTTOM_BUTTON:
		case ChildObjectType.BOTTOM_BUTTON_SUB:
			return AreaType.AREA_BUTTON;
		default:
			return AreaType.AREA_MIDDLE;
		}
	}

	public static string GetMenuTitleTextId(MenuType menuType, int index = 0)
	{
		return GetMenuTitleTextId(GetMenuId(menuType), index);
	}

	public static string GetMenuTitleTextId(string menuId, int index = 0)
	{
		return PAGE_TEXT_ID_FIRST_HALF + menuId + PAGE_TITLE_TEXT_ID_LAST_HALF + index.ToString("00");
	}

	public static string GetMenuMainTextId(MenuType menuType, int index = 0)
	{
		return GetMenuMainTextId(GetMenuId(menuType), index);
	}

	public static string GetMenuMainTextId(string menuId, int index = 0)
	{
		return PAGE_TEXT_ID_FIRST_HALF + menuId + PAGE_MAIN_TEXT_ID_LAST_HALF + index.ToString("00");
	}

	public static string GetCharaSpriteFilename(Define.CharaId charaId)
	{
		return CHARA_SPRITE_FILENAME_LIST[charaId];
	}

	public static string GetButtonIconFilename(ButtonIconType iconType)
	{
		return BUTTON_ICON_FILENAME_LIST[iconType];
	}

	public static ButtonIconType GetButtonIconType(MenuType menuType)
	{
		return MENU_BUTTON_ICON_TABLE[menuType];
	}

	public static bool IsAgreedUserPolicy()
	{
		return SaveData.GetInt("IsAgreedUserPolicy") == 1;
	}

	public static bool CanExecuteButtonOnClick(string sceneName)
	{
		if (SingletonMonoBehaviour<MenuController>.Instance.IsChangingMenu())
		{
			return false;
		}
		return !DialogCommonData.IsPlayingDialogOpenCloseAnime(sceneName);
	}
}
