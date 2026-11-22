using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommonButton : Button
{
	private readonly string RELEASE_ANIME_NAME = "Released";

	private readonly string NORMAL_ANIME_NAME = "Normal";

	private bool useWarningSEFlag_ = true;

	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);
		if (IsInteractable())
		{
			if (base.animator.isActiveAndEnabled)
			{
				base.animator.SetTrigger(RELEASE_ANIME_NAME);
			}
		}
		else if (useWarningSEFlag_)
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlaySE", SeController.SeType.SE_WARNING);
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		if (IsInteractable() && base.animator.isActiveAndEnabled && eventData.eligibleForClick)
		{
			base.animator.SetTrigger(NORMAL_ANIME_NAME);
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		if (IsInteractable() && base.animator.isActiveAndEnabled && eventData.dragging)
		{
			base.animator.SetTrigger(NORMAL_ANIME_NAME);
		}
	}

	public void SetAnimeNormal()
	{
		if (IsInteractable() && base.animator.isActiveAndEnabled)
		{
			base.animator.SetTrigger(NORMAL_ANIME_NAME);
		}
	}

	public void SetUseWarningSEFlag(bool flag)
	{
		useWarningSEFlag_ = flag;
	}
}
