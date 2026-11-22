using UnityEngine;

public class TitleBgSpriteFader : TitleBgFaderBase<TitleBgSpriteFader>
{
	private readonly string SPRITE_RENDERER_OBJECT_NAME = "ScreenBgBack/ImgBack";

	private SpriteRenderer spriteRenderer_;

	protected override void ApplyColor()
	{
		SetSpriteRendererAlpha(ref spriteRenderer_, GetFaderAlpha());
	}

	protected override void SetCanvasActive(bool flag)
	{
		GetComponent<Canvas>().enabled = flag;
	}

	protected override void RegisterFadeObjects()
	{
		base.RegisterFadeObjects();
		GameObject spriteObject = transform.Find(SPRITE_RENDERER_OBJECT_NAME).gameObject;
		spriteRenderer_ = spriteObject.GetComponent<SpriteRenderer>();
	}
}
