using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DebugPlayableTimeChecker : MonoBehaviour
{
	private readonly string SAVE_KEY = "DebugPlayableTimeCheckFlag";

	private readonly string CACHE_FILE_NAME = "playable_data.bin";

	private readonly string ENCODE_UTF_8 = "utf-8";

	private bool isCompletedInitialize_;

	private void Start()
	{
		GetComponent<Toggle>().isOn = SaveData.GetInt(SAVE_KEY) == 1;
		isCompletedInitialize_ = true;
	}

	public void SaveTimeCheckFlag()
	{
		if (isCompletedInitialize_)
		{
			bool isOn = GetComponent<Toggle>().isOn;
			SaveData.SetInt(SAVE_KEY, isOn ? 1 : 0);
			if (isOn)
			{
				ClearCacheData();
			}
		}
	}

	private void ClearCacheData()
	{
		string path = Application.temporaryCachePath + "\\" + CACHE_FILE_NAME;
		using (BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(path), Encoding.GetEncoding(ENCODE_UTF_8)))
		{
			binaryWriter.Write(DateTime.MinValue.ToBinary());
			binaryWriter.Write(false);
			Debug.Log("Clear Cache Data");
			binaryWriter.Close();
		}
	}
}
