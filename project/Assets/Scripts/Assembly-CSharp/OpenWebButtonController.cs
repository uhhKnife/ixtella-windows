using UnityEngine;

public class OpenWebButtonController : MonoBehaviour
{
	public void OnClick(string webPageUrl)
	{
		if (string.IsNullOrEmpty(webPageUrl))
		{
			Debug.Log("[OpenWebButtonController] Error: Need to set web page url");
		}
		else
		{
			Application.OpenURL(webPageUrl);
		}
	}
}
