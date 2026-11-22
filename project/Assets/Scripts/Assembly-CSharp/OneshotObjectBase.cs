using System.Collections;
using UnityEngine;

public abstract class OneshotObjectBase : MonoBehaviour
{
	public float lifeTime_ = 5f;

	private float currentLife_;

	protected string identify_;

	private SpriteMask spriteMask_;

	private string maskSpriteName_;

	private bool isDestroyed;

	protected virtual void Start()
	{
		RegisterChild();
		InvokeLife();
		currentLife_ = lifeTime_;
	}

	protected virtual void Update()
	{
		if (IsOneShot())
		{
			currentLife_ -= Time.deltaTime;
		}
	}

	protected void LateUpdate()
	{
		if (IsSpriteChanged())
		{
			UpdateMaskSprite();
		}
	}

	protected void Awake()
	{
		SetupSpriteMask();
	}

	public void InvokeLife()
	{
		if (IsOneShot())
		{
			Invoke("OnEnd", lifeTime_);
		}
	}

	private void OverWriteIdentify(string identify)
	{
		identify_ = identify;
	}

	protected virtual void OnEndStatusFlucCallback()
	{
	}

	protected virtual void OnEndAnimationCallback()
	{
		DestroyObject();
	}

	public virtual void OnEnd()
	{
		OnEndAnimationCallback();
	}

	protected virtual void DestroyObject()
	{
		isDestroyed = true;
		Object.Destroy(base.gameObject);
	}

	protected virtual void DestroyObjectDirect()
	{
		if (!isDestroyed)
		{
			OnEndStatusFlucCallback();
			isDestroyed = true;
		}
		Object.Destroy(base.gameObject);
	}

	private void SetLifeTime(float life)
	{
		lifeTime_ = life;
		InvokeLife();
	}

	private void SetPlayerAlpha(float alpha)
	{
		SpriteRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<SpriteRenderer>(true);
		SpriteRenderer[] array = componentsInChildren;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			Color color = spriteRenderer.color;
			color.a = alpha;
			spriteRenderer.color = color;
		}
	}

	private void SetupSpriteMask()
	{
		spriteMask_ = base.gameObject.AddComponent<SpriteMask>();
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		SpriteRenderer component2 = GameObject.FindWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER)).GetComponent<SpriteRenderer>();
		if (!(component == null) && !(component2 == null))
		{
			spriteMask_.isCustomRangeActive = true;
			spriteMask_.sprite = component.sprite;
			maskSpriteName_ = spriteMask_.sprite.name;
			spriteMask_.frontSortingLayerID = component2.sortingLayerID;
			spriteMask_.backSortingLayerID = component2.sortingLayerID;
			spriteMask_.frontSortingOrder = component2.sortingOrder;
			spriteMask_.backSortingOrder = 0;
		}
	}

	private void UpdateMaskSprite()
	{
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		if (!(component == null))
		{
			spriteMask_.sprite = component.sprite;
			maskSpriteName_ = spriteMask_.sprite.name;
		}
	}

	private void RegisterChild()
	{
		GameObject gameObject = GameObject.FindWithTag("Player");
		base.gameObject.transform.SetParent(gameObject.transform, false);
	}

	private IEnumerator InvokeWithDelay(float time)
	{
		yield return new WaitForSeconds(time);
		OnEnd();
	}

	private bool IsOneShot()
	{
		return lifeTime_ > 0f;
	}

	private bool IsSpriteChanged()
	{
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		if (component == null)
		{
			return false;
		}
		return component.sprite.name != maskSpriteName_;
	}

	private void OnGUI()
	{
	}
}
