using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/UVScrollAndTiling")]
public class UVScrollAndTiling : MonoBehaviour
{
	[Range(-3f, 3f)]
	public float speedX;

	[Range(-3f, 3f)]
	public float speedY;

	private RawImage rawImage_;

	private Texture tex_;

	private void Start()
	{
		rawImage_ = GetComponent<RawImage>();
		Rect rect = rawImage_.rectTransform.rect;
		tex_ = rawImage_.texture;
		float width = rect.width / (float)tex_.width;
		float height = rect.height / (float)tex_.height;
		rawImage_.uvRect = new Rect(0f, 0f, width, height);
	}

	private void Update()
	{
		float x = Mathf.Repeat(Time.time * speedX, 1f);
		float y = Mathf.Repeat(Time.time * speedY, 1f);
		Rect rect = rawImage_.rectTransform.rect;
		float width = rect.width / (float)tex_.width;
		float height = rect.height / (float)tex_.height;
		rawImage_.uvRect = new Rect(x, y, width, height);
	}
}
