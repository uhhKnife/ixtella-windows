using UnityEngine;

public class StaticCanvas : SingletonMonoBehaviour<StaticCanvas>
{
	private void Awake()
	{
		if (this != SingletonMonoBehaviour<StaticCanvas>.Instance)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}
}
