using UnityEngine;
using UnityEngine.UI;

public class TitleTouchScreen : MonoBehaviour
{
	private readonly string TOUCH_TEXT_ID = "SYS_MENU_0-1_touch_screen";

	private DetectTouchEvent touchEvent_;

	private bool isActive_;

	private void Start()
	{
		GetComponentInChildren<Text>().text = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(TOUCH_TEXT_ID);
	}

	private void Awake()
	{
		touchEvent_ = gameObject.AddComponent<DetectTouchEvent>();
	}

	private void Update()
	{
		if (isActive_)
		{
			touchEvent_.MonitorTouchEvent();
			if (!touchEvent_.IsDragging() && touchEvent_.IsReleasedNoColliderSpace() && !DialogCommonData.ExistsDialogObject(gameObject.scene.name))
			{
				GoToMainScene();
			}
			touchEvent_.MonitorReleaseEvent();
		}
	}

	private void GoToMainScene()
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlaySE", SeController.SeType.SE_SELECT_BUTTON);
		if (MenuCommonData.IsAgreedUserPolicy())
		{
			if (SaveData.GetClass("CurrentPhase", new EventScheduler.PhaseEventInfo()).isNew_)
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CONTROLLER), "OpenMenu", MenuCommonData.MenuType.MENU_MASTER_SELECT);
			}
			else
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SCENE_CONTROLLER), "ChangeScene", Define.SceneId.SCENE_MAIN);
			}
		}
		else
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CONTROLLER), "OpenMenu", MenuCommonData.MenuType.MENU_USER_POLICY);
		}
	}

	private void SetActive(bool flag)
	{
		isActive_ = flag;
		touchEvent_.ResetTouchParam();
	}
}
