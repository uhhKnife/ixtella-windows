using UnityEngine;

public class MessageSender : MonoBehaviour
{
	public static void Send(string target, string method)
	{
		GameObject receiver = FindObject(target);
		if (receiver != null)
		{
			receiver.SendMessage(method);
		}
	}

	public static void Send<T>(string target, string method, T arg)
	{
		GameObject receiver = FindObject(target);
		if (receiver != null)
		{
			receiver.SendMessage(method, arg);
		}
	}

	public static void Send<T>(string target, string method, params T[] args)
	{
		GameObject receiver = FindObject(target);
		if (receiver != null)
		{
			receiver.SendMessage(method, args);
		}
	}

	public static void Send(string target, string method, params object[] args)
	{
		GameObject receiver = FindObject(target);
		if (receiver != null)
		{
			receiver.SendMessage(method, args);
		}
	}

	public static void SendWithTag(string tag, string method)
	{
		GameObject receiver = FindObjectWithTag(tag);
		if (receiver != null)
		{
			receiver.SendMessage(method);
		}
	}

	public static void SendWithTag<T>(string target, string method, T arg)
	{
		GameObject receiver = FindObjectWithTag(target);
		if (receiver != null)
		{
			receiver.SendMessage(method, arg);
		}
	}

	public static void SendWithTag(string tag, string method, params object[] args)
	{
		GameObject receiver = FindObjectWithTag(tag);
		if (receiver != null)
		{
			receiver.SendMessage(method, args);
		}
	}

	public static void SendWithTagMulti(string tag, string method)
	{
		GameObject[] receivers = FindObjectsWithTag(tag);
		for (int i = 0; i < receivers.Length; i++)
		{
			if (receivers[i] != null)
			{
				receivers[i].SendMessage(method);
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
		GameObject found = GameObject.Find(target);
		if (found == null)
		{
			Debug.LogWarning("[Message Sender] object was not found. -> name : " + target);
		}
		return found;
	}

	private static GameObject FindObjectWithTag(string tag)
	{
		GameObject found = GameObject.FindGameObjectWithTag(tag);
		if (found == null)
		{
			Debug.LogWarning("[Message Sender] object was not found. -> tag : " + tag);
		}
		return found;
	}

	private static GameObject[] FindObjectsWithTag(string tag)
	{
		GameObject[] found = GameObject.FindGameObjectsWithTag(tag);
		if (found == null)
		{
			Debug.LogWarning("[Message Sender] object was not found. -> tag : " + tag);
		}
		return found;
	}
}
