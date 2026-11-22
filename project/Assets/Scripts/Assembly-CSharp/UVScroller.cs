using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/UVScroll")]
public class UVScroller : MonoBehaviour
{
	[Range(0f, 3f)]
	public float speedX;

	[Range(0f, 3f)]
	public float speedY;

	private void Start()
	{
		GetComponent<RawImage>().uvRect = new Rect(0f, 0f, 1f, 1f);
	}

	private void Update()
	{
		float x = Mathf.Repeat(Time.time * speedX, 1f);
		float y = Mathf.Repeat(Time.time * speedY, 1f);
		GetComponent<RawImage>().uvRect = new Rect(x, y, 1f, 1f);
	}
}
