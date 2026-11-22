using UnityEngine.UI;

public class GameClearMenu : GameEndMenuBase
{
	private readonly string GAME_CLEAR_TEXT_ID = "SYS_MENU_9_title_00";

	protected override void LoadPrefab()
	{
		base.LoadPrefab();
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.GAME_CLEAR_TITLE);
	}

	protected override void Initialize()
	{
		base.Initialize();
		GetChildObject(MenuCommonData.ChildObjectType.GAME_CLEAR_TITLE).GetComponentInChildren<Text>().text = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(GAME_CLEAR_TEXT_ID);
	}
}
