using UnityEngine;

public class OnStateEnterIdleAnimation : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "SetBlinkable", true);
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "SetMoveable", true);
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "SetClickable", true);
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "CheckStatus");
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "SetBlinkable", false);
	}
}
