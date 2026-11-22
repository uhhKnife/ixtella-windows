using UnityEngine;

public class TimeMeasurer : MonoBehaviour
{
	private float timeLimit_;

	private float timerCount_;

	private bool isMeasuring_;

	private void Update()
	{
	}

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
		if (timerCount_ >= timeLimit_)
		{
			return true;
		}
		return false;
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
