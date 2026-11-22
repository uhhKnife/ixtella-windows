using System.Collections.Generic;
using UnityEngine;

public class PlayerController : SingletonMonoBehaviour<PlayerController>
{
	public enum PlayerActionType
	{
		ACT_DEFAULT = 0,
		ACT_HAPPINESS = 1,
		ACT_GOOD_FEEL = 2,
		ACT_COMPLAIN = 3,
		ACT_COMPLAIN_1SHOT = 4,
		ACT_REFUSE = 5,
		ACT_CALLING = 6,
		ACT_CALLING_WITH_COMPLAIN = 7,
		ACT_SICK = 8,
		ACT_CURE = 9,
		ACT_DRINK_MILK = 10,
		ACT_EAT_SNACK = 11,
		ACT_EAT_SPECIAL = 12,
		ACT_PLAY_BALL = 13,
		ACT_READ_BOOK = 14,
		ACT_SING = 15,
		ACT_SLEEP = 16,
		ACT_DIAPER = 17,
		ACT_ACHIEVE = 18,
		ACT_WAVE_HAND = 19,
		ACT_TYPE_MAX = 20
	}

	public enum PlayerAnimationType
	{
		ANIM_IDLE = 0,
		ANIM_IDLE_NOMOVE = 1,
		ANIM_HAPPINESS = 2,
		ANIM_COMPLAIN = 3,
		ANIM_COMPLAIN_1SHOT = 4,
		ANIM_REFUSE = 5,
		ANIM_DRINK = 6,
		ANIM_DRINK_COMPLAIN = 7,
		ANIM_DRINK_REFUSE = 8,
		ANIM_EAT_GOOD = 9,
		ANIM_EAT_BAD = 10,
		ANIM_WAVE_HAND = 11,
		ANIM_WAVE_HAND_JOY = 12,
		ANIM_SLEEP = 13,
		ANIM_BALL = 14,
		ANIM_BOOK = 15,
		ANIM_SING = 16,
		ANIM_ACHIEVE = 17,
		ANIM_TYPE_MAX = 18
	}

	public enum EmoteIconType
	{
		EMOTE_CALLING = 0,
		EMOTE_COMPLAIN = 1,
		EMOTE_HAPPINESS = 2,
		EMOTE_SICK = 3,
		EMOTE_SLEEP = 4,
		EMOTE_TWINKLE = 5,
		EMOTE_CURE = 6,
		EMOTE_ACHIEVE = 7,
		EMOTE_TYPE_MAX = 8
	}

	public enum ItemIconType
	{
		ITEM_MILK = 0,
		ITEM_SNACK = 1,
		ITEM_SPECIAL_NERO = 2,
		ITEM_SPECIAL_TAMAMO = 3,
		ITEM_SPECIAL_ELIZA = 4,
		ITEM_BALL = 5,
		ITEM_BOOK = 6,
		ITEM_NOTE = 7,
		ITEM_COVERLET = 8,
		ITEM_DIAPER = 9,
		ITEM_TYPE_MAX = 10
	}

	private enum ActorState
	{
		STATE_NORMAL = 0,
		STATE_EATING = 1,
		STATE_SICK = 2,
		STATE_SLEEP = 3,
		STATE_MAX = 4
	}

	[EnumLabel(typeof(EmoteIconType))]
	public GameObject[] emoteIcons_;

	[EnumLabel(typeof(ItemIconType))]
	public GameObject[] itemIcons_;

	private static readonly float MOVE_INTERVAL = 1f;

	private static readonly float MOVEMENT_X = 15f;

	private static readonly float MOVEMENT_Y = 20f;

	private static readonly float MOVE_AREA_MIN_X = -70f;

	private static readonly float MOVE_AREA_MAX_X = 70f;

	private readonly string GINGLE_FILE_NAME = "commandSing";

	private int reactionCount_;

