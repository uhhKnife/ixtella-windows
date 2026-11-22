using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class ButtonBase : MonoBehaviour
{
	private readonly string ICON_OBJECT_NAME = "AreaHorizontal/AreaIcon";

	private GameObject seControllerObject_;

	protected virtual void Start()
	{
		seControllerObject_ = GameObject.FindWithTag("SeController");
	}

	protected virtual void Awake()
	{
		SetButtonMainTextDirectly(string.Empty);
	}

	protected virtual void PlaySelectSE()
	{
		seControllerObject_.SendMessage("PlaySE", SeController.SeType.SE_SELECT_BUTTON);
	}

	protected virtual void PlayWarningSE()
	{
		seControllerObject_.SendMessage("PlaySE", SeController.SeType.SE_WARNING);
	}

	protected virtual void OnClickDisable()
	{
		PlayWarningSE();
	}

	protected GameObject GetSeControllerObject()
	{
		return seControllerObject_;
	}

	protected void SetButtonMainText(string textId)
	{
		SetButtonMainTextDirectly(SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(textId));
	}

	protected void SetButtonMainTextDirectly(string text)
	{
		Text componentInChildren = GetComponentInChildren<Text>();
		if (componentInChildren != null)
		{
			componentInChildren.text = text;
		}
	}

	private void OnClickCommon()
	{
		if (MenuCommonData.CanExecuteButtonOnClick(gameObject.scene.name))
		{
			OnClick();
		}
	}

	protected virtual void OnClick()
	{
		PlaySelectSE();
	}

	protected void SetupOnClickFunction()
	{
		SetOnClickFunction(OnClickCommon);
	}

	protected void SetOnClickFunction(UnityAction function)
	{
		Button button = GetComponent<Button>();
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(function);
	}

	protected void SetupDisableClickEvent()
	{
		EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener((BaseEventData data) =>
		{
			OnClickDisable();
		});
		eventTrigger.triggers.Clear();
		eventTrigger.triggers.Add(entry);
	}

	protected void ClearDisableClickEvent()
	{
		EventTrigger trigger = GetComponent<EventTrigger>();
		if (trigger != null)
		{
			trigger.triggers.Clear();
		}
	}

	protected void OpenMenu(MenuCommonData.MenuType menuType)
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CONTROLLER), "OpenMenu", menuType);
	}

	protected void SetIconActive(bool flag)
	{
		GameObject iconObject = GetIconObject();
		if (iconObject != null)
		{
			iconObject.SetActive(flag);
		}
	}

	protected void SetIcon(MenuCommonData.ButtonIconType iconType)
	{
		GameObject iconObject = GetIconObject();
		if (iconObject != null)
		{
			Image componentInChildren = iconObject.GetComponentInChildren<Image>();
			componentInChildren.sprite = PortSprites.Load(MenuCommonData.GetButtonIconFilename(iconType));
		}
	}

	private GameObject GetIconObject()
	{
		Transform icon = transform.Find(ICON_OBJECT_NAME);
		if (icon != null)
		{
			return icon.gameObject;
		}
		Debug.LogError("[" + GetType().ToString() + "] Error: cannot get icon object");
		return null;
	}
}
