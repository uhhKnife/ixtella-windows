using UnityEngine;

public class MenuScreenFader : FaderBase<MenuScreenFader>
{
	private CanvasGroup menuCanvasGroup_;

	protected override void Initialize()
	{
		RegisterFadeObjects();
		base.Initialize();
	}

	protected override void ApplyColor()
	{
		menuCanvasGroup_.alpha = GetFaderAlpha();
	}

	protected override void SetCanvasActive(bool flag)
	{
		menuCanvasGroup_.interactable = flag;
	}

	protected override float GetInitialAlpha()
	{
		return 0f;
	}

	protected override bool GetInitialCanvasActiveFlag()
	{
		return false;
	}

	protected override void RegisterFadeObjects()
	{
		base.RegisterFadeObjects();
		menuCanvasGroup_ = GetCanvasGroup(Define.TagId.TAG_MENU_CANVAS, gameObject.scene.name);
	}
}
