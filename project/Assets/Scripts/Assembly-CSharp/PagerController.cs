using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PagerController : MonoBehaviour
{
	public enum ArrowType
	{
		LEFT = 0,
		RIGHT = 1,
		MAX = 2
	}

	public GameObject dotPrefab_;

	private readonly Dictionary<ArrowType, string> ARROW_NAME_LIST = new Dictionary<ArrowType, string>
	{
		{
			ArrowType.LEFT,
			"BtnPagerLeft"
		},
		{
			ArrowType.RIGHT,
			"BtnPagerRight"
		}
	};

	private readonly string DOT_PARENT_NAME = "AreaArrowDot/AreaDot/AreaHorizontal";

	private readonly string DOT_ANIMATOR_NAME = "PagerDot";

	private readonly string ARROW_PARENT_NAME = "AreaArrowDot/AreaArrow";

	private int totalPageNum_;

	private int currentPageIndex_;

	private Dictionary<ArrowType, GameObject> arrowObjectList_;

	private List<GameObject> pageDotList_;

	private void Start()
	{
		arrowObjectList_[ArrowType.LEFT].GetComponent<Button>().onClick.AddListener(OnClickLeftArrow);
		arrowObjectList_[ArrowType.RIGHT].GetComponent<Button>().onClick.AddListener(OnClickRightArrow);
		if (totalPageNum_ > 1)
		{
			SetDotAnimeActive(currentPageIndex_, true);
		}
	}

	private void Awake()
	{
		arrowObjectList_ = new Dictionary<ArrowType, GameObject>();
		GameObject gameObject = base.transform.Find(ARROW_PARENT_NAME).gameObject;
		foreach (KeyValuePair<ArrowType, string> item in ARROW_NAME_LIST)
		{
			arrowObjectList_.Add(item.Key, gameObject.transform.Find(item.Value).gameObject);
		}
	}

	private void SetTotalPageNum(int num)
	{
		totalPageNum_ = num;
		if (totalPageNum_ > 1)
		{
			pageDotList_ = new List<GameObject>();
			Transform parent = base.transform.Find(DOT_PARENT_NAME);
			for (int i = 0; i < totalPageNum_; i++)
			{
				pageDotList_.Add(Object.Instantiate(dotPrefab_, parent));
			}
		}
		else
		{
			SetArrowActive(ArrowType.LEFT, false);
			SetArrowActive(ArrowType.RIGHT, false);
		}
	}

	private void SetCurrentPageIndex(int index)
	{
		bool flag = totalPageNum_ > 1;
		if (flag)
		{
			SetDotAnimeActive(currentPageIndex_, false);
		}
		currentPageIndex_ = index;
		if (flag)
		{
			SetDotAnimeActive(currentPageIndex_, true);
			bool flag2 = currentPageIndex_ != 0;
			SetArrowActive(ArrowType.LEFT, flag2);
			bool flag3 = currentPageIndex_ + 1 != totalPageNum_;
			SetArrowActive(ArrowType.RIGHT, flag3);
		}
	}

	private void SetDotAnimeActive(int index, bool flag)
	{
		Animator component = pageDotList_[index].GetComponent<Animator>();
		component.SetBool(DOT_ANIMATOR_NAME, flag);
	}

	private void SetArrowActive(ArrowType arrowType, bool flag)
	{
		arrowObjectList_[arrowType].GetComponent<CommonButton>().enabled = flag;
		arrowObjectList_[arrowType].GetComponentInChildren<Image>().enabled = flag;
	}

	private void OnClickLeftArrow()
	{
		int nextPageIndex = currentPageIndex_ - 1;
		OnClickCommon(nextPageIndex);
	}

	private void OnClickRightArrow()
	{
		int nextPageIndex = currentPageIndex_ + 1;
		OnClickCommon(nextPageIndex);
	}

	private void OnClickCommon(int nextPageIndex)
	{
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlaySE", SeController.SeType.SE_SELECT_BUTTON);
		base.transform.parent.SendMessage("SetCurrentPageIndex", nextPageIndex);
	}
}
