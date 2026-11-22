using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingFader : FaderBase<LoadingFader>
{
	public enum LoadMessageType
	{
		LOADING = 0,
		CONNECTING = 1
	}

	private Dictionary<LoadMessageType, string> LOAD_MESSAGE_TEXT_ID_LIST = new Dictionary<LoadMessageType, string>
	{
		{
			LoadMessageType.LOADING,
			"SYS_LOADING_loading"
		},
		{
			LoadMessageType.CONNECTING,
			"SYS_LOADING_connecting"
		}
	};

	public float fadeKeepTime_;

	public GameObject loadingScreenPrefab_;

	private readonly string MESSAGE_TEXT_OBJECT_NAME = "ScreenLoading/AreaObject/AreaLabelLoading/LabelLoading";

	private readonly string ICON_PARENT_OBJECT_NAME = "ScreenLoading/AreaObject/AreaIcon";

	private GameObject loadingScreenObject_;

	private Canvas faderCanvas_;

	private CanvasGroup messageCanvasGroup_;

	private LoadMessageType messageType_;

	protected override void Awake()
	{
		if (this != SingletonMonoBehaviour<LoadingFader>.Instance)
		{
			Object.Destroy(gameObject);
			return;
		}
		Object.DontDestroyOnLoad(gameObject);
		base.Awake();
	}

	protected override void Initialize()
	{
		faderCanvas_ = GetComponent<Canvas>();
		if (Define.IsActiveScene(Define.SceneId.SCENE_TITLE))
		{
			CreateFadeObjects();
			RegisterFadeObjects();
			SetFaderAlpha(1f);
			SetFadeState(FadeState.KEEPING);
			SetFadeStartParam(fadeTime_);
			ApplyColor();
		}
		else
		{
			base.Initialize();
		}
	}

	protected override void ApplyColor()
	{
		if (messageCanvasGroup_ != null)
		{
			messageCanvasGroup_.alpha = GetFaderAlpha();
		}
	}

	protected override void SetCanvasActive(bool flag)
	{
		faderCanvas_.enabled = flag;
	}

	protected override float GetInitialAlpha()
	{
		return 0f;
	}

	protected override bool GetInitialCanvasActiveFlag()
	{
		return Define.IsActiveScene(Define.SceneId.SCENE_TITLE);
	}

	protected override void CreateFadeObjects()
	{
		base.CreateFadeObjects();
		loadingScreenObject_ = Object.Instantiate(loadingScreenPrefab_, transform);
		loadingScreenObject_.name = loadingScreenPrefab_.name;
	}

	protected override void RegisterFadeObjects()
	{
		base.RegisterFadeObjects();
		messageCanvasGroup_ = GetComponentInChildren<CanvasGroup>();
		SetMessageText(messageType_);
	}

	protected override void OnFadeInEnd()
	{
		base.OnFadeInEnd();
		SetFadeState(FadeState.KEEPING);
		SetFadeStartParam(fadeKeepTime_);
	}

	protected override void OnFadeOutEnd()
	{
		base.OnFadeOutEnd();
		Object.Destroy(loadingScreenObject_.gameObject);
		messageType_ = LoadMessageType.LOADING;
	}

	private void StartFadeIn(LoadMessageType messageType)
	{
		base.StartFadeIn();
		SetMessageType(messageType);
	}

	private void SetMessageType(LoadMessageType messageType)
	{
		messageType_ = messageType;
		SetMessageText(messageType_);
	}

	private void SetMessageText(LoadMessageType messageType)
	{
		Transform message = transform.Find(MESSAGE_TEXT_OBJECT_NAME);
		if (message != null)
		{
			message.gameObject.GetComponent<Text>().text = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(LOAD_MESSAGE_TEXT_ID_LIST[messageType_]);
		}
	}

	private void SetActiveIconRandomly()
	{
		Transform icons = transform.Find(ICON_PARENT_OBJECT_NAME).transform;
		int shown = Random.Range(0, icons.childCount);
		for (int i = 0; i < icons.childCount; i++)
		{
			icons.GetChild(i).gameObject.SetActive(i == shown);
		}
	}
}
