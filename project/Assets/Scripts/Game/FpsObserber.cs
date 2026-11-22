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

	private void Update()
	{
		frame_++;
		float elapsed = Time.realtimeSinceStartup - oldTime_;
		if (elapsed >= interval_)
		{
			frameRate_ = (float)frame_ / elapsed;
			oldTime_ = Time.realtimeSinceStartup;
			frame_ = 0;
		}
	}
}
