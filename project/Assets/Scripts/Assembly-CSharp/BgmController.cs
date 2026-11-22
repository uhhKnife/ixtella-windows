using System.Collections;
using UnityEngine;

public class BgmController : SingletonMonoBehaviour<BgmController>
{
	private enum FadeState
	{
		STATE_FADE_NONE = 0,
		STATE_FADE_IN = 1,
		STATE_FADE_OUT = 2
	}

	public float fadeTime_;

	public float maxVolume_ = 1f;

	private TimeMeasurer timer_;

	private AudioSource audioSource_;

	private FadeState fadeState_;

	private readonly string BGM_FILENAME = "FixlTitleBgm";

	private readonly string BGM_MUTE_STATE_SAVE_KEY = "IsMuteBGM";

	private void Start()
	{
		bool muteBgm = SaveData.GetInt(BGM_MUTE_STATE_SAVE_KEY) == 1;
		SetMuteBgm(muteBgm);
	}

	private void Update()
	{
		if (!timer_.IsMeasuring())
		{
			return;
		}
		timer_.UpdateTime();
		UpdateVolume();
		if (timer_.IsEnd())
		{
			if (fadeState_ == FadeState.STATE_FADE_OUT)
			{
				StopBgm();
			}
			timer_.ResetTimer();
			fadeState_ = FadeState.STATE_FADE_NONE;
		}
	}

	private void Awake()
	{
		if (this != SingletonMonoBehaviour<BgmController>.Instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		timer_ = base.gameObject.AddComponent<TimeMeasurer>();
		audioSource_ = GetComponent<AudioSource>();
	}

	private void StartBgm()
	{
		if (audioSource_.isPlaying)
		{
			StopBgm();
		}
		StartCoroutine(ControlStartBgm(BGM_FILENAME));
	}

	private void RequestStartBgm(string fileName)
	{
		if (audioSource_.isPlaying)
		{
			StopBgm();
		}
		StartCoroutine(ControlStartBgm(fileName));
	}

	private IEnumerator ControlStartBgm(string filePath)
	{
		ResourceRequest request = Resources.LoadAsync(filePath);
		yield return request;
		audioSource_.clip = request.asset as AudioClip;
		if (audioSource_.clip == null)
		{
			Debug.LogWarning("[BgmController] Error: cannot find BGM file '" + filePath + "'");
		}
		else
		{
			Debug.Log("start BGM '" + filePath + "'");
			audioSource_.Play();
		}
		if (fadeState_ == FadeState.STATE_FADE_NONE)
		{
			audioSource_.volume = maxVolume_;
		}
	}

	private void StopBgm()
	{
		audioSource_.Stop();
		Resources.UnloadAsset(audioSource_.clip);
	}

	private void StartFadeInBgm()
	{
		fadeState_ = FadeState.STATE_FADE_IN;
		timer_.StartMesuaring(fadeTime_);
		StartBgm();
	}

	private void StartFadeOutBgm()
	{
		fadeState_ = FadeState.STATE_FADE_OUT;
		timer_.StartMesuaring(fadeTime_);
	}

	private void SetMuteBgm(bool flag)
	{
		audioSource_.mute = flag;
		Debug.Log("change BGM mute state " + ((!audioSource_.mute) ? "false" : "true"));
	}

	private void UpdateVolume()
	{
		if (fadeState_ == FadeState.STATE_FADE_NONE)
		{
			audioSource_.volume = maxVolume_;
		}
		else if (fadeTime_ > 0f)
		{
			float value = ((fadeState_ != FadeState.STATE_FADE_IN) ? (fadeTime_ - timer_.GetTimerCount()) : timer_.GetTimerCount()) / fadeTime_;
			value = Mathf.Clamp01(value);
			audioSource_.volume = value * maxVolume_;
		}
	}
}
