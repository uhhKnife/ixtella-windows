using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeTextController : SwipeObjectBase
{
	private List<string> pageTextList_;

	public void SetupPageText(ref List<string> textIdList)
	{
		SetTotalPageNum(textIdList.Count);
		SetPageTextList(ref textIdList);
		SetAllPageText();
	}

	private void SetPageTextList(ref List<string> textIdList)
	{
		pageTextList_ = new List<string>();
		for (int i = 0; i < textIdList.Count; i++)
		{
			pageTextList_.Add(SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(textIdList[i]));
		}
	}

	private void SetAllPageText()
	{
		List<GameObject> pageObjectList = GetPageObjectList();
		for (int i = 0; i < pageObjectList.Count; i++)
		{
			pageObjectList[i].GetComponentInChildren<Text>().text = GetPageText(i);
		}
	}

	private string GetPageText(int pageIndex)
	{
		if (0 <= pageIndex && pageIndex < pageTextList_.Count)
		{
			return pageTextList_[pageIndex];
		}
		Debug.LogError("[" + GetType().ToString() + "] Error: invalid page index " + pageIndex);
		return string.Empty;
	}
}
