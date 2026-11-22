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
		GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
		pageWidth_ = grid.cellSize.x + grid.spacing.x;
	}

	protected void Update()
	{
		if (!isMovingToDestPosition_)
		{
			return;
		}
		float offset = pageWidth_ * (float)currentPageIndex_ + content.anchoredPosition.x;
		float distance = Mathf.Abs(offset);
		if (distance < 0.1f)
		{
			content.anchoredPosition = new Vector2(destPositionX_, content.anchoredPosition.y);
			isMovingToDestPosition_ = false;
			return;
		}
		float step = -offset * Time.deltaTime * FORCE_VALUE;
		float nextX = content.anchoredPosition.x + step;
		if (content.anchoredPosition.x > destPositionX_)
		{
			if (nextX <= destPositionX_)
			{
				nextX = destPositionX_;
			}
		}
		else if (nextX >= destPositionX_)
		{
			nextX = destPositionX_;
		}
		content.anchoredPosition = new Vector2(nextX, content.anchoredPosition.y);
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
		int targetIndex = currentPageIndex_;
		float pageX = pageWidth_ * (float)currentPageIndex_;
		float threshold = pageWidth_ / 4f;
		if (-content.anchoredPosition.x < pageX - threshold)
		{
			targetIndex--;
		}
		else if (pageX + threshold < 0f - content.anchoredPosition.x)
		{
			targetIndex++;
		}
		if (targetIndex < 0)
		{
			targetIndex = 0;
		}
		else if (targetIndex >= content.childCount)
		{
			targetIndex = content.childCount - 1;
		}
		if (currentPageIndex_ != targetIndex)
		{
			transform.parent.parent.SendMessage("SetCurrentPageIndex", targetIndex);
		}
		else
		{
			isMovingToDestPosition_ = true;
		}
	}

	private void SetCurrentPageIndex(int pageIndex)
	{
		isMovingToDestPosition_ = true;
		destPositionX_ = -pageWidth_ * (float)pageIndex;
		currentPageIndex_ = pageIndex;
	}
}
