using UnityEngine;

public class OnStateExitJoyJudgeClear : StateMachineBehaviour
{
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (SingletonMonoBehaviour<GameController>.Instance.IsGameClear())
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CONTROLLER), "OpenMenu", MenuCommonData.MenuType.MENU_GAME_CLEAR);
		}
	}
}
