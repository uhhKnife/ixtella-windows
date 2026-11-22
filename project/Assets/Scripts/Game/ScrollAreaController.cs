using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScrollAreaController : MonoBehaviour
{
	public enum ChildObjectType
	{
		SCROLL_TEXT = 0,
		CHECK_BOX = 1,
		SCROLL_BUTTON = 2
	}

	private Dictionary<ChildObjectType, string> CHILD_OBJECT_NAME_LIST = new Dictionary<ChildObjectType, string>
	{
		{
			ChildObjectType.SCROLL_TEXT,
			"ScrollView"
		},
		{
			ChildObjectType.CHECK_BOX,
			"AreaBottom"
		},
		{
			ChildObjectType.SCROLL_BUTTON,
			"ScrollViewButton"
		}
	};

	public GameObject buttonPrefab_;

	public Color urlColor_;

	private readonly string TEXT_OBJECT_NAME = "ScrollView/Viewport/Content";

	private readonly string CHECK_BOX_OBJECT_NAME = "AreaBottom/AreaHorizontal";

	private readonly string BUTTON_PARENT_OBJECT_NAME = "ScrollViewButton/Viewport/Content";

	protected void Awake()
	{
		SetupUrlLink();
	}

	public void SetChildObjectActive(ChildObjectType objectType, bool flag)
	{
		GameObject child = transform.Find(CHILD_OBJECT_NAME_LIST[objectType]).gameObject;
		child.SetActive(flag);
	}

	public void SetScrollText(string text)
	{
		GetScrollTextComponent().text = text;
	}

	public void SetScrollTextFontSize(int size)
	{
		GetScrollTextComponent().fontSize = size;
	}

	public void SetAlignment(TextAnchor anchor)
	{
		GetScrollTextComponent().alignment = anchor;
	}

	private void SetupUrlLink()
	{
		GetScrollTextComponent().SetClickableByRegex("http(s)?://([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?", urlColor_, (string url) =>
		{
			Application.OpenURL(url);
		});
	}

	private RegexHypertext GetScrollTextComponent()
	{
		GameObject textObject = transform.Find(TEXT_OBJECT_NAME).gameObject;
		return textObject.GetComponent<RegexHypertext>();
	}

	public void SetCheckBoxText(string text)
	{
		GetCheckBoxObject().GetComponentInChildren<Text>().text = text;
	}

	public void SetOnCheckChangedFunction(UnityAction<bool> function)
	{
		Toggle toggle = GetCheckBoxObject().GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(function);
	}

	private GameObject GetCheckBoxObject()
	{
		return transform.Find(CHECK_BOX_OBJECT_NAME).gameObject;
	}

	public GameObject AddButton()
	{
		GameObject buttonParent = transform.Find(BUTTON_PARENT_OBJECT_NAME).gameObject;
		return Object.Instantiate(buttonPrefab_, buttonParent.transform);
	}
}
