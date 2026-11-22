using UnityEngine;

public class SeController : SingletonMonoBehaviour<SeController>
{
	public enum SeType
	{
		SE_SELECT_BUTTON = 0,
		SE_WARNING = 1,
		SE_TOUCH_CHARACTER = 2,
		SE_BABY_VOICE_JOY = 3,
		SE_BABY_VOICE_REFUSE = 4
	}

	[SerializeField]
	[EnumLabel(typeof(SeType))]
	public AudioClip[] seClipList_;

	private readonly string SE_MUTE_STATE_SAVE_KEY = "IsMuteSE";

	private AudioSource audioSource_;

	private void Start()
	{
		bool muteSE = SaveData.GetInt(SE_MUTE_STATE_SAVE_KEY) == 1;
		SetMuteSE(muteSE);
	}

	private void Awake()
	{
		if (this != SingletonMonoBehaviour<SeController>.Instance)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			audioSource_ = GetComponent<AudioSource>();
		}
	}

	public void PlaySE(SeType seType)
	{
		if (IsPlaying())
		{
			StopSE();
		}
		audioSource_.PlayOneShot(GetAudioClip(seType));
	}

	public void StopSE()
	{
		audioSource_.Stop();
	}

	private void SetMuteSE(bool flag)
	{
		audioSource_.mute = flag;
		Debug.Log("change SE mute state " + ((!audioSource_.mute) ? "false" : "true"));
	}

	public bool IsPlaying()
	{
		return audioSource_.isPlaying;
	}

	private AudioClip GetAudioClip(SeType seType)
	{
		return seClipList_[(int)seType];
	}

	private void PlayMasterVoiceSe(SeType seType)
	{
		if (Define.IsActiveScene(Define.SceneId.SCENE_MAIN))
		{
			PlaySE(seType);
		}
	}
}
