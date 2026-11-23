using UnityEngine;

public class CharacterBGController : MonoBehaviour
{
	public GameObject cosGetBgPrefab_;

	private SpriteRenderer spriteRenderer_;

	private GameObject cosGetBgHandler_;

	private readonly string MALE_BG_NAME = "bg_01";

	private readonly string FEMALE_BG_NAME = "bg_02";

	private readonly Color DEFAULT_COLOR = new Color(1f, 1f, 1f);

	private readonly Color SLEEPING_COLOR = new Color(150f / 255f, 165f / 255f, 195f / 255f);

	private void Start()
	{
		spriteRenderer_ = GetComponent<SpriteRenderer>();
		ChangeBackground(SingletonMonoBehaviour<GameController>.Instance.GetPlayerGender());
	}

	private void ChangeBackground(PlayerStatusManager.Gender gender)
	{
		string path = ((gender != PlayerStatusManager.Gender.GENDER_MALE) ? FEMALE_BG_NAME : MALE_BG_NAME);
		Sprite sprite = SpriteLoader.Load(path);
		spriteRenderer_.sprite = sprite;
		if (SingletonMonoBehaviour<PlayerStatusManager>.Instance != null)
		{
			bool isTurnOff = SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsSleeping();
			ChangeSleepingColor(isTurnOff);
		}
		Resources.UnloadUnusedAssets();
	}

	private void ChangeSleepingColor(bool isTurnOff)
	{
		if (spriteRenderer_ != null)
		{
			spriteRenderer_.color = (isTurnOff ? SLEEPING_COLOR : DEFAULT_COLOR);
			if (SingletonMonoBehaviour<PlayerController>.Instance != null)
			{
				float playerAlpha = SingletonMonoBehaviour<PlayerController>.Instance.GetPlayerAlpha();
				Color color = spriteRenderer_.color;
				color.a = playerAlpha;
				spriteRenderer_.color = color;
			}
		}
	}

	private void CreateCostumeGetBackground(float alpha)
	{
		cosGetBgHandler_ = Object.Instantiate(cosGetBgPrefab_);
		cosGetBgHandler_.transform.SetParent(transform.root, false);
		SpriteRenderer sprite = cosGetBgHandler_.GetComponent<SpriteRenderer>();
		Color color = sprite.color;
		color.a = alpha;
		sprite.color = color;
	}

	private void DestroyCostumeGetBackground()
	{
		if (cosGetBgHandler_ != null)
		{
			Object.Destroy(cosGetBgHandler_);
			cosGetBgHandler_ = null;
		}
	}
}
