using UnityEngine;

public class ButtonAnimeManager : SingletonMonoBehaviour<ButtonAnimeManager>
{
	public RuntimeAnimatorController[] buttonAnimeList_;

	public RuntimeAnimatorController GetButtonAnime(int animeIndex)
	{
		if (0 <= animeIndex && animeIndex < buttonAnimeList_.Length)
		{
			return buttonAnimeList_[animeIndex];
		}
		Debug.LogError(string.Concat("[", GetType(), "] Error: invalid animeIndex = ", animeIndex));
		return null;
	}

	private void Awake()
	{
		if (this != SingletonMonoBehaviour<ButtonAnimeManager>.Instance)
		{
			Object.Destroy(gameObject);
		}
	}
}
