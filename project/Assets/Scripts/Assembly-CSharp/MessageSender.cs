using UnityEngine;

public class MessageSender : MonoBehaviour
{
	public static void Send(string target, string method)
	{
		GameObject gameObject = FindObject(target);
		if (gameObject != null)
		{
			gameObject.SendMessage(method);
		}
	}

	public static void Send<T>(string target, string method, T arg)
	{
		GameObject gameObject = FindObject(target);
		if (gameObject != null)
		{
			gameObject.SendMessage(method, arg);
		}
	}

	public static void Send<T>(string target, string method, params T[] args)
	{
		GameObject gameObject = FindObject(target);
		if (gameObject != null)
		{
			gameObject.SendMessage(method, args);
		}
	}

	public static void Send(string target, string method, params object[] args)
	{
		GameObject gameObject = FindObject(target);
		if (gameObject != null)
		{
			gameObject.SendMessage(method, args);
		}
	}

	public static void SendWithTag(string tag, string method)
	{
		GameObject gameObject = FindObjectWithTag(tag);
		if (gameObject != null)
		{
			gameObject.SendMessage(method);
		}
	}

	public static void SendWithTag<T>(string target, string method, T arg)
	{
		GameObject gameObject = FindObjectWithTag(target);
		if (gameObject != null)
		{
			gameObject.SendMessage(method, arg);
		}
	}

	public static void SendWithTag(string tag, string method, params object[] args)
	{
		GameObject gameObject = FindObjectWithTag(tag);
		if (gameObject != null)
		{
			gameObject.SendMessage(method, args);
		}
	}

	public static void SendWithTagMulti(string tag, string method)
	{
		GameObject[] array = FindObjectsWithTag(tag);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				array[i].SendMessage(method);
			}
		}
	}

	public static void SendWithObject<T>(GameObject target, string method, T arg)
	{
		if (target != null)
		{
			target.SendMessage(method, arg);
		}
	}

	public static void SendWithObject(GameObject target, string method, params object[] arg)
	{
		if (target != null)
		{
			target.SendMessage(method, arg);
		}
	}

	private static GameObject FindObject(string target)
	{
		GameObject gameObject = GameObject.Find(target);
		if (gameObject == null)
		{
			Debug.LogWarning("[Message Sender] object was not found. -> name : " + target);
		}
		return gameObject;
	}

	private static GameObject FindObjectWithTag(string tag)
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag(tag);
		if (gameObject == null)
		{
			Debug.LogWarning("[Message Sender] object was not found. -> tag : " + tag);
		}
		return gameObject;
	}

	private static GameObject[] FindObjectsWithTag(string tag)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(tag);
		if (array == null)
		{
			Debug.LogWarning("[Message Sender] object was not found. -> tag : " + tag);
		}
		return array;
	}
}
