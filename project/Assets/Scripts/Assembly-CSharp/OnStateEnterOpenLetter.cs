using UnityEngine;

public class OnStateEnterOpenLetter : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Debug.Log("[LetterController] letter is opened.");
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CONTROLLER), "OpenMenu", MenuCommonData.MenuType.MENU_GAME_OVER);
	}
}
