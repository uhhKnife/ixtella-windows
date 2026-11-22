using UnityEngine;

public class FpsObserber : MonoBehaviour
{
	public float interval_ = 0.5f;

	private int frame_;

	private float oldTime_;

	private float frameRate_;

	public float FrameRate
	{
		get
		{
			return frameRate_;
		}
	}

	private void Awake()
	{
		oldTime_ = Time.realtimeSinceStartup;
	}

	private void Start()
	{
	}

	private void Update()
	{
		frame_++;
		float num = Time.realtimeSinceStartup - oldTime_;
		if (!(num < interval_))
		{
			frameRate_ = (float)frame_ / num;
			oldTime_ = Time.realtimeSinceStartup;
			frame_ = 0;
		}
	}
}
