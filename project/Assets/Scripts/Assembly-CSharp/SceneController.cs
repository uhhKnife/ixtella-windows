using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : SingletonMonoBehaviour<SceneController>
{
	private const int RELEASE_WAIT_DELAY = 5;

	private Define.SceneId currentSceneId_ = Define.SceneId.SCENE_NONE;

	private Define.SceneId prevSceneId_ = Define.SceneId.SCENE_NONE;

	private void Awake()
	{
		if (this != SingletonMonoBehaviour<SceneController>.Instance)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			currentSceneId_ = Define.GetSceneId(SceneManager.GetActiveScene().name);
		}
	}

	public Define.SceneId GetPrevSceneId()
	{
		return prevSceneId_;
	}

	private void ChangeScene(Define.SceneId nextSceneType)
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CONTROLLER), "ClearMenuType");
		StartCoroutine(ControlTransition(nextSceneType));
	}

	private IEnumerator ControlTransition(Define.SceneId nextSceneId)
	{
		if (nextSceneId == currentSceneId_)
		{
			Debug.LogWarning("[" + GetType().ToString() + "] Warning: cannot transfer the same scene " + nextSceneId);
			yield break;
		}
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_FADE_CONTROLLER), "StartFadeIn", LoadingFader.LoadMessageType.LOADING);
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CANVAS), "StartFadeOut");
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "StartFadeOut", 7u);
		switch (currentSceneId_)
		{
		case Define.SceneId.SCENE_TITLE:
			MessageSender.SendWithTagMulti(Define.GetTagName(Define.TagId.TAG_TITLE_BG_CANVAS), "StartFadeOut");
			break;
		case Define.SceneId.SCENE_MAIN:
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER_CANVAS), "StartFadeOut");
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER_BG_CANVAS), "StartFadeOut");
			break;
		}
		AsyncOperation nextSceneAsync = SceneManager.LoadSceneAsync(Define.GetSceneName(nextSceneId), LoadSceneMode.Additive);
		nextSceneAsync.allowSceneActivation = false;
		GameObject fader = GameObject.FindWithTag(Define.GetTagName(Define.TagId.TAG_FADE_CONTROLLER));
		LoadingFader fadeController = fader.GetComponent<LoadingFader>();
		while (!fadeController.EnablesFadeOut())
		{
			yield return null;
		}
		Resources.UnloadUnusedAssets();
		while (nextSceneAsync.progress < 0.9f)
		{
			yield return null;
		}
		nextSceneAsync.allowSceneActivation = true;
		yield return nextSceneAsync;
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(Define.GetSceneName(nextSceneId)));
		Debug.Log("change scene " + currentSceneId_.ToString() + " -> " + nextSceneId);
		prevSceneId_ = currentSceneId_;
		currentSceneId_ = nextSceneId;
		if (prevSceneId_ != Define.SceneId.SCENE_NONE)
		{
			SceneManager.UnloadSceneAsync(Define.GetSceneName(prevSceneId_));
			Debug.Log("unload scene " + prevSceneId_);
		}
		for (int i = 0; i < 5; i++)
		{
			yield return null;
		}
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_FADE_CONTROLLER), "StartFadeOut");
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "StartFadeIn", 7u);
		switch (currentSceneId_)
		{
		case Define.SceneId.SCENE_TITLE:
			MessageSender.SendWithTagMulti(Define.GetTagName(Define.TagId.TAG_TITLE_BG_CANVAS), "StartFadeIn");
			break;
		case Define.SceneId.SCENE_MAIN:
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "ChangeCostume", SingletonMonoBehaviour<PlayerStatusManager>.Instance.GetEquipedCharaID());
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER_CANVAS), "StartFadeIn");
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER_BG_CANVAS), "StartFadeIn");
			break;
		}
	}
}
