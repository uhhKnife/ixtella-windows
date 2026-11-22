using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeObjectBase : MonoBehaviour
{
	public GameObject pagePrefab_;

	private readonly string SCROLL_VIEW_OBJECT_NAME = "AreaSwipe/ScrollView";

	private readonly string CURRENT_PAGE_SET_FUNCTION_NAME = "SetCurrentPageIndex";

	private readonly string TOTAL_PAGE_SET_FUNCTION_NAME = "SetTotalPageNum";

	private readonly string PAGE_PARENT_OBJECT_NAME = "AreaSwipe/ScrollView/Viewport/Content";

	private PagerController pagerController_;

	private ScrollRectExpansion scrollRect_;

	private GameObject pageParentObject_;

	private List<GameObject> pageObjectList_;

	protected virtual void Awake()
	{
		pagerController_ = GetComponentInChildren<PagerController>();
		scrollRect_ = base.transform.Find(SCROLL_VIEW_OBJECT_NAME).GetComponent<ScrollRectExpansion>();
		pageParentObject_ = base.transform.Find(PAGE_PARENT_OBJECT_NAME).gameObject;
	}

	protected void SetTotalPageNum(int num)
	{
		pageObjectList_ = new List<GameObject>();
		for (int i = 0; i < num; i++)
		{
			pageObjectList_.Add(Object.Instantiate(pagePrefab_, pageParentObject_.transform));
			pageObjectList_[i].SetActive(true);
		}
		if (num == 1)
		{
			pagerController_.gameObject.SetActive(false);
			SetScrollMovementType(ScrollRect.MovementType.Clamped);
		}
		else
		{
			pagerController_.SendMessage(TOTAL_PAGE_SET_FUNCTION_NAME, num);
			SetScrollMovementType(ScrollRect.MovementType.Elastic);
			SetCurrentPageIndex(0);
		}
	}

	protected void SetCurrentPageIndex(int pageIndex)
	{
		pagerController_.SendMessage(CURRENT_PAGE_SET_FUNCTION_NAME, pageIndex);
		scrollRect_.SendMessage(CURRENT_PAGE_SET_FUNCTION_NAME, pageIndex);
	}

	protected List<GameObject> GetPageObjectList()
	{
		return pageObjectList_;
	}

	protected void SetScrollMovementType(ScrollRect.MovementType movementType)
	{
		scrollRect_.movementType = movementType;
	}
}
