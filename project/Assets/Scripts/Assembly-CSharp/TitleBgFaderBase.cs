using UnityEngine;

public abstract class TitleBgFaderBase<T> : FaderBase<T> where T : MonoBehaviour
{
	protected override void Initialize()
	{
		RegisterFadeObjects();
		base.Initialize();
	}

	protected override void SetCanvasActive(bool flag)
	{
		GetComponent<Canvas>().enabled = flag;
	}

	protected override float GetInitialAlpha()
	{
		return 0f;
	}

	protected override bool GetInitialCanvasActiveFlag()
	{
		return false;
	}
}
