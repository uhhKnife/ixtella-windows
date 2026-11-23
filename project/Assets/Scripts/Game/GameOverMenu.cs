using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : GameEndMenuBase
{
	private readonly Dictionary<GameController.GameState, int> TEXT_START_INDEX_LIST = new Dictionary<GameController.GameState, int>
	{
		{
			GameController.GameState.GAME_STATE_GAMEOVER_BY_SATIETY,
			0
		},
		{
			GameController.GameState.GAME_STATE_GAMEOVER_BY_FEEL,
			10
		},
		{
			GameController.GameState.GAME_STATE_GAMEOVER_BY_SICK,
			20
		}
	};

	private readonly string GAME_OVER_TEXT_ID = "SYS_MENU_8_title_00";

	protected override void LoadPrefab()
	{
		base.LoadPrefab();
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.GAME_OVER_TITLE);
	}

	protected override void Initialize()
	{
		base.Initialize();
		GetChildObject(MenuCommonData.ChildObjectType.GAME_OVER_TITLE).GetComponentInChildren<Text>().text = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(GAME_OVER_TEXT_ID);
	}

	protected override int GetTextStartIndex()
	{
		GameController.GameState key = SingletonMonoBehaviour<GameController>.Instance.GetGameState();
		if (DebugMode.Enabled && !TEXT_START_INDEX_LIST.ContainsKey(key))
		{
			Debug.LogWarning("[" + GetType().ToString() + "] Error: invalid game over state " + key);
			key = (GameController.GameState)Random.Range(2, 5);
		}
		return TEXT_START_INDEX_LIST[key];
	}

	protected override string GetMainTextId(int index = 0)
	{
		return base.GetMainTextId(index) + (SingletonMonoBehaviour<GameController>.Instance.IsMale() ? string.Empty : "_f");
	}
}
