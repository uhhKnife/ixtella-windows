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
		// [PC port] The "DebugMenu" scene isn't in the build. Create the debug objects in code instead.
		// They always exist (EventScheduler looks for DebugMenu whenever debug is on) and are
		// hidden unless Options > Debug is ON.
		PortDebug.CreateObjects();
	}
}
