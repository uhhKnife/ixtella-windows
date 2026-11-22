using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TitleBgImageFader : TitleBgFaderBase<TitleBgImageFader>
{
	[SerializeField]
	private float fadeInWaitTime_;

	private CanvasGroup canvasGroup_;

	protected override void ApplyColor()
	{
		canvasGroup_.alpha = GetFaderAlpha();
	}

	protected override void SetCanvasActive(bool flag)
	{
		GetComponent<Canvas>().enabled = flag;
	}

	protected override void RegisterFadeObjects()
	{
		base.RegisterFadeObjects();
		canvasGroup_ = GetComponent<CanvasGroup>();
	}

	protected override void StartFadeIn()
	{
		StartCoroutine(ControlStartFadeIn());
	}

	private IEnumerator ControlStartFadeIn()
	{
		yield return new WaitForSeconds(fadeInWaitTime_);
		_003CStartFadeIn_003E__BaseCallProxy0();
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private void _003CStartFadeIn_003E__BaseCallProxy0()
	{
		base.StartFadeIn();
	}
}
