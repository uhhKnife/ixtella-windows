using UnityEngine;
using UnityEngine.UI;

public class MainButtonsController : MonoBehaviour
{
	public enum MainButtonType
	{
		MEAL = 0,
		PLAY = 1,
		DIAPER = 2,
		SLEEP = 3,
		TREATMENT = 4,
		PARAMETER = 5,
		COSTUME = 6,
		TITLE = 7
	}

	public enum TitleButtonType
	{
		OPTION = 0,
		BANNER_LINK = 1,
		BANNER_IXTELLA = 2
	}

	[SerializeField]
	private GameObject[] buttonObjectList_;

	private void SetAllButtonInteractive(bool flag)
	{
		for (int i = 0; i < buttonObjectList_.Length; i++)
		{
			buttonObjectList_[i].GetComponent<Button>().interactable = flag;
		}
	}

	private void SetButtonInteractive(object[] args)
	{
		int buttonIndex = (int)args[0];
		bool flag = (bool)args[1];
		SetSingleButtonInteractive(buttonIndex, flag);
	}

	private void SetSingleButtonInteractive(int buttonIndex, bool flag)
	{
		if (0 <= buttonIndex && buttonIndex < buttonObjectList_.Length)
		{
			buttonObjectList_[buttonIndex].GetComponent<Button>().interactable = flag;
			return;
		}
		Debug.LogError("[" + GetType().ToString() + "] Error: invalid button index " + buttonIndex);
	}

	private void SetButtonActive(object[] args)
	{
		int buttonIndex = (int)args[0];
		bool flag = (bool)args[1];
		SetSingleButtonActive(buttonIndex, flag);
	}

	private void SetAllButtonActive(bool flag)
	{
		for (int i = 0; i < buttonObjectList_.Length; i++)
		{
			SetSingleButtonActive(i, flag);
		}
	}

	private void SetSingleButtonActive(int buttonIndex, bool flag)
	{
		if (0 <= buttonIndex && buttonIndex < buttonObjectList_.Length)
		{
			buttonObjectList_[buttonIndex].GetComponent<Button>().enabled = flag;
			Image[] componentsInChildren = buttonObjectList_[buttonIndex].GetComponentsInChildren<Image>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = flag;
			}
		}
		else
		{
			Debug.LogError("[" + GetType().ToString() + "] Error: invalid button index " + buttonIndex);
		}
	}

	private void SetAllButtonAnimeNormal()
	{
		for (int i = 0; i < buttonObjectList_.Length; i++)
		{
			CommonButton component = buttonObjectList_[i].GetComponent<CommonButton>();
			if (component != null)
			{
				component.SetAnimeNormal();
			}
		}
	}

	private void SetButtonsGameOverState()
	{
		SetAllButtonInteractive(false);
		SetSingleButtonInteractive(5, true);
		SetSingleButtonInteractive(7, true);
	}

	private void SetButtonsSleepState()
	{
		SetButtonsGameOverState();
		SetSingleButtonInteractive(6, true);
	}
}
