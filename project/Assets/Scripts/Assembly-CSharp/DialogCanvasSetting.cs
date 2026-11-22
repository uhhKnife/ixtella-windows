using UnityEngine;

public class DialogCanvasSetting : MonoBehaviour
{
	private void Awake()
	{
		GetComponent<Canvas>().enabled = false;
	}
}
