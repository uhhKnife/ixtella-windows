using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticObjectsManager : SingletonMonoBehaviour<StaticObjectsManager>
{
	private void Awake()
	{
		if (this != SingletonMonoBehaviour<StaticObjectsManager>.Instance)
		{
			Object.Destroy(gameObject);
			return;
		}
		Object.DontDestroyOnLoad(gameObject);
		// The DebugMenu scene isn't in the build, so create its objects here.
		PortDebug.CreateObjects();
	}
}