	private Dictionary<PlayerAnimationType, string> animationTrrigers = new Dictionary<PlayerAnimationType, string>
	{
		{
			PlayerAnimationType.ANIM_IDLE,
			"idle"
		},
		{
			PlayerAnimationType.ANIM_IDLE_NOMOVE,
			"idle_nomove"
		},
		{
			PlayerAnimationType.ANIM_HAPPINESS,
			"happiness"
		},
		{
			PlayerAnimationType.ANIM_COMPLAIN,
			"complain"
		},
		{
			PlayerAnimationType.ANIM_COMPLAIN_1SHOT,
			"complain_1shot"
		},
		{
			PlayerAnimationType.ANIM_REFUSE,
			"refuse"
		},
		{
			PlayerAnimationType.ANIM_DRINK,
			"drink"
		},
		{
			PlayerAnimationType.ANIM_DRINK_COMPLAIN,
			"drink_complain"
		},
		{
			PlayerAnimationType.ANIM_DRINK_REFUSE,
			"drink_refuse"
		},
		{
			PlayerAnimationType.ANIM_EAT_GOOD,
			"eat_good"
		},
		{
			PlayerAnimationType.ANIM_EAT_BAD,
			"eat_bad"
		},
		{
			PlayerAnimationType.ANIM_WAVE_HAND,
			"wave_hand"
		},
		{
			PlayerAnimationType.ANIM_WAVE_HAND_JOY,
			"wave_hand_joy"
		},
		{
			PlayerAnimationType.ANIM_SLEEP,
			"sleep"
		},
		{
			PlayerAnimationType.ANIM_BALL,
			"ball"
		},
		{
			PlayerAnimationType.ANIM_BOOK,
			"book"
		},
		{
			PlayerAnimationType.ANIM_SING,
			"sing"
		},
		{
			PlayerAnimationType.ANIM_ACHIEVE,
			"achieve"
		}
	};

	private float moveTimer_;

	private ActorState state_;

	private Animator animator_;

	public Vector3 costumeGetPos_ = Vector3.zero;

	private Vector3 defaultPos_ = Vector3.zero;

	private Vector3 oldPosition_ = Vector3.zero;

	private SpriteRenderer spriteRenderer_;

	private SpriteRenderer eyeSpriteRenderer_;

	private static int idMainTex = Shader.PropertyToID("_MainTex");

	private MaterialPropertyBlock block_;

	private MaterialPropertyBlock eyeBlock_;

	private bool mooveable_ = true;

	private bool clickable_ = true;

