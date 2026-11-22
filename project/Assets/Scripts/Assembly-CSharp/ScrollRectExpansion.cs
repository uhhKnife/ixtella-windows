using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectExpansion : ScrollRect
{
	private readonly float FORCE_VALUE = 6f;

	private float pageWidth_;

	private float destPositionX_;

	private bool isMovingToDestPosition_;

	private int currentPageIndex_;

	protected override void Awake()
	{
		base.Awake();
		GridLayoutGroup component = base.content.GetComponent<GridLayoutGroup>();
		pageWidth_ = component.cellSize.x + component.spacing.x;
	}

	protected void Update()
	{
		if (!isMovingToDestPosition_)
		{
			return;
		}
		float num = pageWidth_ * (float)currentPageIndex_ + base.content.anchoredPosition.x;
		float num2 = Mathf.Abs(num);
		if (num2 < 0.1f)
		{
			base.content.anchoredPosition = new Vector2(destPositionX_, base.content.anchoredPosition.y);
			isMovingToDestPosition_ = false;
			return;
		}
		float num3 = 0f - num * Time.deltaTime * FORCE_VALUE;
		float num4 = base.content.anchoredPosition.x + num3;
		if (base.content.anchoredPosition.x > destPositionX_)
		{
			if (num4 <= destPositionX_)
			{
				num4 = destPositionX_;
			}
		}
		else if (num4 >= destPositionX_)
		{
			num4 = destPositionX_;
		}
		base.content.anchoredPosition = new Vector2(num4, base.content.anchoredPosition.y);
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
		isMovingToDestPosition_ = false;
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
		StopMovement();
		int num = currentPageIndex_;
		float num2 = pageWidth_ * (float)currentPageIndex_;
		float num3 = pageWidth_ / 4f;
		if (0f - base.content.anchoredPosition.x < num2 - num3)
		{
			num--;
		}
		else if (num2 + num3 < 0f - base.content.anchoredPosition.x)
		{
			num++;
		}
		if (num < 0)
		{
			num = 0;
		}
		else if (num >= base.content.childCount)
		{
			num = base.content.childCount - 1;
		}
		if (currentPageIndex_ != num)
		{
			base.transform.parent.parent.SendMessage("SetCurrentPageIndex", num);
		}
		else
		{
			isMovingToDestPosition_ = true;
		}
	}

	private void SetCurrentPageIndex(int pageIndex)
	{
		isMovingToDestPosition_ = true;
		destPositionX_ = 0f - pageWidth_ * (float)pageIndex;
		currentPageIndex_ = pageIndex;
	}
}
