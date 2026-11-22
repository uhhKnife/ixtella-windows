using UnityEngine;

public abstract class FaderBase<T> : SingletonMonoBehaviour<T> where T : MonoBehaviour
{
	public enum FadeState
	{
		NONE = 0,
		FADE_IN = 1,
		KEEPING = 2,
		WAITING_FADE_OUT = 3,
		FADE_OUT = 4
	}

	public float fadeTime_;

	private TimeMeasurer timer_;

	private float faderAlpha_;

	private FadeState fadeState_;

	protected void Start()
	{
		Initialize();
	}

	protected void Update()
	{
		if (!timer_.IsMeasuring())
		{
			return;
		}
		timer_.UpdateTime();
		bool flag = timer_.IsEnd();
		UpdateFadeAlpha(flag);
		ApplyColor();
		if (flag)
		{
			switch (fadeState_)
			{
			case FadeState.FADE_IN:
				OnFadeInEnd();
				break;
			case FadeState.KEEPING:
				OnKeepingEnd();
				break;
			case FadeState.FADE_OUT:
				OnFadeOutEnd();
				break;
			case FadeState.WAITING_FADE_OUT:
				break;
			}
		}
	}

	protected virtual void Awake()
	{
		timer_ = base.gameObject.AddComponent<TimeMeasurer>();
	}

	protected abstract void ApplyColor();

	protected abstract void SetCanvasActive(bool flag);

	protected abstract float GetInitialAlpha();

	protected abstract bool GetInitialCanvasActiveFlag();

	protected virtual void Initialize()
	{
		SetFaderAlpha(GetInitialAlpha());
		SetFadeState(FadeState.NONE);
		SetCanvasActive(GetInitialCanvasActiveFlag());
		ApplyColor();
	}

	protected virtual void OnFadeInEnd()
	{
		SetFadeState(FadeState.NONE);
		timer_.ResetTimer();
	}

	protected virtual void OnKeepingEnd()
	{
		SetFadeState(FadeState.WAITING_FADE_OUT);
		timer_.ResetTimer();
	}

	protected virtual void OnFadeOutEnd()
	{
		SetFadeState(FadeState.NONE);
		timer_.ResetTimer();
		SetCanvasActive(false);
	}

	private void UpdateFadeAlpha(bool isFadeEnd)
	{
		if (!(fadeTime_ <= 0f))
		{
			switch (fadeState_)
			{
			case FadeState.FADE_IN:
				faderAlpha_ = ((!isFadeEnd) ? (timer_.GetTimerCount() / fadeTime_) : 1f);
				break;
			case FadeState.FADE_OUT:
				faderAlpha_ = ((!isFadeEnd) ? ((fadeTime_ - timer_.GetTimerCount()) / fadeTime_) : 0f);
				break;
			}
		}
	}

	protected virtual void StartFadeIn()
	{
		if (NeedsFadeIn())
		{
			CreateFadeObjects();
			RegisterFadeObjects();
			SetFadeState(FadeState.FADE_IN);
			SetFaderAlpha(0f);
			SetFadeStartParam(fadeTime_);
			ApplyColor();
		}
	}

	protected virtual void StartFadeOut()
	{
		if (NeedsFadeOut())
		{
			RegisterFadeObjects();
			SetFadeState(FadeState.FADE_OUT);
			SetFaderAlpha(1f);
			SetFadeStartParam(fadeTime_);
			ApplyColor();
		}
	}

	protected virtual void CreateFadeObjects()
	{
	}

	protected virtual void RegisterFadeObjects()
	{
	}

	protected void SetFadeStartParam(float time)
	{
		SetCanvasActive(true);
		timer_.StartMesuaring(time);
	}

	protected void SetFaderAlpha(float alpha)
	{
		faderAlpha_ = alpha;
	}

	protected float GetFaderAlpha()
	{
		return faderAlpha_;
	}

	protected void SetFadeState(FadeState fadeState)
	{
		fadeState_ = fadeState;
	}

	protected virtual bool NeedsFadeIn()
	{
		return faderAlpha_ < 1f;
	}

	protected virtual bool NeedsFadeOut()
	{
		return faderAlpha_ > 0f;
	}

	protected CanvasGroup GetCanvasGroup(Define.TagId tagId, string canvasSceneName)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(Define.GetTagName(tagId));
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject.scene.name == canvasSceneName)
			{
				return gameObject.GetComponentInChildren<CanvasGroup>();
			}
		}
		return null;
	}

	protected void SetSpriteRendererAlpha(ref SpriteRenderer spriteRenderer, float alpha)
	{
		Color color = spriteRenderer.color;
		color.a = alpha;
		spriteRenderer.color = color;
	}

	public bool IsFading()
	{
		return timer_.IsMeasuring();
	}

	public bool EnablesFadeOut()
	{
		return fadeState_ == FadeState.WAITING_FADE_OUT;
	}
}
