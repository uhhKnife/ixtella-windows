using UnityEngine;
using UnityEngine.UI;

public class StatusUICommon : MonoBehaviour
{
	private readonly string HEART_AREA_NAME = "AreaIcon/AreaHorizontal";

	private HeartIcon[] heartIconList_;

	private bool isCompletedInitialize_;

	private void Start()
	{
		RegisterHeartIconObjects();
		isCompletedInitialize_ = true;
	}

	public void SetStatus(int value)
	{
		if (heartIconList_ == null)
		{
			return;
		}
		for (int i = 0; i < heartIconList_.Length; i++)
		{
			if (i < value)
			{
				heartIconList_[i].SetFull();
			}
			else
			{
				heartIconList_[i].SetEmpty();
			}
		}
	}

	public void SetStatusNameText(string textId)
	{
		GetComponentInChildren<Text>().text = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText(textId);
	}

	private void RegisterHeartIconObjects()
	{
		GameObject gameObject = base.transform.Find(HEART_AREA_NAME).gameObject;
		heartIconList_ = new HeartIcon[gameObject.transform.childCount];
		for (int i = 0; i < heartIconList_.Length; i++)
		{
			GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
			heartIconList_[i] = gameObject2.AddComponent<HeartIcon>();
		}
	}

	public bool IsEnabledToInitializeHeart()
	{
		if (!isCompletedInitialize_)
		{
			return false;
		}
		for (int i = 0; i < heartIconList_.Length; i++)
		{
			if (!heartIconList_[i].IsCompletedInitialize())
			{
				return false;
			}
		}
		return true;
	}
}
