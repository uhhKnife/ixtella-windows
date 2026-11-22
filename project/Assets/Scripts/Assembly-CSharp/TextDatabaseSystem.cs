using System;
using System.Collections.Generic;
using UnityEngine;

public class TextDatabaseSystem : SingletonMonoBehaviour<TextDatabaseSystem>
{
	private Dictionary<string, string> textDataList_;

	private readonly string TEXT_FILE_NAME = "ixtella_system_textlist";

	private readonly string SERVANT_NAME_TEXT_ID_COMMON = "SYS_servant_name_";

	private void Awake()
	{
		if (this != SingletonMonoBehaviour<TextDatabaseSystem>.Instance)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		LoadTextDatabase();
	}

	private void LoadTextDatabase()
	{
		TextAsset textAsset = Resources.Load(TEXT_FILE_NAME) as TextAsset;
		if (textAsset == null)
		{
			Debug.LogError("[" + GetType().ToString() + "] cannot load " + TEXT_FILE_NAME);
			return;
		}
		string[] array = textAsset.text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
		textDataList_ = new Dictionary<string, string>();
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			string text = string.Empty;
			for (int j = 1; j < array2.Length; j++)
			{
				text += array2[j];
			}
			text = text.Replace("<br>", "\n");
			textDataList_.Add(array2[0], text);
		}
		Resources.UnloadAsset(textAsset);
	}

	public string GetText(string textId)
	{
		if (textDataList_.ContainsKey(textId))
		{
			return textDataList_[textId];
		}
		Debug.LogError("textId = " + textId + " does not exist");
		return null;
	}

	public bool ExistsText(string textId)
	{
		return textDataList_.ContainsKey(textId);
	}

	public string GetServantNameText(Define.CharaId charaId)
	{
		string textId = SERVANT_NAME_TEXT_ID_COMMON + Define.GetCharaName(charaId);
		return GetText(textId);
	}
}
