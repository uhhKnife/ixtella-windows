using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AppliPlayableStateChecker : MonoBehaviour
{
	private enum DownloadDataType
	{
		YEAR = 0,
		MONTH = 1,
		DAY = 2,
		HOUR = 3,
		MINUTE = 4,
		PLAYABLE_FLAG = 5,
		DATA_MAX = 6
	}

	private enum CheckResultType
	{
		NONE = 0,
		BEFORE_PLAYABLE_TIME = 1,
		CONNECTION_ERROR = 2,
		DOWNLOAD_ERROR = 3,
		SUCCESS = 4
	}

	public class PlayableData
	{
		private DateTime time_;

		private bool flag_;

		public PlayableData()
		{
			clear();
		}

		private void clear()
		{
			time_ = DateTime.MinValue;
			flag_ = false;
		}

		public DateTime GetTime()
		{
			return time_;
		}

		public bool GetFlag()
		{
			return flag_;
		}

		public void SetTime(DateTime time)
		{
			time_ = time;
		}

		public void SetFlag(bool flag)
		{
			flag_ = flag;
		}
	}

	private readonly Dictionary<DownloadDataType, int> DOWNLOAD_BINARY_SIZE = new Dictionary<DownloadDataType, int>
	{
		{
			DownloadDataType.YEAR,
			2
		},
		{
			DownloadDataType.MONTH,
			1
		},
		{
			DownloadDataType.DAY,
			1
		},
		{
			DownloadDataType.HOUR,
			1
		},
		{
			DownloadDataType.MINUTE,
			1
		},
		{
			DownloadDataType.PLAYABLE_FLAG,
			1
		}
	};

	private readonly Dictionary<CheckResultType, string> ERROR_TEXT_ID = new Dictionary<CheckResultType, string>
	{
		{
			CheckResultType.BEFORE_PLAYABLE_TIME,
			"SYS_MENU_0_dialog_date_error"
		},
		{
			CheckResultType.CONNECTION_ERROR,
			"SYS_MENU_0_dialog_connection_error"
		},
		{
			CheckResultType.DOWNLOAD_ERROR,
			"SYS_MENU_0_dialog_download_error"
		}
	};

	private readonly string ENCODE_UTF_8 = "utf-8";

	private readonly int DIALOG_OPEN_WAIT_FRAME_NUM = 3;

	private readonly string DOWNLOAD_FILE_URL = "https://api-ixt-play.marv-games.jp/api/ixt/play/ixtella_playable_data.bin";

	private readonly string CACHE_FILE_NAME = "playable_data.bin";

	private string cacheFilePath_;

	private PlayableData officialPlayableData_ = new PlayableData();

	private PlayableData cachePlayableData_ = new PlayableData();

	private CheckResultType resultType_;

	private readonly string DOWNLOAD_FILE_URL_DEBUG = "https://api-ixt-play.marv-games.jp/api/ixt/play/ixtella_playable_data_debug.bin";

	private void Awake()
	{
		cacheFilePath_ = Application.temporaryCachePath + "\\" + CACHE_FILE_NAME;
	}

	private void StartPlayableStateCheck()
	{
		StartCoroutine(CheckAppliPlayableState());
	}

	private IEnumerator CheckAppliPlayableState()
	{
		resultType_ = CheckResultType.NONE;
		ReadCachePlayableData();
		if (GetPlayableFlagOnCache())
		{
			resultType_ = CheckResultType.SUCCESS;
		}
		else if (ExistsPlayableDateOnCache())
		{
			if (IsPassedPlayableDate())
			{
				yield return StartCoroutine(CheckOfficialPlayableFlag());
			}
			else
			{
				resultType_ = CheckResultType.BEFORE_PLAYABLE_TIME;
			}
		}
		else
		{
			yield return StartCoroutine(CheckOfficialPlayableFlag());
		}
		ReflectCheckResult();
	}

	private IEnumerator CheckOfficialPlayableFlag()
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_FADE_CONTROLLER), "SetMessageType", LoadingFader.LoadMessageType.CONNECTING);
		yield return StartCoroutine(DownloadUnlockDataFile());
		if (resultType_ == CheckResultType.NONE)
		{
			if (officialPlayableData_.GetFlag())
			{
				WriteCachePlayableData();
				resultType_ = CheckResultType.SUCCESS;
			}
			else
			{
				WriteCachePlayableData();
				resultType_ = CheckResultType.BEFORE_PLAYABLE_TIME;
			}
		}
	}

	private IEnumerator DownloadUnlockDataFile()
	{
		UnityWebRequest www = UnityWebRequest.Get(DOWNLOAD_FILE_URL);
		if (IsDebugCheck() && !IsPassedPlayableDate())
		{
			www = UnityWebRequest.Get(DOWNLOAD_FILE_URL_DEBUG);
		}
		yield return www.SendWebRequest();
		if (www.isHttpError || www.isNetworkError)
		{
			Debug.LogError(www.error);
			resultType_ = CheckResultType.CONNECTION_ERROR;
		}
		else if (www.downloadHandler.data.Length == GetExpectedBinaryFileSize())
		{
			AnalyzeDownloadData(www.downloadHandler.data);
		}
		else
		{
			resultType_ = CheckResultType.DOWNLOAD_ERROR;
		}
	}

	private void ReflectCheckResult()
	{
		if (resultType_ == CheckResultType.NONE)
		{
			Debug.LogError("[" + GetType().ToString() + "] Error: check is failed");
		}
		if (resultType_ == CheckResultType.SUCCESS)
		{
			Debug.Log("Time Check is succeed");
			StartCoroutine(ControlFadeIn());
		}
		else
		{
			OpenErrorDialog(resultType_);
		}
	}

	private IEnumerator ControlFadeIn()
	{
		GameObject fader = GameObject.FindWithTag("Fader");
		LoadingFader fadeController = fader.GetComponent<LoadingFader>();
		while (!fadeController.EnablesFadeOut())
		{
			yield return null;
		}
		fader.SendMessage("StartFadeOut", LoadingFader.LoadMessageType.LOADING);
		if (MenuCommonData.IsAgreedUserPolicy())
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "StartFadeIn", 7u);
			MessageSender.SendWithTagMulti(Define.GetTagName(Define.TagId.TAG_TITLE_BG_CANVAS), "StartFadeIn");
		}
		else
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CONTROLLER), "OpenMenu", MenuCommonData.MenuType.MENU_USER_POLICY);
		}
	}

	private bool GetPlayableFlagOnCache()
	{
		// [PC port] The unlock server is gone; always treat the game as unlocked
		// (same change as the offline APK).
		return true;
	}

	private bool ExistsPlayableDateOnCache()
	{
		return !cachePlayableData_.GetTime().Equals(DateTime.MinValue);
	}

	private bool IsPassedPlayableDate()
	{
		if (cachePlayableData_.GetTime().Equals(DateTime.MinValue))
		{
			return false;
		}
		return DateTime.Now.CompareTo(cachePlayableData_.GetTime()) >= 0;
	}

	private void OpenErrorDialog(CheckResultType errorType)
	{
		Debug.Log("Open " + errorType.ToString() + " dialog");
		StartCoroutine(ControlOpenErrorDialog(errorType));
	}

	private IEnumerator ControlOpenErrorDialog(CheckResultType errorType)
	{
		DialogCommonData.SetCanvasComponentActive(base.gameObject.scene.name, true);
		yield return DIALOG_OPEN_WAIT_FRAME_NUM;
		ResourceRequest prefabAsync = Resources.LoadAsync(DialogCommonData.GetDialogPrefabName(DialogCommonData.DialogType.DIALOG_MESSAGE));
		while (!prefabAsync.isDone)
		{
			yield return null;
		}
		GameObject dialogObject = DialogCommonData.InstantiateDialog(prefabAsync, base.gameObject.scene.name);
		dialogObject.SetActive(true);
		PlayableCheckErrorDialog errorDialog = dialogObject.AddComponent<PlayableCheckErrorDialog>();
		errorDialog.SetBodyTextId(ERROR_TEXT_ID[errorType]);
		DialogCommonData.SetAnimeTriggerFlag(base.gameObject.scene.name, true);
	}

	private int GetExpectedBinaryFileSize()
	{
		int num = 0;
		foreach (int value in DOWNLOAD_BINARY_SIZE.Values)
		{
			num += value;
		}
		return num;
	}

	private void WriteCachePlayableData()
	{
		if (officialPlayableData_.GetTime().Equals(DateTime.MinValue))
		{
			return;
		}
		using (BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(cacheFilePath_), Encoding.GetEncoding(ENCODE_UTF_8)))
		{
			binaryWriter.Write(officialPlayableData_.GetTime().ToBinary());
			binaryWriter.Write(officialPlayableData_.GetFlag());
			Debug.Log(string.Concat("Save Cache Data: ", officialPlayableData_.GetTime(), ", ", officialPlayableData_.GetFlag()));
			binaryWriter.Close();
		}
	}

	private void ReadCachePlayableData()
	{
		if (File.Exists(cacheFilePath_))
		{
			using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(cacheFilePath_)))
			{
				DateTime time = DateTime.FromBinary(binaryReader.ReadInt64());
				cachePlayableData_.SetTime(time);
				bool flag = binaryReader.ReadBoolean();
				cachePlayableData_.SetFlag(flag);
				Debug.Log(string.Concat("Read Cache Data: ", cachePlayableData_.GetTime(), ", ", cachePlayableData_.GetFlag()));
				binaryReader.Close();
				return;
			}
		}
		cachePlayableData_.SetFlag(false);
	}

	private void AnalyzeDownloadData(byte[] downloadFile)
	{
		Dictionary<DownloadDataType, int> dictionary = new Dictionary<DownloadDataType, int>();
		int num = 0;
		foreach (DownloadDataType key in DOWNLOAD_BINARY_SIZE.Keys)
		{
			switch (key)
			{
			case DownloadDataType.YEAR:
				dictionary.Add(key, BitConverter.ToInt16(downloadFile, num));
				break;
			case DownloadDataType.MONTH:
			case DownloadDataType.DAY:
			case DownloadDataType.HOUR:
			case DownloadDataType.MINUTE:
				dictionary.Add(key, downloadFile[num]);
				break;
			case DownloadDataType.PLAYABLE_FLAG:
				officialPlayableData_.SetFlag(BitConverter.ToBoolean(downloadFile, num));
				break;
			}
			num += DOWNLOAD_BINARY_SIZE[key];
		}
		officialPlayableData_.SetTime(new DateTime(dictionary[DownloadDataType.YEAR], dictionary[DownloadDataType.MONTH], dictionary[DownloadDataType.DAY], dictionary[DownloadDataType.HOUR], dictionary[DownloadDataType.MINUTE], 0));
		Debug.Log("Read Official Data: PlayableTime = " + officialPlayableData_.GetTime().ToString() + " PlayableFlag = " + officialPlayableData_.GetFlag());
	}

	private bool GetDebugTimeCheckFlag()
	{
		return SaveData.GetInt("DebugPlayableTimeCheckFlag") == 1;
	}

	private bool IsDebugCheck()
	{
		return Debug.isDebugBuild && GetDebugTimeCheckFlag();
	}
}
