using UnityEngine;

public class LetterController : MonoBehaviour
{
	private void Start()
	{
	}

	private void SetPlayerAlpha(float alpha)
	{
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		Color color = sprite.color;
		color.a = alpha;
		sprite.color = color;
	}

	public void OnClick()
	{
		GetComponent<Animator>().SetTrigger("open");
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "SetAllButtonInteractive", false);
	}
}
