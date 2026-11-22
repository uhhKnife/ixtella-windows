public class SceneTransitionButtonController : ButtonBase
{
	public Define.SceneId nextSceneType_;

	protected override void Start()
	{
		base.Start();
		SetupOnClickFunction();
	}

	protected override void OnClick()
	{
		base.OnClick();
		MessageSender.SendWithTag("SceneController", "ChangeScene", nextSceneType_);
	}
}
