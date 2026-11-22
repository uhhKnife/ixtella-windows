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
		// The DebugMenu scene isn't in the build, so create its objects here.
		PortDebug.CreateObjects();
	}
}
