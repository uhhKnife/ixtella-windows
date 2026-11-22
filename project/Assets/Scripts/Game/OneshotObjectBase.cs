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
		Object.Destroy(gameObject);
	}

	protected virtual void DestroyObjectDirect()
	{
		if (!isDestroyed)
		{
			OnEndStatusFlucCallback();
			isDestroyed = true;
		}
		Object.Destroy(gameObject);
	}

	private void SetLifeTime(float life)
	{
		lifeTime_ = life;
		InvokeLife();
	}

	private void SetPlayerAlpha(float alpha)
	{
		SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>(true);
		foreach (SpriteRenderer spriteRenderer in componentsInChildren)
		{
			Color color = spriteRenderer.color;
			color.a = alpha;
			spriteRenderer.color = color;
		}
	}

	private void SetupSpriteMask()
	{
		spriteMask_ = gameObject.AddComponent<SpriteMask>();
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		SpriteRenderer player = GameObject.FindWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER)).GetComponent<SpriteRenderer>();
		if (sprite != null && player != null)
		{
			spriteMask_.isCustomRangeActive = true;
			spriteMask_.sprite = sprite.sprite;
			maskSpriteName_ = spriteMask_.sprite.name;
			spriteMask_.frontSortingLayerID = player.sortingLayerID;
			spriteMask_.backSortingLayerID = player.sortingLayerID;
			spriteMask_.frontSortingOrder = player.sortingOrder;
			spriteMask_.backSortingOrder = 0;
		}
	}

	private void UpdateMaskSprite()
	{
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		if (sprite != null)
		{
			spriteMask_.sprite = sprite.sprite;
			maskSpriteName_ = spriteMask_.sprite.name;
		}
	}

	private void RegisterChild()
	{
		GameObject player = GameObject.FindWithTag("Player");
		transform.SetParent(player.transform, false);
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
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		if (sprite == null)
		{
			return false;
		}
		return sprite.sprite.name != maskSpriteName_;
	}

	private void OnGUI()
	{
	}
}
