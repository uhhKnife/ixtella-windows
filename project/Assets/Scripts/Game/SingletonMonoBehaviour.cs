using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T instance;

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (T)Object.FindAnyObjectByType(typeof(T));
				if (instance == null)
				{
					Debug.LogError(string.Concat(typeof(T), "is nothing"));
				}
			}
			return instance;
		}
	}
}
