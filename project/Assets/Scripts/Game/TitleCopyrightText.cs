using UnityEngine;
using UnityEngine.UI;

public class TitleCopyrightText : MonoBehaviour
{
	private readonly string COPYRIGHT_TEXT_ID = "SYS_MENU_0-1_copyright";

	private void Start()
	{
		GetComponent<Text>().text = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(COPYRIGHT_TEXT_ID);
	}
}
