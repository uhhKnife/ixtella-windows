using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

public class GameController : SingletonMonoBehaviour<GameController>
{
	public enum GameState
	{
		GAME_STATE_NORMAL = 0,
		GAME_STATE_CLEAR = 1,
		GAME_STATE_GAMEOVER_BY_SATIETY = 2,
		GAME_STATE_GAMEOVER_BY_FEEL = 3,
		GAME_STATE_GAMEOVER_BY_SICK = 4
	}

	[Flags]
	public enum GameFlag
	{
		FLAG_GET_COSTUME = 2
	}

	private GameObject playerHandle_;

	private bool canCook_;

	private bool canSing_;

	private bool isDialogLoading_;

	private PlayerStatusManager.Gender playerGender_;

	private GameState gameState_;

	private GameFlag gameFlag_;

	[RuntimeInitializeOnLoadMethod]
	private static void OnRuntimeMethodLoad()
	{
	}

	private void Start()
	{
		if (Define.IsActiveScene(Define.SceneId.SCENE_TITLE))
		{
			MessageSender.SendWithTag("PlayableStateChecker", "StartPlayableStateCheck");
		}
		playerGender_ = (PlayerStatusManager.Gender)SaveData.GetInt("PlayerGender");
		gameState_ = (GameState)SaveData.GetInt("GameState");
		gameFlag_ = (GameFlag)SaveData.GetInt("GameFlag");
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_BGM_CONTROLLER), "StartBgm");
		if (Debug.isDebugBuild && Define.IsActiveScene(Define.SceneId.SCENE_MAIN))
		{
			StartCoroutine(DebugFadeIn());
		}
	}

	private void Update()
	{
		if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) && Input.GetKeyDown(KeyCode.Escape) && Define.IsActiveScene(Define.SceneId.SCENE_TITLE) && SingletonMonoBehaviour<MenuController>.Instance.GetCurrentMenuType() == MenuCommonData.MenuType.MENU_NONE && !isDialogLoading_ && !DialogCommonData.ExistsDialogObject(Define.GetSceneName(Define.SceneId.SCENE_TITLE)))
		{
			StartCoroutine(OpenGameQuitDialog());
		}
	}

	private void Awake()
	{
		Input.multiTouchEnabled = false;
		Application.targetFrameRate = Define.GetFrameRate();
		if (this != SingletonMonoBehaviour<GameController>.Instance)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
	}

	private void OnApplicationQuit()
	{
		WriteSaveData();
	}

	private void WriteSaveData()
	{
		SaveData.SetInt("GameState", (int)gameState_);
		SaveData.SetInt("GameFlag", (int)gameFlag_);
		SaveData.SetInt("PlayerGender", (int)playerGender_);
		SaveData.Save();
	}

	public void registerPlayer(GameObject obj)
	{
		playerHandle_ = obj;
	}

	private void SetPlayerGender(PlayerStatusManager.Gender gender)
	{
		playerGender_ = gender;
	}

	public PlayerStatusManager.Gender GetPlayerGender()
	{
		return playerGender_;
	}

	public bool IsMale()
	{
		return playerGender_ == PlayerStatusManager.Gender.GENDER_MALE;
	}

	private void RaiseGameFlag(GameFlag flag)
	{
		gameFlag_ |= flag;
	}

	private void DropGameFlag(GameFlag flag)
	{
		gameFlag_ &= ~flag;
	}

	public bool CheckGameFlag(GameFlag flag)
	{
		return (gameFlag_ & flag) != 0;
	}

	private void SetCookable(bool flag)
	{
		canCook_ = flag;
	}

	private void SetSingable(bool flag)
	{
		canSing_ = flag;
	}

	public bool CanCook()
	{
		return canCook_;
	}

	public bool CanSing()
	{
		return canSing_;
	}

	private void SetGameState(GameState state)
	{
		gameState_ = state;
	}

	private void ResetGameState()
	{
		gameState_ = GameState.GAME_STATE_NORMAL;
	}

	public GameState GetGameState()
	{
		return gameState_;
	}

	public bool IsGameClear()
	{
		return gameState_ == GameState.GAME_STATE_CLEAR;
	}

	public bool IsGameOver()
	{
		return gameState_ == GameState.GAME_STATE_GAMEOVER_BY_SATIETY || gameState_ == GameState.GAME_STATE_GAMEOVER_BY_FEEL || gameState_ == GameState.GAME_STATE_GAMEOVER_BY_SICK;
	}

	public void OnGameClear()
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "GameCreared");
		SetGameState(GameState.GAME_STATE_CLEAR);
	}

	public void OnGameEnd(GameState state)
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "GameOvered");
		SetGameState(state);
		if (Define.IsActiveScene(Define.SceneId.SCENE_MAIN) && SingletonMonoBehaviour<MenuController>.Instance.GetCurrentMenuType() == MenuCommonData.MenuType.MENU_NONE)
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "SetButtonsGameOverState");
		}
	}

	private void FinalizeGame()
	{
		if (DebugMode.Enabled)
		{
			SavePlayLog();
		}
		ResetGameState();
		SaveData.SetInt("GameState", (int)gameState_);
		gameFlag_ = (GameFlag)0;
		SetCookable(false);
		SetSingable(false);
		if (playerHandle_ != null)
		{
			playerHandle_.SetActive(true);
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "ResetGame");
			playerHandle_.SetActive(false);
		}
		SetPlayerGender(PlayerStatusManager.Gender.GENDER_MALE);
		SaveData.SetInt("PlayerGender", (int)playerGender_);
		GameObject letter = GameObject.FindWithTag("Letter");
		if (letter != null)
		{
			UnityEngine.Object.Destroy(letter);
		}
		EventScheduler scheduler = GetComponent<EventScheduler>();
		if (scheduler != null)
		{
			scheduler.SendMessage("ResetGame");
		}
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_DATE_TIME_OBSERBER), "ResetGame");
	}

	private void InitializeGame()
	{
		playerHandle_.SetActive(true);
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "InitializeStatus");
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "InitializePhase");
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_DATE_TIME_OBSERBER), "SetGameStartTime", DateTime.Now);
	}

	private IEnumerator DebugFadeIn()
	{
		yield return null;
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "StartFadeIn", 7u);
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER_CANVAS), "StartFadeIn");
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER_BG_CANVAS), "StartFadeIn");
	}

	private void SavePlayLog()
	{
		if (!DebugMode.Enabled)
		{
			return;
		}
		string path = Application.temporaryCachePath + "\\parameter_log.txt";
		using (StreamWriter writer = new StreamWriter(File.OpenWrite(path), Encoding.GetEncoding("utf-8")))
		{
			writer.Write("フェーズ\t体重\t経過時間\t体力\t知力\t魔力\r\n");
			for (int i = 0; i < SingletonMonoBehaviour<GrowthDatabase>.Instance.GetTotalPhaseNum(); i++)
			{
				string prefix = "Phase" + i;
				writer.Write(i + "\t" + SaveData.GetFloat(prefix + "Weight") + "\t" + SaveData.GetInt(prefix + "Hour") + ":" + SaveData.GetInt(prefix + "Minute") + ":" + SaveData.GetInt(prefix + "Second") + "\t" + SaveData.GetFloat(prefix + "Strength") + "\t" + SaveData.GetFloat(prefix + "Intelligence") + "\t" + SaveData.GetFloat(prefix + "Magic") + "\r\n");
				SaveData.SetFloat(prefix + "Weight", 0f);
				SaveData.SetInt(prefix + "Hour", 0);
				SaveData.SetInt(prefix + "Minute", 0);
				SaveData.SetInt(prefix + "Second", 0);
				SaveData.SetFloat(prefix + "Strength", 0f);
				SaveData.SetFloat(prefix + "Intelligence", 0f);
				SaveData.SetFloat(prefix + "Magic", 0f);
			}
			Debug.Log("Save Play Log.");
		}
	}

	private IEnumerator OpenGameQuitDialog()
	{
		isDialogLoading_ = true;
		string titleSceneName = Define.GetSceneName(Define.SceneId.SCENE_TITLE);
		DialogCommonData.SetCanvasComponentActive(titleSceneName, true);
		ResourceRequest prefabAsync = Resources.LoadAsync(DialogCommonData.GetDialogPrefabName(DialogCommonData.DialogType.DIALOG_MESSAGE));
		while (!prefabAsync.isDone)
		{
			yield return null;
		}
		GameObject dialogObject = DialogCommonData.InstantiateDialog(prefabAsync, titleSceneName);
		dialogObject.SetActive(true);
		dialogObject.AddComponent<GameQuitDialog>();
		DialogCommonData.SetAnimeTriggerFlag(titleSceneName, true);
		isDialogLoading_ = false;
	}
}
