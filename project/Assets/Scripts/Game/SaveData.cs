using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveData
{
	[Serializable]
	private class SaveDataBase
	{
		private string path;

		private string fileName;

		private Dictionary<string, string> saveDictionary;

		public string Path
		{
			get
			{
				return path;
			}
			set
			{
				path = value;
			}
		}

		public string FileName
		{
			get
			{
				return fileName;
			}
			set
			{
				fileName = value;
			}
		}

		public SaveDataBase(string _path, string _fileName)
		{
			path = _path;
			fileName = _fileName;
			saveDictionary = new Dictionary<string, string>();
			Load();
		}

		public void SetList<T>(string key, List<T> list)
		{
			keyCheck(key);
			saveDictionary[key] = JsonUtility.ToJson(new Serialization<T>(list));
		}

		public List<T> GetList<T>(string key, List<T> _default)
		{
			if (saveDictionary.Count == 0)
			{
				return _default;
			}
			keyCheck(key);
			if (!saveDictionary.ContainsKey(key))
			{
				return _default;
			}
			string json = saveDictionary[key];
			Serialization<T> serialization = JsonUtility.FromJson<Serialization<T>>(json);
			return serialization.ToList();
		}

		public T GetClass<T>(string key, T _default) where T : class, new()
		{
			if (saveDictionary.Count == 0)
			{
				return _default;
			}
			keyCheck(key);
			if (!saveDictionary.ContainsKey(key))
			{
				return _default;
			}
			string json = saveDictionary[key];
			return JsonUtility.FromJson<T>(json);
		}

		public void SetClass<T>(string key, T obj) where T : class, new()
		{
			keyCheck(key);
			string value = JsonUtility.ToJson(obj);
			saveDictionary[key] = value;
		}

		public void SetString(string key, string value)
		{
			keyCheck(key);
			saveDictionary[key] = value;
		}

		public string GetString(string key, string _default)
		{
			if (saveDictionary.Count == 0)
			{
				return _default;
			}
			keyCheck(key);
			if (!saveDictionary.ContainsKey(key))
			{
				return _default;
			}
			return saveDictionary[key];
		}

		public void SetInt(string key, int value)
		{
			keyCheck(key);
			saveDictionary[key] = value.ToString();
		}

		public int GetInt(string key, int _default)
		{
			if (saveDictionary.Count == 0)
			{
				return _default;
			}
			keyCheck(key);
			if (!saveDictionary.ContainsKey(key))
			{
				return _default;
			}
			int result;
			if (!int.TryParse(saveDictionary[key], out result))
			{
				return 0;
			}
			return result;
		}

		public void SetFloat(string key, float value)
		{
			keyCheck(key);
			saveDictionary[key] = value.ToString();
		}

		public float GetFloat(string key, float _default)
		{
			if (saveDictionary.Count == 0)
			{
				return _default;
			}
			keyCheck(key);
			if (!saveDictionary.ContainsKey(key))
			{
				return _default;
			}
			float result;
			if (!float.TryParse(saveDictionary[key], out result))
			{
				return 0f;
			}
			return result;
		}

		public void Clear()
		{
			saveDictionary.Clear();
		}

		public void Remove(string key)
		{
			keyCheck(key);
			if (saveDictionary.ContainsKey(key))
			{
				saveDictionary.Remove(key);
			}
		}

		public bool ContainsKey(string _key)
		{
			return saveDictionary.ContainsKey(_key);
		}

		public List<string> Keys()
		{
			return saveDictionary.Keys.ToList();
		}

		public void Save()
		{
			Serialization<string, string> serialization = new Serialization<string, string>(saveDictionary);
			serialization.OnBeforeSerialize();
			string encrypted = GameDataCrypter.Encrypt(JsonUtility.ToJson(serialization));
			Debug.Log("Data Saved.");
			using (BinaryWriter writer = new BinaryWriter(new FileStream(path + fileName, FileMode.Create, FileAccess.Write)))
			{
				writer.Write(encrypted);
			}
		}

		public void Load()
		{
			if (File.Exists(path + fileName))
			{
				string json;
				using (BinaryReader reader = new BinaryReader(new FileStream(path + fileName, FileMode.Open, FileAccess.Read)))
				{
					json = GameDataCrypter.Decrypt(reader.ReadString());
				}
				Debug.Log("Data Loaded.");
				Serialization<string, string> serialization = JsonUtility.FromJson<Serialization<string, string>>(json);
				serialization.OnAfterDeserialize();
				saveDictionary = serialization.ToDictionary();
			}
			else
			{
				Debug.Log("Fileが存在しません。");
				Debug.Log("新しくセーブデータを作成します。");
				saveDictionary = new Dictionary<string, string>();
			}
		}

		public string GetJsonString(string key)
		{
			keyCheck(key);
			if (saveDictionary.ContainsKey(key))
			{
				return saveDictionary[key];
			}
			return null;
		}

		private void keyCheck(string _key)
		{
			if (string.IsNullOrEmpty(_key))
			{
				throw new ArgumentException("invalid key!!");
			}
		}
	}

	[Serializable]
	private class Serialization<T>
	{
		public List<T> target;

		public Serialization()
		{
		}

		public Serialization(List<T> target)
		{
			this.target = target;
		}

		public List<T> ToList()
		{
			return target;
		}
	}

	[Serializable]
	private class Serialization<TKey, TValue>
	{
		public List<TKey> keys;

		public List<TValue> values;

		private Dictionary<TKey, TValue> target;

		public Serialization()
		{
		}

		public Serialization(Dictionary<TKey, TValue> target)
		{
			this.target = target;
		}

		public Dictionary<TKey, TValue> ToDictionary()
		{
			return target;
		}

		public void OnBeforeSerialize()
		{
			keys = new List<TKey>(target.Keys);
			values = new List<TValue>(target.Values);
		}

		public void OnAfterDeserialize()
		{
			int count = Math.Min(keys.Count, values.Count);
			target = new Dictionary<TKey, TValue>(count);
			for (int i = 0; i < count; i++)
			{
				target.Add(keys[i], values[i]);
			}
		}
	}

	private static SaveDataBase savedatabase;

	private static SaveDataBase Savedatabase
	{
		get
		{
			if (savedatabase == null)
			{
				string path = Application.persistentDataPath + "//";
				string fileName = "FixlSavedata.json";
				savedatabase = new SaveDataBase(path, fileName);
			}
			return savedatabase;
		}
	}

	private SaveData()
	{
	}

	public static void SetList<T>(string key, List<T> list)
	{
		Savedatabase.SetList(key, list);
	}

	public static List<T> GetList<T>(string key, List<T> _default)
	{
		return Savedatabase.GetList(key, _default);
	}

	public static T GetClass<T>(string key, T _default) where T : class, new()
	{
		return Savedatabase.GetClass(key, _default);
	}

	public static void SetClass<T>(string key, T obj) where T : class, new()
	{
		Savedatabase.SetClass(key, obj);
	}

	public static void SetString(string key, string value)
	{
		Savedatabase.SetString(key, value);
	}

	public static string GetString(string key, string _default = "")
	{
		return Savedatabase.GetString(key, _default);
	}

	public static void SetInt(string key, int value)
	{
		Savedatabase.SetInt(key, value);
	}

	public static int GetInt(string key, int _default = 0)
	{
		return Savedatabase.GetInt(key, _default);
	}

	public static void SetFloat(string key, float value)
	{
		Savedatabase.SetFloat(key, value);
	}

	public static float GetFloat(string key, float _default = 0f)
	{
		return Savedatabase.GetFloat(key, _default);
	}

	public static void Clear()
	{
		Savedatabase.Clear();
	}

	public static void Remove(string key)
	{
		Savedatabase.Remove(key);
	}

	public static bool ContainsKey(string _key)
	{
		return Savedatabase.ContainsKey(_key);
	}

	public static List<string> Keys()
	{
		return Savedatabase.Keys();
	}

	public static void Save()
	{
		Savedatabase.Save();
	}
}
