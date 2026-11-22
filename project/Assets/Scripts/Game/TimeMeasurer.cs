using UnityEngine;

public class TimeMeasurer : MonoBehaviour
{
	private float timeLimit_;

	private float timerCount_;

	private bool isMeasuring_;

	public void StartMesuaring(float timeLimit)
	{
		timeLimit_ = timeLimit;
		timerCount_ = 0f;
		isMeasuring_ = true;
	}

	public void ResetTimer()
	{
		timeLimit_ = 0f;
		timerCount_ = 0f;
		isMeasuring_ = false;
	}

	public bool IsMeasuring()
	{
		return isMeasuring_;
	}

	public bool IsEnd()
	{
		return timerCount_ >= timeLimit_;
	}

	public float GetTimerCount()
	{
		return timerCount_;
	}

	public void UpdateTime()
	{
		if (timerCount_ < timeLimit_)
		{
			timerCount_ += Time.deltaTime;
		}
	}
}
