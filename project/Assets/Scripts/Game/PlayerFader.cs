using System.Collections.Generic;
using UnityEngine;

public class PlayerFader : FaderBase<PlayerFader>
{
	private List<GameObject> playerObjectList_;

	protected override void Initialize()
	{
		RegisterFadeObjects();
		base.Initialize();
	}

	protected override void ApplyColor()
	{
		if (playerObjectList_.Count != transform.childCount)
		{
			RegisterFadeObjects();
		}
		for (int i = 0; i < playerObjectList_.Count; i++)
		{
			SpriteRenderer spriteRenderer = playerObjectList_[i].GetComponent<SpriteRenderer>();
			SetSpriteRendererAlpha(ref spriteRenderer, GetFaderAlpha());
			SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>(true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				SetSpriteRendererAlpha(ref componentsInChildren[j], GetFaderAlpha());
			}
		}
	}

	protected override void RegisterFadeObjects()
	{
		base.RegisterFadeObjects();
		playerObjectList_ = new List<GameObject>();
		for (int i = 0; i < transform.childCount; i++)
		{
			playerObjectList_.Add(transform.GetChild(i).gameObject);
		}
	}

	protected override void SetCanvasActive(bool flag)
	{
		RegisterFadeObjects();
		for (int i = 0; i < playerObjectList_.Count; i++)
		{
			BoxCollider2D boxCollider = playerObjectList_[i].GetComponent<BoxCollider2D>();
			if (boxCollider != null)
			{
				boxCollider.enabled = flag;
			}
		}
	}

	protected override float GetInitialAlpha()
	{
		return 0f;
	}

	protected override bool GetInitialCanvasActiveFlag()
	{
		return Define.IsActiveScene(Define.SceneId.SCENE_MAIN);
	}
}
