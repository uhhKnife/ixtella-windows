using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MenuBase : MonoBehaviour
{
	public enum CommonResourceType
	{
		TITLE = 0,
		MESSAGE_BG = 1,
		BUTTON = 2,
		MAX = 3
	}

	private readonly string TITLE_TEXT_PARENT_OBJECT_NAME = "AreaObject/AreaTitle/BgTitle";

	private readonly string TITLE_TEXT_OBJECT_NAME = "LabelTitle";

	private readonly string BUTTON_DEFAULT_TEXT_ID = "SYS_common_return";

	private readonly string BUTTON_TITLE_TEXT_ID = "SYS_common_return_title";

	private MenuCommonButton button_;

	private Dictionary<MenuCommonData.ChildObjectType, ResourceRequest> resourceRequestList_;

	private Dictionary<MenuCommonData.ChildObjectType, GameObject> objectList_;

	private bool isCompletedChildCreate_;

	private bool isButtonVisible_ = true;

	private MenuCommonData.MenuType menuType_;

	private GameObject menuController_;

	protected void Update()
	{
		if (isCompletedChildCreate_)
		{
			UpdateMenu();
		}
	}

	protected virtual void UpdateMenu()
	{
	}

	protected virtual void Awake()
	{
		menuController_ = GameObject.FindWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CONTROLLER));
		StartCoroutine(ControlLoading());
	}

	public void OnDestroy()
	{
		DestroyChildrenObjects();
	}

	protected IEnumerator ControlLoading()
	{
		isCompletedChildCreate_ = false;
		LoadPrefab();
		while (!IsCompletedLoading())
		{
			yield return null;
		}
		CreateChildrenObjects();
		Initialize();
		SetAllObjectsActive(false);
		menuController_.SendMessage("SetEnableTransitionFlag", true);
		isCompletedChildCreate_ = true;
	}

	protected virtual bool IsCompletedLoading()
	{
		foreach (ResourceRequest value in resourceRequestList_.Values)
		{
			if (!value.isDone)
			{
				return false;
			}
		}
		return true;
	}

	protected virtual void LoadPrefab()
	{
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.PAGE_TITLE);
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.MESSAGE_BG);
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.BOTTOM_BUTTON);
	}

	protected void AddPrefabLoadRequest(MenuCommonData.ChildObjectType objectType)
	{
		if (resourceRequestList_ == null)
		{
			resourceRequestList_ = new Dictionary<MenuCommonData.ChildObjectType, ResourceRequest>();
		}
		resourceRequestList_.Add(objectType, Resources.LoadAsync(MenuCommonData.GetPrefabName(objectType)));
	}

	protected virtual void CreateChildrenObjects()
	{
		foreach (MenuCommonData.ChildObjectType key in resourceRequestList_.Keys)
		{
			AddChildObject(key);
		}
	}

	protected void AddChildObject(MenuCommonData.ChildObjectType objectType)
	{
		if (objectList_ == null)
		{
			objectList_ = new Dictionary<MenuCommonData.ChildObjectType, GameObject>();
		}
		objectList_.Add(objectType, Object.Instantiate((GameObject)resourceRequestList_[objectType].asset, MenuCommonData.GetAreaObjectTransform(MenuCommonData.GetObjectCreateArea(objectType))));
		objectList_[objectType].name = resourceRequestList_[objectType].asset.name + "_" + MenuCommonData.GetMenuId(menuType_);
	}

	protected virtual void Initialize()
	{
		if (objectList_.ContainsKey(MenuCommonData.ChildObjectType.PAGE_TITLE))
		{
			SetMainTitleText(GetTitleTextId());
		}
		button_ = objectList_[MenuCommonData.ChildObjectType.BOTTOM_BUTTON].AddComponent<MenuCommonButton>();
		SetButtonText(GetButtonTextId());
		SetButtonOnClickFunction();
	}

	protected virtual void DestroyChildrenObjects()
	{
		foreach (GameObject value in objectList_.Values)
		{
			Object.Destroy(value);
		}
	}

	protected virtual string GetTitleTextId(int index = 0)
	{
		return MenuCommonData.GetMenuTitleTextId(menuType_, index);
	}

	protected virtual string GetMainTextId(int index = 0)
	{
		return MenuCommonData.GetMenuMainTextId(menuType_, index);
	}

	protected virtual string GetButtonTextId()
	{
		return BUTTON_DEFAULT_TEXT_ID;
	}

	private void OnButtonClickCommon()
	{
		if (MenuCommonData.CanExecuteButtonOnClick(base.gameObject.scene.name))
		{
			OnButtonClick();
		}
	}

	protected virtual void OnButtonClick()
	{
		button_.PlaySelectSE();
	}

	protected void SetMainTitleText(string textId)
	{
		if (!string.IsNullOrEmpty(textId))
		{
			GameObject gameObject = base.transform.Find(TITLE_TEXT_PARENT_OBJECT_NAME + "_" + MenuCommonData.GetMenuId(menuType_)).gameObject;
			GameObject gameObject2 = gameObject.transform.Find(TITLE_TEXT_OBJECT_NAME).gameObject;
			gameObject2.GetComponent<Text>().text = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(textId);
		}
	}

	protected void SetButtonText(string textId)
	{
		button_.SetButtonMainText(textId);
	}

	protected string GetGoToTitleTextId()
	{
		return BUTTON_TITLE_TEXT_ID;
	}

	private void SetButtonOnClickFunction()
	{
		button_.SetOnClickFunction(OnButtonClickCommon);
	}

	protected void SetButtonVisible(bool flag)
	{
		isButtonVisible_ = flag;
	}

	protected void SetButtonInteractive(bool flag)
	{
		button_.SetInteractable(flag);
	}

	protected void GoToNextMenu(MenuCommonData.MenuType nextMenuType)
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CONTROLLER), "OpenMenu", nextMenuType);
	}

	protected void CloseMenu()
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CONTROLLER), "CloseMenu");
	}

	protected void GoToNextScene(Define.SceneId sceneId)
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SCENE_CONTROLLER), "ChangeScene", sceneId);
	}

	protected PlayerStatusManager.Gender GetCurrentGender()
	{
		return SingletonMonoBehaviour<GameController>.Instance.GetPlayerGender();
	}

	protected GameObject GetChildObject(MenuCommonData.ChildObjectType objectType)
	{
		return objectList_[objectType];
	}

	public void SetMenuType(MenuCommonData.MenuType menuType)
	{
		menuType_ = menuType;
	}

	protected MenuCommonData.MenuType GetMenuType()
	{
		return menuType_;
	}

	public void SetAllObjectsActive(bool flag)
	{
		foreach (KeyValuePair<MenuCommonData.ChildObjectType, GameObject> item in objectList_)
		{
			if (item.Key != MenuCommonData.ChildObjectType.BOTTOM_BUTTON || isButtonVisible_ || !flag)
			{
				item.Value.SetActive(flag);
			}
		}
	}
}
