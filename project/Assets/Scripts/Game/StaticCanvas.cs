using UnityEngine;

public class StaticCanvas : SingletonMonoBehaviour<StaticCanvas>
{
	private void Awake()
	{
		if (this != SingletonMonoBehaviour<StaticCanvas>.Instance)
		{
			Object.Destroy(gameObject);
		}
		else
		{
			Object.DontDestroyOnLoad(gameObject);
		}
	}
}