	private void Awake()
	{
		if (this != SingletonMonoBehaviour<PlayerController>.Instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		moveTimer_ = MOVE_INTERVAL;
		state_ = ActorState.STATE_NORMAL;
		animator_ = GetComponent<Animator>();
		spriteRenderer_ = GetComponent<SpriteRenderer>();
		block_ = new MaterialPropertyBlock();
		spriteRenderer_.GetPropertyBlock(block_);
		eyeSpriteRenderer_ = base.transform.GetChild(0).GetComponent<SpriteRenderer>();
		eyeBlock_ = new MaterialPropertyBlock();
		eyeSpriteRenderer_.GetPropertyBlock(eyeBlock_);
	}

	private void Start()
	{
		SingletonMonoBehaviour<GameController>.Instance.registerPlayer(base.gameObject);
		defaultPos_ = base.transform.localPosition;
		oldPosition_ = base.transform.localPosition;
		ResetReactionCount();
		SetPlayerAlpha(0f);
	}

	private void Update()
	{
		if (state_ == ActorState.STATE_NORMAL)
		{
			Move();
		}
	}

	private void LateUpdate()
	{
		spriteRenderer_.SetPropertyBlock(block_);
		eyeSpriteRenderer_.SetPropertyBlock(eyeBlock_);
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
		if (SingletonMonoBehaviour<GameController>.Instance.IsGameOver())
		{
			GameObject gameObject = GameObject.FindWithTag("Letter");
			if ((bool)gameObject)
			{
				gameObject.SendMessage("SetPlayerAlpha", alpha);
			}
		}
	}

	public void OnClick()
	{
		if (CanClick())
		{
			oldPosition_ = base.transform.localPosition;
			Vector3 localPosition = oldPosition_;
			localPosition.y = Mathf.RoundToInt(localPosition.y + MOVEMENT_Y);
			base.transform.localPosition = localPosition;
			PlayAction(PlayerActionType.ACT_WAVE_HAND);
			Invoke("EndReaction", 0.1f);
		}
	}

	private void EndReaction()
	{
		oldPosition_.y = Mathf.RoundToInt(defaultPos_.y);
		base.transform.localPosition = oldPosition_;
	}

	private void Move()
	{
		if (CanMove() && UpdateMoveTimer())
		{
			Vector2 position = GetComponent<SpriteRenderer>().transform.localPosition;
			position.x += MOVEMENT_X * (float)Utility.GetPlusOrMinus();
			position.x = Mathf.Clamp(position.x, MOVE_AREA_MIN_X, MOVE_AREA_MAX_X);
			position.y = defaultPos_.y;
			SetPosition(position);
		}
	}

	private bool UpdateMoveTimer()
	{
		moveTimer_ -= Time.deltaTime;
		if (moveTimer_ < 0f)
		{
			moveTimer_ = MOVE_INTERVAL;
			return true;
		}
		return false;
	}

	private void SetPosition(Vector2 pos)
	{
		pos.x = Mathf.RoundToInt(pos.x);
		pos.y = Mathf.RoundToInt(pos.y);
		GetComponent<SpriteRenderer>().transform.localPosition = pos;
	}

	private void PlayAnimation(PlayerAnimationType type)
	{
		if (type >= PlayerAnimationType.ANIM_IDLE && type < PlayerAnimationType.ANIM_TYPE_MAX)
		{
			string trigger = animationTrrigers[type];
			animator_.SetTrigger(trigger);
		}
	}

	private void PlayEmotion(EmoteIconType type, float lifeTime = -1f)
	{
		if (type < EmoteIconType.EMOTE_CALLING || type >= EmoteIconType.EMOTE_TYPE_MAX)
		{
			return;
		}
		if (emoteIcons_.Length > (int)type)
		{
			GameObject gameObject = Object.Instantiate(emoteIcons_[(int)type], emoteIcons_[(int)type].transform.position, Quaternion.identity);
			gameObject.SendMessage("RegisterChild", GetPlayerAlpha());
			gameObject.SendMessage("SetPlayerAlpha", GetPlayerAlpha());
			if (lifeTime > 0f)
			{
				gameObject.SendMessage("SetLifeTime", lifeTime);
			}
		}
	}

	private void PlayFood(ItemIconType type, bool isSuccess)
	{
		if (type < ItemIconType.ITEM_MILK || type >= ItemIconType.ITEM_BALL)
		{
			return;
		}
		if (itemIcons_.Length > (int)type)
		{
			GameObject gameObject = Object.Instantiate(itemIcons_[(int)type], itemIcons_[(int)type].transform.position, Quaternion.identity);
			gameObject.SendMessage("SetSeccess", isSuccess);
			gameObject.SendMessage("SetPlayerAlpha", GetPlayerAlpha());
			if (type == ItemIconType.ITEM_SPECIAL_NERO)
			{
				gameObject.SendMessage("OverWriteIdentify", "SPECIAL_NERO");
			}
			if (type == ItemIconType.ITEM_SPECIAL_TAMAMO)
			{
				gameObject.SendMessage("OverWriteIdentify", "SPECIAL_TAMAMO");
			}
			if (type == ItemIconType.ITEM_SPECIAL_ELIZA)
			{
				gameObject.SendMessage("OverWriteIdentify", "SPECIAL_ELIZA");
			}
		}
	}

	private void PlayItem(ItemIconType type)
	{
		if (type >= ItemIconType.ITEM_BALL && type < ItemIconType.ITEM_TYPE_MAX)
		{
			if (itemIcons_.Length > (int)type)
			{
				GameObject gameObject = Object.Instantiate(itemIcons_[(int)type], itemIcons_[(int)type].transform.position, Quaternion.identity);
				gameObject.SendMessage("RegisterChild", GetPlayerAlpha());
				gameObject.SendMessage("SetPlayerAlpha", GetPlayerAlpha());
			}
		}
	}

	private bool CanOverWriteAnimation()
	{
		if (animator_.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("baby_m_base_sleep"))
		{
			Debug.LogWarning("[CanOverWriteAnimation] I'm sleeping now.");
			return false;
		}
		return true;
	}

	private void PlayAction(PlayerActionType type)
	{
		if (CanOverWriteAnimation())
		{
			ResetAnimation();
			SetMoveable(false);
			SetClickable(false);
			switch (type)
			{
			case PlayerActionType.ACT_DEFAULT:
				SetMoveable(true);
				SetClickable(true);
				PlayAnimation(PlayerAnimationType.ANIM_IDLE);
				break;
			case PlayerActionType.ACT_HAPPINESS:
				PlayAnimation(PlayerAnimationType.ANIM_HAPPINESS);
				PlayEmotion(EmoteIconType.EMOTE_HAPPINESS);
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlayMasterVoiceSe", SeController.SeType.SE_BABY_VOICE_JOY);
				break;
			case PlayerActionType.ACT_GOOD_FEEL:
				PlayAnimation(PlayerAnimationType.ANIM_HAPPINESS);
				PlayEmotion(EmoteIconType.EMOTE_HAPPINESS);
				PlayEmotion(EmoteIconType.EMOTE_TWINKLE);
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlayMasterVoiceSe", SeController.SeType.SE_BABY_VOICE_JOY);
				break;
			case PlayerActionType.ACT_COMPLAIN:
				SetMoveable(true);
				PlayAnimation(PlayerAnimationType.ANIM_COMPLAIN);
				PlayEmotion(EmoteIconType.EMOTE_COMPLAIN);
				break;
			case PlayerActionType.ACT_COMPLAIN_1SHOT:
				PlayAnimation(PlayerAnimationType.ANIM_COMPLAIN_1SHOT);
				PlayEmotion(EmoteIconType.EMOTE_COMPLAIN, 2.4f);
				break;
			case PlayerActionType.ACT_REFUSE:
				PlayRefuse();
				break;
			case PlayerActionType.ACT_CALLING:
				SetMoveable(true);
				PlayEmotion(EmoteIconType.EMOTE_CALLING);
				break;
			case PlayerActionType.ACT_CALLING_WITH_COMPLAIN:
				SetMoveable(true);
				PlayAnimation(PlayerAnimationType.ANIM_COMPLAIN);
				PlayEmotion(EmoteIconType.EMOTE_CALLING);
				break;
			case PlayerActionType.ACT_SICK:
				SetMoveable(true);
				PlayAnimation(PlayerAnimationType.ANIM_COMPLAIN);
				PlayEmotion(EmoteIconType.EMOTE_SICK);
				break;
			case PlayerActionType.ACT_CURE:
				CareCure();
				break;
			case PlayerActionType.ACT_DRINK_MILK:
				DrinkMilk();
				break;
			case PlayerActionType.ACT_EAT_SNACK:
				EatSnack();
				break;
			case PlayerActionType.ACT_EAT_SPECIAL:
				EatSpecial();
				break;
			case PlayerActionType.ACT_PLAY_BALL:
				PlayBall();
				break;
			case PlayerActionType.ACT_READ_BOOK:
				PlayBook();
				break;
			case PlayerActionType.ACT_SING:
				PlaySing();
				break;
			case PlayerActionType.ACT_SLEEP:
				PlaySleep();
				break;
			case PlayerActionType.ACT_DIAPER:
				CareDiaper();
				break;
			case PlayerActionType.ACT_ACHIEVE:
				AchieveCostume();
				break;
			case PlayerActionType.ACT_WAVE_HAND:
				WaveHand();
				break;
			}
		}
	}

	private void ResetAnimation()
	{
		for (int i = 1; i < base.gameObject.transform.childCount; i++)
		{
			base.gameObject.transform.GetChild(i).SendMessage("DestroyObjectDirect");
		}
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_CHARACTER_BACKGROUND), "DestroyCostumeGetBackground");
	}

	private void ResetPosition()
	{
		base.transform.localPosition = defaultPos_;
	}

	private void SetPerformPosition()
	{
		base.transform.localPosition = costumeGetPos_;
	}

	public float GetPlayerAlpha()
	{
		return base.gameObject.GetComponent<SpriteRenderer>().color.a;
	}

	private void ChangeCostume(Define.CharaId charaId)
	{
		string costumeId = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetCostumeId(charaId);
		Texture2D texture2D = Resources.Load(costumeId) as Texture2D;
		if (texture2D == null)
		{
			Debug.LogError("[ChangeCostume] costume texture is not found. id -> " + charaId);
			return;
		}
		block_.SetTexture(idMainTex, texture2D);
		ChangeBlinkTexture(charaId);
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "SetCharaId", charaId);
	}

	private void ChangeBlinkTexture(Define.CharaId id)
	{
		string path = ((!Define.IsMaleCharacter(id)) ? "baby_f_p_blink" : "baby_m_p_blink");
		Texture2D value = Resources.Load(path) as Texture2D;
		eyeBlock_.SetTexture(idMainTex, value);
	}

	private void SetBlinkable(bool flag)
	{
		base.gameObject.transform.GetChild(0).gameObject.SetActive(flag);
	}

	private void PlayRefuse()
	{
		PlayAnimation(PlayerAnimationType.ANIM_REFUSE);
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlayMasterVoiceSe", SeController.SeType.SE_BABY_VOICE_REFUSE);
	}

	private void DrinkMilk()
	{
		PlayerStatusManager component = base.gameObject.GetComponent<PlayerStatusManager>();
		if (component.IsStatusMax(PlayerStatusManager.PlayerStatusType.TYPE_SATIETY))
		{
			PlayRefuse();
			return;
		}
		ResetPosition();
		AnimatorStateInfo currentAnimatorStateInfo = animator_.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.shortNameHash == Animator.StringToHash("complain_1") || currentAnimatorStateInfo.shortNameHash == Animator.StringToHash("complain_2"))
		{
			PlayAnimation(PlayerAnimationType.ANIM_DRINK_COMPLAIN);
		}
		else if (currentAnimatorStateInfo.shortNameHash == Animator.StringToHash("refuse"))
		{
			PlayAnimation(PlayerAnimationType.ANIM_DRINK_REFUSE);
		}
		else
		{
			PlayAnimation(PlayerAnimationType.ANIM_DRINK);
		}
		PlayFood(ItemIconType.ITEM_MILK, true);
	}

	private void EatSnack()
	{
		PlayerStatusManager component = base.gameObject.GetComponent<PlayerStatusManager>();
		if (component.IsStatusMax(PlayerStatusManager.PlayerStatusType.TYPE_FEEL))
		{
			PlayRefuse();
			return;
		}
		ResetPosition();
		PlayAnimation(PlayerAnimationType.ANIM_EAT_GOOD);
		PlayFood(ItemIconType.ITEM_SNACK, true);
	}

	private void EatSpecial()
	{
		PlayerStatusManager component = base.gameObject.GetComponent<PlayerStatusManager>();
		if (component.IsStatusMax(PlayerStatusManager.PlayerStatusType.TYPE_SATIETY))
		{
			PlayRefuse();
			return;
		}
		ResetPosition();
		GrowthDatabase.CommandInfo commandInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindCommandInfo("SPECIAL_NERO");
		GrowthDatabase.CommandInfo commandInfo2 = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindCommandInfo("SPECIAL_TAMAMO");
		GrowthDatabase.CommandInfo commandInfo3 = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindCommandInfo("SPECIAL_ELIZA");
		int probabilityValue = commandInfo.GetProbabilityValue();
		int num = probabilityValue + commandInfo2.GetProbabilityValue();
		int num2 = num + commandInfo3.GetProbabilityValue();
		int num3 = Random.Range(0, 100);
		if (num3 < probabilityValue)
		{
			PlayAnimation(PlayerAnimationType.ANIM_EAT_GOOD);
			PlayFood(ItemIconType.ITEM_SPECIAL_NERO, true);
		}
		else if (num3 >= probabilityValue && num3 < num)
		{
			PlayAnimation(PlayerAnimationType.ANIM_EAT_GOOD);
			PlayFood(ItemIconType.ITEM_SPECIAL_TAMAMO, true);
		}
		else if (num3 >= num && num3 < num2)
		{
			PlayAnimation(PlayerAnimationType.ANIM_EAT_BAD);
			PlayFood(ItemIconType.ITEM_SPECIAL_ELIZA, false);
		}
		else
		{
			Debug.LogError("unknown food.");
		}
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetCookable", false);
	}

	private void PlaySleep()
	{
		if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsSleeping())
		{
			ResetPosition();
			PlayAnimation(PlayerAnimationType.ANIM_SLEEP);
			PlayEmotion(EmoteIconType.EMOTE_SLEEP);
			PlayItem(ItemIconType.ITEM_COVERLET);
		}
		else if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsSleepy())
		{
			MessageSender.SendWithObject(SingletonMonoBehaviour<PlayerStatusManager>.Instance.gameObject, "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SLEEPING, true);
		}
		else
		{
			PlayRefuse();
		}
	}

	private void PlayBall()
	{
		PlayerStatusManager component = base.gameObject.GetComponent<PlayerStatusManager>();
		if (component.IsStatusMax(PlayerStatusManager.PlayerStatusType.TYPE_FEEL))
		{
			PlayRefuse();
			return;
		}
		ResetPosition();
		PlayAnimation(PlayerAnimationType.ANIM_BALL);
		PlayItem(ItemIconType.ITEM_BALL);
	}

	private void PlayBook()
	{
		PlayerStatusManager component = base.gameObject.GetComponent<PlayerStatusManager>();
		if (component.IsStatusMax(PlayerStatusManager.PlayerStatusType.TYPE_FEEL))
		{
			PlayRefuse();
			return;
		}
		ResetPosition();
		PlayAnimation(PlayerAnimationType.ANIM_BOOK);
		PlayItem(ItemIconType.ITEM_BOOK);
	}

	private void PlaySing()
	{
		if (SingletonMonoBehaviour<GameController>.Instance.CanSing())
		{
			ResetPosition();
			PlayAnimation(PlayerAnimationType.ANIM_SING);
			PlayItem(ItemIconType.ITEM_NOTE);
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetSingable", false);
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_BGM_CONTROLLER), "RequestStartBgm", GINGLE_FILE_NAME);
		}
		else
		{
			PlayRefuse();
		}
	}

	private void AchieveCostume()
	{
		PlayAnimation(PlayerAnimationType.ANIM_ACHIEVE);
		PlayEmotion(EmoteIconType.EMOTE_ACHIEVE);
		SetPerformPosition();
		MessageSender.Send(Define.GetTagName(Define.TagId.TAG_CHARACTER_BACKGROUND), "CreateCostumeGetBackground", GetPlayerAlpha());
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlayMasterVoiceSe", SeController.SeType.SE_BABY_VOICE_JOY);
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "SetAllButtonInteractive", false);
	}

	public bool IsPlayingAchieve()
	{
		if (animator_.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("baby_m_base_achieve"))
		{
			return true;
		}
		return false;
	}

	private void CareCure()
	{
		if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsSick())
		{
			ResetPosition();
			PlayEmotion(EmoteIconType.EMOTE_CURE);
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SICK, false);
		}
		else
		{
			PlayRefuse();
		}
	}

	private void CareDiaper()
	{
		if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsDiaper())
		{
			ResetPosition();
			PlayAnimation(PlayerAnimationType.ANIM_COMPLAIN);
			PlayItem(ItemIconType.ITEM_DIAPER);
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER), "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_DIAPER, false);
		}
		else
		{
			PlayRefuse();
		}
	}

	private void AddSick()
	{
		PlayAnimation(PlayerAnimationType.ANIM_COMPLAIN);
		PlayEmotion(EmoteIconType.EMOTE_SICK);
	}

	private void WaveHand()
	{
		SetClickable(true);
		bool flag = false;
		reactionCount_--;
		if (reactionCount_ <= 0)
		{
			flag = true;
			ResetReactionCount();
		}
		PlayerAnimationType type = ((!flag) ? PlayerAnimationType.ANIM_WAVE_HAND : PlayerAnimationType.ANIM_WAVE_HAND_JOY);
		PlayAnimation(type);
		SeController.SeType arg = ((!flag) ? SeController.SeType.SE_TOUCH_CHARACTER : SeController.SeType.SE_BABY_VOICE_JOY);
		MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlayMasterVoiceSe", arg);
	}

	private void ResetReactionCount()
	{
		reactionCount_ = SingletonMonoBehaviour<GrowthDatabase>.Instance.GetWaveHandCount();
	}

	private void GameCreared()
	{
		HidePlayer();
	}

	private void GameOvered()
	{
		GameObject original = Resources.Load("Letter") as GameObject;
		GameObject gameObject = Object.Instantiate(original);
		gameObject.transform.SetParent(base.gameObject.transform.parent, false);
		gameObject.SendMessage("SetPlayerAlpha", GetPlayerAlpha());
		HidePlayer();
		if (!Define.IsActiveScene(Define.SceneId.SCENE_MAIN))
		{
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_PLAYER_CANVAS), "SetCanvasActive", false);
		}
	}

	private void HidePlayer()
	{
		SetPlayerAlpha(0f);
		base.gameObject.SetActive(false);
	}

	private void ResetGame()
	{
		moveTimer_ = MOVE_INTERVAL;
		state_ = ActorState.STATE_NORMAL;
		SetMoveable(true);
		SetClickable(true);
		ResetPosition();
		ResetAnimation();
		PlayAnimation(PlayerAnimationType.ANIM_IDLE);
		defaultPos_ = base.transform.localPosition;
		oldPosition_ = base.transform.localPosition;
		ResetReactionCount();
	}

	private void WakeUp()
	{
		if (SingletonMonoBehaviour<GameController>.Instance.IsGameClear())
		{
			GameClearJoy();
			return;
		}
		ResetAnimation();
		SetMoveable(true);
		SetClickable(true);
		PlayAnimation(PlayerAnimationType.ANIM_IDLE);
	}

	private void GameClearJoy()
	{
		ResetPosition();
		ResetAnimation();
		SetMoveable(false);
		SetClickable(false);
		if (Define.IsActiveScene(Define.SceneId.SCENE_MAIN))
		{
			if (!DialogCommonData.IsDialogActive(Define.GetSceneName(Define.SceneId.SCENE_MAIN)) && SingletonMonoBehaviour<MenuController>.Instance.GetCurrentMenuType() == MenuCommonData.MenuType.MENU_NONE)
			{
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlayMasterVoiceSe", SeController.SeType.SE_BABY_VOICE_JOY);
				PlayAnimation(PlayerAnimationType.ANIM_HAPPINESS);
				PlayEmotion(EmoteIconType.EMOTE_HAPPINESS);
				MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "SetAllButtonInteractive", false);
			}
			else
			{
				PlayAnimation(PlayerAnimationType.ANIM_IDLE);
			}
		}
	}

	private void OnFarewell()
	{
		SetMoveable(false);
		SetClickable(false);
		PlayAnimation(PlayerAnimationType.ANIM_IDLE_NOMOVE);
	}

	private void SetMoveable(bool flag)
	{
		mooveable_ = flag;
	}

	private void SetClickable(bool flag)
	{
		clickable_ = flag;
	}

	private bool CanMove()
	{
		return mooveable_;
	}

	private bool CanClick()
	{
		return clickable_;
	}

	private void DebugPlayAction(PlayerActionType type)
	{
		ResetAnimation();
		SetMoveable(false);
		SetClickable(false);
		switch (type)
		{
		case PlayerActionType.ACT_DEFAULT:
			SetMoveable(true);
			SetClickable(true);
			PlayAnimation(PlayerAnimationType.ANIM_IDLE);
			break;
		case PlayerActionType.ACT_HAPPINESS:
			PlayAnimation(PlayerAnimationType.ANIM_HAPPINESS);
			PlayEmotion(EmoteIconType.EMOTE_HAPPINESS);
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlayMasterVoiceSe", SeController.SeType.SE_BABY_VOICE_JOY);
			break;
		case PlayerActionType.ACT_GOOD_FEEL:
			PlayAnimation(PlayerAnimationType.ANIM_HAPPINESS);
			PlayEmotion(EmoteIconType.EMOTE_HAPPINESS);
			PlayEmotion(EmoteIconType.EMOTE_TWINKLE);
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlayMasterVoiceSe", SeController.SeType.SE_BABY_VOICE_JOY);
			break;
		case PlayerActionType.ACT_COMPLAIN:
			SetMoveable(true);
			PlayAnimation(PlayerAnimationType.ANIM_COMPLAIN);
			PlayEmotion(EmoteIconType.EMOTE_COMPLAIN);
			break;
		case PlayerActionType.ACT_COMPLAIN_1SHOT:
			PlayAnimation(PlayerAnimationType.ANIM_COMPLAIN_1SHOT);
			PlayEmotion(EmoteIconType.EMOTE_COMPLAIN, 2.4f);
			break;
		case PlayerActionType.ACT_REFUSE:
			PlayAnimation(PlayerAnimationType.ANIM_REFUSE);
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlayMasterVoiceSe", SeController.SeType.SE_BABY_VOICE_REFUSE);
			break;
		case PlayerActionType.ACT_CALLING:
			SetMoveable(true);
			PlayAnimation(PlayerAnimationType.ANIM_IDLE);
			PlayEmotion(EmoteIconType.EMOTE_CALLING);
			break;
		case PlayerActionType.ACT_CALLING_WITH_COMPLAIN:
			SetMoveable(true);
			PlayAnimation(PlayerAnimationType.ANIM_COMPLAIN);
			PlayEmotion(EmoteIconType.EMOTE_CALLING);
			break;
		case PlayerActionType.ACT_SICK:
			SetMoveable(true);
			PlayEmotion(EmoteIconType.EMOTE_SICK);
			break;
		case PlayerActionType.ACT_CURE:
			ResetPosition();
			PlayEmotion(EmoteIconType.EMOTE_CURE);
			break;
		case PlayerActionType.ACT_DRINK_MILK:
			ResetPosition();
			PlayAnimation(PlayerAnimationType.ANIM_DRINK);
			PlayFood(ItemIconType.ITEM_MILK, true);
			break;
		case PlayerActionType.ACT_EAT_SNACK:
			ResetPosition();
			PlayAnimation(PlayerAnimationType.ANIM_EAT_GOOD);
			PlayFood(ItemIconType.ITEM_SNACK, true);
			break;
		case PlayerActionType.ACT_EAT_SPECIAL:
		{
			ResetPosition();
			GrowthDatabase.CommandInfo commandInfo = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindCommandInfo("SPECIAL_NERO");
			GrowthDatabase.CommandInfo commandInfo2 = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindCommandInfo("SPECIAL_TAMAMO");
			GrowthDatabase.CommandInfo commandInfo3 = SingletonMonoBehaviour<GrowthDatabase>.Instance.FindCommandInfo("SPECIAL_ELIZA");
			int probabilityValue = commandInfo.GetProbabilityValue();
			int num = probabilityValue + commandInfo2.GetProbabilityValue();
			int num2 = num + commandInfo3.GetProbabilityValue();
			int num3 = Random.Range(0, 101);
			if (num3 < probabilityValue)
			{
				PlayAnimation(PlayerAnimationType.ANIM_EAT_GOOD);
				PlayFood(ItemIconType.ITEM_SPECIAL_NERO, true);
			}
			else if (num3 >= probabilityValue && num3 < num)
			{
				PlayAnimation(PlayerAnimationType.ANIM_EAT_GOOD);
				PlayFood(ItemIconType.ITEM_SPECIAL_TAMAMO, true);
			}
			else if (num3 >= num && num3 < num2)
			{
				PlayAnimation(PlayerAnimationType.ANIM_EAT_BAD);
				PlayFood(ItemIconType.ITEM_SPECIAL_ELIZA, false);
			}
			else
			{
				Debug.LogError("unknown food.");
			}
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetCookable", false);
			break;
		}
		case PlayerActionType.ACT_PLAY_BALL:
			ResetPosition();
			PlayAnimation(PlayerAnimationType.ANIM_BALL);
			PlayItem(ItemIconType.ITEM_BALL);
			break;
		case PlayerActionType.ACT_READ_BOOK:
			ResetPosition();
			PlayAnimation(PlayerAnimationType.ANIM_BOOK);
			PlayItem(ItemIconType.ITEM_BOOK);
			break;
		case PlayerActionType.ACT_SING:
			ResetPosition();
			PlayAnimation(PlayerAnimationType.ANIM_SING);
			PlayItem(ItemIconType.ITEM_NOTE);
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "SetSingable", false);
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_BGM_CONTROLLER), "RequestStartBgm", GINGLE_FILE_NAME);
			break;
		case PlayerActionType.ACT_SLEEP:
			ResetPosition();
			PlayAnimation(PlayerAnimationType.ANIM_SLEEP);
			PlayEmotion(EmoteIconType.EMOTE_SLEEP);
			PlayItem(ItemIconType.ITEM_COVERLET);
			if (SingletonMonoBehaviour<PlayerStatusManager>.Instance.IsSleepy())
			{
				MessageSender.SendWithObject(SingletonMonoBehaviour<PlayerStatusManager>.Instance.gameObject, "RaiseCondition", PlayerStatusManager.PlayerConditionState.TYPE_SLEEPING, true);
			}
			break;
		case PlayerActionType.ACT_DIAPER:
			ResetPosition();
			PlayAnimation(PlayerAnimationType.ANIM_COMPLAIN);
			PlayItem(ItemIconType.ITEM_DIAPER);
			break;
		case PlayerActionType.ACT_ACHIEVE:
			PlayAnimation(PlayerAnimationType.ANIM_ACHIEVE);
			PlayEmotion(EmoteIconType.EMOTE_ACHIEVE);
			SetPerformPosition();
			MessageSender.Send(Define.GetTagName(Define.TagId.TAG_CHARACTER_BACKGROUND), "CreateCostumeGetBackground", GetPlayerAlpha());
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_SE_CONTROLLER), "PlayMasterVoiceSe", SeController.SeType.SE_BABY_VOICE_JOY);
			MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MAIN_CANVAS), "SetAllButtonInteractive", false);
			break;
		case PlayerActionType.ACT_WAVE_HAND:
			SetClickable(true);
			PlayAnimation(PlayerAnimationType.ANIM_WAVE_HAND);
			break;
		}
	}
}
