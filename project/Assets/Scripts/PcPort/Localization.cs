// Kept in PlayerPrefs so a progress reset doesn't change it. Defaults to the Windows UI language.
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Localization
{
    public enum Language
    {
        Japanese = 0,
        English = 1
    }

    private const string PREF_KEY = "PcPortLanguage";

    private const string ENGLISH_SUFFIX = "_en";

    private static bool loaded_;

    private static Language current_;

    public static Language Current
    {
        get
        {
            if (!loaded_)
            {
                int saved = PlayerPrefs.GetInt(PREF_KEY, -1);
                if (saved == (int)Language.Japanese || saved == (int)Language.English)
                {
                    current_ = (Language)saved;
                }
                else
                {
                    current_ = (Application.systemLanguage == SystemLanguage.Japanese) ? Language.Japanese : Language.English;
                }
                loaded_ = true;
            }
            return current_;
        }
    }

    public static bool IsEnglish
    {
        get { return Current == Language.English; }
    }

    public static void Set(Language language)
    {
        current_ = language;
        loaded_ = true;
        PlayerPrefs.SetInt(PREF_KEY, (int)language);
        PlayerPrefs.Save();
    }

    // Use a language for this run only, without saving it (PortTour).
    public static void OverrideForSession(Language language)
    {
        current_ = language;
        loaded_ = true;
    }

    // Resources name of the English version of a text file.
    public static string EnglishFile(string baseName)
    {
        return baseName + ENGLISH_SUFFIX;
    }

    // Switch language and rebuild the title screen so every text is set again.
    // Only used from the options menu, which is on the title screen.
    public static void SwitchAndReloadTitle(Language language)
    {
        if (language == Current)
        {
            return;
        }
        Set(language);
        if (SingletonMonoBehaviour<TextDatabaseSystem>.Instance != null)
        {
            SingletonMonoBehaviour<TextDatabaseSystem>.Instance.Reload();
        }
        // Run on a persistent object: everything in the title scene is about to be destroyed.
        SingletonMonoBehaviour<StaticObjectsManager>.Instance.StartCoroutine(ReloadTitle());
    }

    private static IEnumerator ReloadTitle()
    {
        // Same sequence the game uses for a scene change: fade to the loading screen first.
        MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_FADE_CONTROLLER), "StartFadeIn", LoadingFader.LoadMessageType.LOADING);
        GameObject fader = GameObject.FindWithTag(Define.GetTagName(Define.TagId.TAG_FADE_CONTROLLER));
        LoadingFader fadeController = (fader != null) ? fader.GetComponent<LoadingFader>() : null;
        float timeout = Time.realtimeSinceStartup + 3f;
        while (fadeController != null && !fadeController.EnablesFadeOut() && Time.realtimeSinceStartup < timeout)
        {
            yield return null;
        }
        MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_MENU_CONTROLLER), "ClearMenuType");
        SceneManager.LoadScene(Define.GetSceneName(Define.SceneId.SCENE_TITLE), LoadSceneMode.Single);
        yield return null;
        yield return null;
        // GameController is persistent, so its Start (which starts this check at boot) won't run again.
        // The check is what fades the title screen in.
        MessageSender.SendWithTag("PlayableStateChecker", "StartPlayableStateCheck");
    }
}
