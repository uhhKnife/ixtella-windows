using UnityEngine;

public class HeartIcon : MonoBehaviour
{
	private readonly string HEART_ANIME_TRIGGER_NAME = "HeartIsOn";

	private bool isCompletedInitialize_;

	private void Start()
	{
		isCompletedInitialize_ = true;
	}

	public void SetFull()
	{
		Animator animator = GetComponent<Animator>();
		animator.SetBool(HEART_ANIME_TRIGGER_NAME, true);
	}

	public void SetEmpty()
	{
		Animator animator = GetComponent<Animator>();
		animator.SetBool(HEART_ANIME_TRIGGER_NAME, false);
	}

	public bool IsCompletedInitialize()
	{
		return isCompletedInitialize_;
	}
}
