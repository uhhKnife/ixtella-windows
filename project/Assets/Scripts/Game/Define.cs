using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Define
{
	public enum CharaId
	{
		CHID_NONE = -1,
		CHID_NERO = 0,
		CHID_TAMAMO = 1,
		CHID_ATTILA = 2,
		CHID_ELIZA = 3,
		CHID_MUMEI = 4,
		CHID_KARNA = 5,
		CHID_GILGAMESH = 6,
		CHID_CUCHULAIN = 7,
		CHID_ALTORIA = 8,
		CHID_CHARLE = 9,
		CHID_MASTER_M = 10,
		CHID_MASTER_F = 11,
		CHID_MAX = 12
	}

	public enum TagId
	{
		TAG_NONE = -1,
		TAG_GAME_CONTROLLER = 0,
		TAG_PLAYER = 1,
		TAG_FADE_CONTROLLER = 2,
		TAG_SCENE_CONTROLLER = 3,
		TAG_MENU_CONTROLLER = 4,
		TAG_BGM_CONTROLLER = 5,
		TAG_SE_CONTROLLER = 6,
		TAG_MENU_CANVAS = 7,
		TAG_STATUS_UI = 8,
		TAG_DIALOG_CANVAS = 9,
		TAG_PLAYABLE_STATE_CHECKER = 10,
		TAG_DATE_TIME_OBSERBER = 11,
		TAG_MAIN_CANVAS = 12,
		TAG_CHARACTER_BACKGROUND = 13,
		TAG_LETTER = 14,
		TAG_LOCAL_PUSH = 15,
		TAG_PLAYER_CANVAS = 16,
		TAG_PLAYER_BG_CANVAS = 17,
		TAG_TOUCH_SCREEN = 18,
		TAG_TITLE_BG_CANVAS = 19,
		TAG_MAX = 20
	}

	public enum SceneId
	{
		SCENE_NONE = -1,
		SCENE_TITLE = 0,
		SCENE_MAIN = 1,
		SCENE_DEBUG = 2,
		SCENE_MAX = 3
	}

	public enum SaveKey
	{
		KEY_PLAYER_STATUS = 0,
		KEY_SATIETY_TIME = 1,
		KEY_FEEL_TIME = 2,
		KEY_SATIETY_LIMIT = 3,
		KEY_FEEL_LIMIT = 4,
		KEY_SICK_LIMIT = 5,
		KEY_PHASE_INFO = 6,
		KEY_GAME_STATE = 7,
		KEY_PLAYER_GENDER = 8,
		KEY_GAME_START_DATE = 9,
		KEY_LAST_PLAY_DATE = 10
	}

	public static readonly Dictionary<CharaId, string> CHARAID_LIST = new Dictionary<CharaId, string>
	{
		{
			CharaId.CHID_NERO,
			"CH001"
		},
		{
			CharaId.CHID_TAMAMO,
			"CH002"
		},
		{
			CharaId.CHID_ATTILA,
			"CH003"
		},
		{
			CharaId.CHID_ELIZA,
			"CH004"
		},
		{
			CharaId.CHID_MUMEI,
			"CH006"
		},
		{
			CharaId.CHID_KARNA,
			"CH007"
		},
		{
			CharaId.CHID_GILGAMESH,
			"CH008"
		},
		{
			CharaId.CHID_CUCHULAIN,
			"CH009"
		},
		{
			CharaId.CHID_ALTORIA,
			"CH016"
		},
		{
			CharaId.CHID_CHARLE,
			"CH024"
		},
		{
			CharaId.CHID_MASTER_M,
			"CH301"
		},
		{
			CharaId.CHID_MASTER_F,
			"CH302"
		}
	};

	public static readonly Dictionary<TagId, string> TAGID_LIST = new Dictionary<TagId, string>
	{
		{
			TagId.TAG_GAME_CONTROLLER,
			"GameController"
		},
		{
			TagId.TAG_PLAYER,
			"Player"
		},
		{
			TagId.TAG_FADE_CONTROLLER,
			"Fader"
		},
		{
			TagId.TAG_SCENE_CONTROLLER,
			"SceneController"
		},
		{
			TagId.TAG_MENU_CONTROLLER,
			"MenuController"
		},
		{
			TagId.TAG_BGM_CONTROLLER,
			"BgmController"
		},
		{
			TagId.TAG_SE_CONTROLLER,
			"SeController"
		},
		{
			TagId.TAG_MENU_CANVAS,
			"MenuCanvas"
		},
		{
			TagId.TAG_STATUS_UI,
			"StatusUI"
		},
		{
			TagId.TAG_DIALOG_CANVAS,
			"DialogCanvas"
		},
		{
			TagId.TAG_PLAYABLE_STATE_CHECKER,
			"PlayableStateChecker"
		},
		{
			TagId.TAG_DATE_TIME_OBSERBER,
			"DateTimeObserber"
		},
		{
			TagId.TAG_MAIN_CANVAS,
			"MainCanvas"
		},
		{
			TagId.TAG_CHARACTER_BACKGROUND,
			"CharacterBackground"
		},
		{
			TagId.TAG_LETTER,
			"Letter"
		},
		{
			TagId.TAG_LOCAL_PUSH,
			"LocalPush"
		},
		{
			TagId.TAG_PLAYER_CANVAS,
			"PlayerCanvas"
		},
		{
			TagId.TAG_PLAYER_BG_CANVAS,
			"PlayerBGCanvas"
		},
		{
			TagId.TAG_TOUCH_SCREEN,
			"TouchScreen"
		},
		{
			TagId.TAG_TITLE_BG_CANVAS,
			"TitleBgCanvas"
		}
	};

	private static readonly Dictionary<SceneId, string> SCENE_NAME_LIST = new Dictionary<SceneId, string>
	{
		{
			SceneId.SCENE_TITLE,
			"Title"
		},
		{
			SceneId.SCENE_MAIN,
			"Main"
		},
		{
			SceneId.SCENE_DEBUG,
			"Debug"
		}
	};

	private static readonly Dictionary<SaveKey, string> SAVE_KEY_LIST = new Dictionary<SaveKey, string>
	{
		{
			SaveKey.KEY_PLAYER_STATUS,
			"PlayerStatus"
		},
		{
			SaveKey.KEY_SATIETY_TIME,
			"SatietyTimer"
		},
		{
			SaveKey.KEY_FEEL_TIME,
			"FeelTimer"
		},
		{
			SaveKey.KEY_SATIETY_LIMIT,
			"SatietyLimit"
		},
		{
			SaveKey.KEY_FEEL_LIMIT,
			"FeelLimit"
		},
		{
			SaveKey.KEY_SICK_LIMIT,
			"SickLimit"
		},
		{
			SaveKey.KEY_PHASE_INFO,
			"CurrentPhase"
		},
		{
			SaveKey.KEY_GAME_STATE,
			"GameState"
		},
		{
			SaveKey.KEY_PLAYER_GENDER,
			"PlayerGender"
		},
		{
			SaveKey.KEY_GAME_START_DATE,
			"StartGameTime"
		},
		{
			SaveKey.KEY_LAST_PLAY_DATE,
			"LastTime"
		}
	};

	private static readonly int FRAME_RATE = 30;

	private Define()
	{
	}

	public static void Initialize()
	{
	}

	public static string GetCharaName(CharaId id)
	{
		if (id < CharaId.CHID_NERO || id >= CharaId.CHID_MAX)
		{
			Debug.LogError("invalid charaid. -> " + id);
			return "DUMMY_CHID";
		}
		return CHARAID_LIST[id];
	}

	public static CharaId GetCharaIdFromName(string name)
	{
		return CHARAID_LIST.FirstOrDefault((KeyValuePair<CharaId, string> x) => x.Value.Equals(name)).Key;
	}

	public static CharaId GetMasterCharaId(PlayerStatusManager.Gender gender)
	{
		return (gender != PlayerStatusManager.Gender.GENDER_MALE) ? CharaId.CHID_MASTER_F : CharaId.CHID_MASTER_M;
	}

	public static bool IsMaleCharacter(CharaId id)
	{
		switch (id)
		{
		case CharaId.CHID_MUMEI:
		case CharaId.CHID_KARNA:
		case CharaId.CHID_GILGAMESH:
		case CharaId.CHID_CUCHULAIN:
		case CharaId.CHID_CHARLE:
		case CharaId.CHID_MASTER_M:
			return true;
		case CharaId.CHID_NERO:
		case CharaId.CHID_TAMAMO:
		case CharaId.CHID_ATTILA:
		case CharaId.CHID_ELIZA:
		case CharaId.CHID_ALTORIA:
		case CharaId.CHID_MASTER_F:
			return false;
		default:
			Debug.LogWarning("unknown gender charaId. -> " + id);
			return true;
		}
	}

	public static bool IsMasterId(CharaId id)
	{
		return id == CharaId.CHID_MASTER_M || id == CharaId.CHID_MASTER_F;
	}

	public static string GetTagName(TagId id)
	{
		if (id < TagId.TAG_GAME_CONTROLLER || id >= TagId.TAG_MAX)
		{
			Debug.LogError("invalid tagid. -> " + id);
			return "DUMMY_TAGID";
		}
		return TAGID_LIST[id];
	}

	public static string GetSceneName(SceneId id)
	{
		if (id <= SceneId.SCENE_NONE || id >= SceneId.SCENE_MAX)
		{
			Debug.LogError("invalid sceneid. -> " + id);
			return "DUMMY_SCENEID";
		}
		return SCENE_NAME_LIST[id];
	}

	public static SceneId GetSceneId(string sceneName)
	{
		foreach (KeyValuePair<SceneId, string> pair in SCENE_NAME_LIST)
		{
			if (pair.Value == sceneName)
			{
				return pair.Key;
			}
		}
		return SceneId.SCENE_NONE;
	}

	public static bool IsActiveScene(SceneId sceneId)
	{
		return SceneManager.GetActiveScene().name == GetSceneName(sceneId);
	}

	public static string GetSaveKey(SaveKey key)
	{
		if (key < SaveKey.KEY_PLAYER_STATUS || key > SaveKey.KEY_LAST_PLAY_DATE)
		{
			Debug.LogError("invalid key. -> " + key);
			return "DUMMY_KEY";
		}
		return SAVE_KEY_LIST[key];
	}

	public static int GetFrameRate()
	{
		return FRAME_RATE;
	}
}
