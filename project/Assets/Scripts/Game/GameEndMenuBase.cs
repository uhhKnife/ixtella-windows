public class GameEndMenuBase : SwipeTextMenuBase
{
	protected override void LoadPrefab()
	{
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.MESSAGE_BG);
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.BOTTOM_BUTTON);
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.TEXT_PAGE_CONTROLLER);
	}

	protected override void OnButtonClick()
	{
		base.OnButtonClick();
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "FinalizeGame");
		GoToNextScene(Define.SceneId.SCENE_TITLE);
	}

	protected override string GetButtonTextId()
	{
		return GetGoToTitleTextId();
	}
}
