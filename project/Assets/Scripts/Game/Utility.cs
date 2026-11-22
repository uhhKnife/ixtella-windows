using UnityEngine;

public class Utility
{
	public static void PrintLog(string str)
	{
		if (Debug.isDebugBuild)
		{
			Debug.Log(str);
		}
	}

	public static void PrintLog<T>(string str, T arg)
	{
		if (Debug.isDebugBuild)
		{
			Debug.Log(str + arg);
		}
	}

	public static int GetPlusOrMinus()
	{
		return Random.Range(0, 2) * 2 - 1;
	}

	public static bool SuccessRandom(int probability)
	{
		int roll = Random.Range(0, 100);
		return roll < probability;
	}

	public static int ConvertHMStoSec(int h, int m, int s)
	{
		return h * 60 * 60 + m * 60 + s;
	}
}
