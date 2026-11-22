using System.Collections.Generic;

public class SwipeTextMenuBase : MenuBase
{
	private List<string> textIdList_;

	private SwipeTextController pageController_;

	protected override void LoadPrefab()
	{
		base.LoadPrefab();
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.TEXT_PAGE_CONTROLLER);
	}

	protected override void Initialize()
	{
		base.Initialize();
		pageController_ = GetChildObject(MenuCommonData.ChildObjectType.TEXT_PAGE_CONTROLLER).GetComponent<SwipeTextController>();
		SetupTextIdList();
		pageController_.SetupPageText(ref textIdList_);
	}

	protected void SetupTextIdList()
	{
		int index = GetTextStartIndex();
		textIdList_ = new List<string>();
		string mainTextId = GetMainTextId(index);
		while (SingletonMonoBehaviour<TextDatabaseSystem>.Instance.ExistsText(mainTextId))
		{
			textIdList_.Add(mainTextId);
			index++;
			mainTextId = GetMainTextId(index);
		}
	}

	protected virtual int GetTextStartIndex()
	{
		return 0;
	}
}
