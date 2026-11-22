using UnityEngine;

public class HeartIcon : MonoBehaviour
{
	private readonly string HEART_IMAGE_NAME = "ImgHeart";

	private readonly string HEART_ANIME_TRIGGER_NAME = "HeartIsOn";

	private bool isCompletedInitialize_;

	private void Start()
	{
		GameObject gameObject = base.transform.Find(HEART_IMAGE_NAME).gameObject;
		isCompletedInitialize_ = true;
	}

	public void SetFull()
	{
		Animator component = GetComponent<Animator>();
		component.SetBool(HEART_ANIME_TRIGGER_NAME, true);
	}

	public void SetEmpty()
	{
		Animator component = GetComponent<Animator>();
		component.SetBool(HEART_ANIME_TRIGGER_NAME, false);
	}

	public bool IsCompletedInitialize()
	{
		return isCompletedInitialize_;
	}
}
