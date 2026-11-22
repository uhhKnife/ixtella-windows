using UnityEngine.Events;
using UnityEngine.UI;

public class MenuCommonButton : ButtonBase
{
	public new void SetButtonMainText(string textId)
	{
		base.SetButtonMainText(textId);
	}

	public new void SetOnClickFunction(UnityAction function)
	{
		base.SetOnClickFunction(function);
	}

	public void SetVisible(bool flag)
	{
		gameObject.SetActive(flag);
	}

	public new void PlaySelectSE()
	{
		base.PlaySelectSE();
	}

	public void SetInteractable(bool flag)
	{
		Button button = GetComponent<Button>();
		button.interactable = flag;
	}
}
