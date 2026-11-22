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
		int num = GetTextStartIndex();
		textIdList_ = new List<string>();
		string mainTextId = GetMainTextId(num);
		while (SingletonMonoBehaviour<TextDatabaseSystem>.Instance.ExistsText(mainTextId))
		{
			textIdList_.Add(mainTextId);
			num++;
			mainTextId = GetMainTextId(num);
		}
	}

	protected virtual int GetTextStartIndex()
	{
		return 0;
	}
}
