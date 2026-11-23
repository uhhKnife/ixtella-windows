using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SwipeCharacterController : SwipeObjectBase
{
	public GameObject charaPrefab_;

	public GameObject framePrefab_;

	public GameObject newIconPrefab_;

	public Sprite maleBgSprite_;

	public Sprite femaleBgSprite_;

	public Sprite maleQuestionSprite_;

	public Sprite femaleQuestionSprite_;

	private readonly string CHARA_PARENT_OBJECT_NAME = "AreaGrid";

	private readonly string CHARA_BG_OBJECT_NAME = "BgChara";

	private readonly string CHARA_IMAGE_OBJECT_NAME = "ImgChara";

	private readonly int PAGE_MAX_CHARA_NUM = 6;

	private Dictionary<Define.CharaId, GameObject> charaObjectList_;

	private Dictionary<Define.CharaId, GameObject> newIconObjectList_;

	private GameObject frameObject_;

	private bool isHiddenLockedChara_;

	private bool isShownSelectingFrame_;

	private bool isShownNewIcon_;

	private bool isSelectableLlockedChara_ = true;

	private int characterNum_;

	protected override void Awake()
	{
		base.Awake();
		charaObjectList_ = new Dictionary<Define.CharaId, GameObject>();
		newIconObjectList_ = new Dictionary<Define.CharaId, GameObject>();
	}

	private void OnDestroy()
	{
		foreach (GameObject chara in charaObjectList_.Values)
		{
			Object.Destroy(chara);
		}
		foreach (GameObject newIcon in newIconObjectList_.Values)
		{
			Object.Destroy(newIcon);
		}
		if (frameObject_ != null)
		{
			Object.Destroy(frameObject_);
		}
	}

	public void SetTotalCharacterNum(int num)
	{
		int totalPageNum = (num - 1) / PAGE_MAX_CHARA_NUM + 1;
		SetTotalPageNum(totalPageNum);
	}

	public void AddCharacter(Define.CharaId charaId, UnityAction<Define.CharaId> onClickFunction)
	{
		int pageIndex = characterNum_ / PAGE_MAX_CHARA_NUM;
		if (0 <= pageIndex && pageIndex < GetPageObjectList().Count)
		{
			GameObject charaParent = GetPageObjectList()[pageIndex].transform.Find(CHARA_PARENT_OBJECT_NAME).gameObject;
			GameObject charaObject = Object.Instantiate(charaPrefab_, charaParent.transform);
			charaObjectList_.Add(charaId, charaObject);
			CommonButton button = charaObject.GetComponent<CommonButton>();
			button.onClick.AddListener(() =>
			{
				onClickFunction(charaId);
			});
			button.SetUseWarningSEFlag(false);
			button.interactable = isSelectableLlockedChara_ || !isLockedChara(charaId);
			charaObject.SetActive(true);
			SetCharacterIcons(ref charaObject, charaId);
			if (isShownNewIcon_ && isLockedChara(charaId))
			{
				GameObject newIcon = Object.Instantiate(newIconPrefab_, charaObject.transform);
				newIconObjectList_.Add(charaId, newIcon);
				SetNewActive(charaId, true);
			}
			if (isShownSelectingFrame_ && charaId == SingletonMonoBehaviour<PlayerStatusManager>.Instance.GetCurrentCharaId())
			{
				SetFrame(charaId);
			}
		}
		else
		{
			Debug.LogError("[" + GetType().ToString() + "] Error: invalid page Index " + pageIndex);
		}
		characterNum_++;
	}

	protected void SetCharacterIcons(ref GameObject charaObject, Define.CharaId charaId)
	{
		SetCharacterImage(charaObject, charaId);
		SetCharacterBgImage(ref charaObject, charaId);
	}

	private void SetCharacterImage(GameObject charaObject, Define.CharaId charaId)
	{
		GameObject imageObject = charaObject.transform.Find(CHARA_IMAGE_OBJECT_NAME).gameObject;
		Image image = imageObject.GetComponent<Image>();
		if (isHiddenLockedChara_ && !Define.IsMasterId(charaId) && isLockedChara(charaId))
		{
			image.sprite = (Define.IsMaleCharacter(charaId) ? maleQuestionSprite_ : femaleQuestionSprite_);
		}
		else
		{
			image.sprite = SpriteLoader.Load(MenuCommonData.GetCharaSpriteFilename(charaId));
		}
	}

	private void SetCharacterBgImage(ref GameObject charaObject, Define.CharaId charaId)
	{
		GameObject bgObject = charaObject.transform.Find(CHARA_BG_OBJECT_NAME).gameObject;
		Image image = bgObject.GetComponent<Image>();
		if (Define.IsMaleCharacter(charaId))
		{
			image.sprite = maleBgSprite_;
		}
		else
		{
			image.sprite = femaleBgSprite_;
		}
	}

	public int GetPageMaxCharacterNum()
	{
		return PAGE_MAX_CHARA_NUM;
	}

	public void SetHideLockedCharaFlag(bool flag)
	{
		isHiddenLockedChara_ = flag;
	}

	public void SetShowSelectingFrameFlag(bool flag)
	{
		isShownSelectingFrame_ = flag;
		if (isShownSelectingFrame_)
		{
			frameObject_ = Object.Instantiate(framePrefab_);
			frameObject_.SetActive(false);
		}
	}

	public void SetShowNewIconFlag(bool flag)
	{
		isShownNewIcon_ = flag;
	}

	public void SetLockedCharaSelectableFlag(bool flag)
	{
		isSelectableLlockedChara_ = flag;
	}

	public void SetFrame(Define.CharaId charaId)
	{
		frameObject_.transform.SetParent(charaObjectList_[charaId].transform, false);
		frameObject_.SetActive(true);
	}

	public void SetNewActive(Define.CharaId charaId, bool flag)
	{
		newIconObjectList_[charaId].SetActive(flag);
	}

	private bool isLockedChara(Define.CharaId charaId)
	{
		if (Define.IsMasterId(charaId))
		{
			return false;
		}
		GalleryData.CostumeUnlockData costumeUnlockData = SaveData.GetClass(GalleryData.GetUnlockSaveDataKey(charaId), new GalleryData.CostumeUnlockData());
		return costumeUnlockData.GetUnlockFlag() == GalleryData.UnlockFlag.FLAG_LOCKED;
	}
}
