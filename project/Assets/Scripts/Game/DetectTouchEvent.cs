using UnityEngine;

public class DetectTouchEvent : MonoBehaviour
{
	private readonly int DRAG_THRESHOLD = 160;

	private bool isTouching_;

	private bool isDragging_;

	private Vector3 touchedPosition_;

	public void MonitorTouchEvent()
	{
		if (isTouching_)
		{
			if (!isDragging_ && IsDraggedOverThreshold())
			{
				isDragging_ = true;
			}
		}
		else if (IsTouched())
		{
			isTouching_ = true;
			touchedPosition_ = Input.mousePosition;
		}
	}

	public void MonitorReleaseEvent()
	{
		if (IsReleased())
		{
			ResetTouchParam();
		}
	}

	public void ResetTouchParam()
	{
		isTouching_ = false;
		isDragging_ = false;
		touchedPosition_ = default(Vector3);
	}

	public bool IsTouched()
	{
		return Input.GetMouseButtonDown(0);
	}

	public bool IsReleased()
	{
		return Input.GetMouseButtonUp(0);
	}

	public bool IsDragging()
	{
		return isDragging_;
	}

	public bool IsTouchedNoColliderSpace()
	{
		if (!IsTouched())
		{
			return false;
		}
		return !IsTouchedCollider();
	}

	public bool IsReleasedNoColliderSpace()
	{
		if (!IsReleased())
		{
			return false;
		}
		return !IsTouchedCollider();
	}

	private bool IsTouchedCollider()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		return Physics2D.Raycast(ray.origin, ray.direction).collider != null;
	}

	private bool IsDraggedOverThreshold()
	{
		if (Mathf.Abs(Input.mousePosition.x - touchedPosition_.x) > (float)DRAG_THRESHOLD)
		{
			return true;
		}
		return Mathf.Abs(Input.mousePosition.y - touchedPosition_.y) > (float)DRAG_THRESHOLD;
	}
}
