using UnityEngine;

public class LetterController : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void SetPlayerAlpha(float alpha)
	{
		SpriteRenderer component = base.gameObject.GetComponent<SpriteRenderer>();
		Color color = component.color;
		color.a = alpha;
		component.color = color;
	}

	public void OnClick()
	{
		GetComponent<Animator>().SetTrigger("open");
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "SetAllButtonInteractive", false);
	}
}
