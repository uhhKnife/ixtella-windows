using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Button Common")]
public class ButtonCommonComponent : MonoBehaviour
{
	public int buttonAnimeListIndex;

	private void Awake()
	{
		SetButtonComponent();
		SetAnimatorComponent();
	}

	private void SetButtonComponent()
	{
		CommonButton commonButton = base.gameObject.AddComponent<CommonButton>();
		commonButton.transition = Selectable.Transition.Animation;
	}

	private void SetAnimatorComponent()
	{
		Animator animator = base.gameObject.AddComponent<Animator>();
		animator.runtimeAnimatorController = Object.Instantiate(SingletonMonoBehaviour<ButtonAnimeManager>.Instance.GetButtonAnime(buttonAnimeListIndex));
	}
}
