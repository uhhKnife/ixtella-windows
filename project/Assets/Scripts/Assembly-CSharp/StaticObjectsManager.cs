using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticObjectsManager : SingletonMonoBehaviour<StaticObjectsManager>
{
	private void Awake()
	{
		if (this != SingletonMonoBehaviour<StaticObjectsManager>.Instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Object.DontDestroyOnLoad(base.gameObject);
		if (Debug.isDebugBuild)
		{
			SceneManager.LoadScene("DebugMenu", LoadSceneMode.Additive);
		}
	}
}
