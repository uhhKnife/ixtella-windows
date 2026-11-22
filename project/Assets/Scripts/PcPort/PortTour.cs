// [PC port] Automated screenshot tour for checking layouts (both languages).
// Does nothing unless the game is started with -porttour:
//
//   IkustellaLink.exe -porttour -porttour-lang en -porttour-out C:\shots -screen-width 540 -screen-height 960
//
// It opens every menu and dialog through the game's own controllers, saves a PNG of each,
// and quits. The language is used for this run only (not saved). It touches game state
// in memory (game-over screens) and the game may autosave, so back up FixlSavedata.json first.
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

public class PortTour : MonoBehaviour
{
    private static string outDir_;

    private int shot_;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void BeforeSceneLoad()
    {
        string lang = Arg("-porttour-lang");
        if (HasArg("-porttour") && lang != null)
        {
            Localization.OverrideForSession(lang == "ja" ? Localization.Language.Japanese : Localization.Language.English);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AfterSceneLoad()
    {
        if (!HasArg("-porttour"))
        {
            return;
        }
        outDir_ = Arg("-porttour-out") ?? Path.Combine(Application.persistentDataPath, "porttour");
        Directory.CreateDirectory(outDir_);
        GameObject go = new GameObject("PortTour");
        DontDestroyOnLoad(go);
        go.AddComponent<PortTour>();
    }

    private static bool HasArg(string name)
    {
        return Array.IndexOf(Environment.GetCommandLineArgs(), name) >= 0;
    }

    private static string Arg(string name)
    {
        string[] args = Environment.GetCommandLineArgs();
        int i = Array.IndexOf(args, name);
        return (i >= 0 && i + 1 < args.Length) ? args[i + 1] : null;
    }

    private void Start()
    {
        StartCoroutine(Run());
    }

    private IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        shot_++;
        string path = Path.Combine(outDir_, shot_.ToString("00") + "_" + name + ".png");
        ScreenCapture.CaptureScreenshot(path);
        Debug.Log("[PortTour] " + path);
        yield return new WaitForSeconds(0.3f);
    }

    private static string Tag(Define.TagId id)
    {
        return Define.GetTagName(id);
    }

    private IEnumerator Menu(MenuCommonData.MenuType type, string name)
    {
        MessageSender.SendWithTag(Tag(Define.TagId.TAG_MENU_CONTROLLER), "OpenMenu", type);
        yield return new WaitForSeconds(2.5f);
        yield return StartCoroutine(Capture(name));
        // Help pages are swipe pages: capture page 2 if there is one.
        SwipeTextController pages = FindObjectOfType<SwipeTextController>();
        if (pages != null && pages.transform.childCount > 0)
        {
            Transform content = pages.GetComponentInChildren<UnityEngine.UI.ScrollRect>() != null ? pages.GetComponentInChildren<UnityEngine.UI.ScrollRect>().content : null;
            if (content != null && content.childCount > 1)
            {
                pages.SendMessage("SetCurrentPageIndex", 1, SendMessageOptions.DontRequireReceiver);
                yield return new WaitForSeconds(1.2f);
                yield return StartCoroutine(Capture(name + "_p2"));
            }
        }
    }

    private IEnumerator Dialog(string sceneName, string textId, string tag, string value, string name)
    {
        DialogCommonData.SetCanvasComponentActive(sceneName, true);
        ResourceRequest req = Resources.LoadAsync(DialogCommonData.GetDialogPrefabName(DialogCommonData.DialogType.DIALOG_MESSAGE));
        while (!req.isDone)
        {
            yield return null;
        }
        GameObject dialog = DialogCommonData.InstantiateDialog(req, sceneName);
        dialog.SetActive(true);
        PortTourDialog d = dialog.AddComponent<PortTourDialog>();
        d.Setup(textId, tag, value);
        DialogCommonData.SetAnimeTriggerFlag(sceneName, true);
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(Capture(name));
        dialog.SendMessage("CloseDialog", SendMessageOptions.DontRequireReceiver);
        yield return new WaitForSeconds(1.2f);
    }

    private static void SetField(object target, string field, object value)
    {
        FieldInfo f = target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic);
        f.SetValue(target, value);
    }

    private IEnumerator Run()
    {
        yield return new WaitForSeconds(9f);
        string title = Define.GetSceneName(Define.SceneId.SCENE_TITLE);
        yield return StartCoroutine(Capture("title"));

        MenuCommonData.MenuType[] titleMenus =
        {
            MenuCommonData.MenuType.MENU_OPTION_TOP,
            MenuCommonData.MenuType.MENU_HOW_TO_PLAY_TOP,
            MenuCommonData.MenuType.MENU_HOW_TO_PLAY_1,
            MenuCommonData.MenuType.MENU_HOW_TO_PLAY_2,
            MenuCommonData.MenuType.MENU_HOW_TO_CARE_COMMAND,
            MenuCommonData.MenuType.MENU_HOW_TO_PLAY_4,
            MenuCommonData.MenuType.MENU_HOW_TO_PLAY_5,
            MenuCommonData.MenuType.MENU_HOW_TO_PLAY_6,
            MenuCommonData.MenuType.MENU_HOW_TO_PLAY_7,
            MenuCommonData.MenuType.MENU_HELP_MEAL_COMMAND,
            MenuCommonData.MenuType.MENU_HELP_PLAY_COMMAND,
            MenuCommonData.MenuType.MENU_HELP_DIAPER_COMMAND,
            MenuCommonData.MenuType.MENU_HELP_SLEEP_COMMAND,
            MenuCommonData.MenuType.MENU_HELP_TREATMENT,
            MenuCommonData.MenuType.MENU_HELP_PARAMETER,
            MenuCommonData.MenuType.MENU_HELP_COSTUME,
            MenuCommonData.MenuType.MENU_MASTER_SELECT,
            MenuCommonData.MenuType.MENU_USER_POLICY,
            MenuCommonData.MenuType.MENU_CREDIT
        };
        foreach (MenuCommonData.MenuType m in titleMenus)
        {
            yield return StartCoroutine(Menu(m, m.ToString().Replace("MENU_", "").ToLower()));
        }
        MessageSender.SendWithTag(Tag(Define.TagId.TAG_MENU_CONTROLLER), "CloseMenu");
        yield return new WaitForSeconds(2f);

        string servant = SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetServantNameText(Define.CharaId.CHID_CHARLE);
        yield return StartCoroutine(Dialog(title, "SYS_MENU_0-1_dialog_00", null, null, "dlg_quit"));
        yield return StartCoroutine(Dialog(title, "SYS_MENU_0-1-2_dialog_00", null, null, "dlg_reset_confirm"));
        yield return StartCoroutine(Dialog(title, "SYS_MENU_0-1-2_dialog_01", null, null, "dlg_reset_done"));
        yield return StartCoroutine(Dialog(title, "SYS_MENU_0-2_dialog_00", "[gender]", SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText("SYS_master_female"), "dlg_gender"));
        yield return StartCoroutine(Dialog(title, "SYS_MENU_4_dialog_00", "[servant_name]", servant, "dlg_costume_change"));
        yield return StartCoroutine(Dialog(title, "SYS_MENU_4_dialog_00", "[servant_name]", SingletonMonoBehaviour<TextDatabaseSystem>.Instance.GetText("SYS_MENU_4_default_costume"), "dlg_costume_original"));
        yield return StartCoroutine(Dialog(title, "SYS_MENU_4_dialog_01", null, null, "dlg_costume_gender"));
        yield return StartCoroutine(Dialog(title, "SYS_MENU_7-1_dialog_00", "[servant_name]", servant, "dlg_reward"));
        yield return StartCoroutine(Dialog(title, "SYS_MENU_0_dialog_date_error", null, null, "dlg_date_error"));
        yield return StartCoroutine(Dialog(title, "SYS_MENU_0_dialog_connection_error", null, null, "dlg_connection_error"));
        yield return StartCoroutine(Dialog(title, "SYS_MENU_0_dialog_download_error", null, null, "dlg_download_error"));

        // Main screen
        MessageSender.SendWithTag(Tag(Define.TagId.TAG_SCENE_CONTROLLER), "ChangeScene", Define.SceneId.SCENE_MAIN);
        yield return new WaitForSeconds(9f);
        yield return StartCoroutine(Capture("main"));
        // Meal / Play choice dialogs, opened the same way the care buttons do.
        foreach (OpenDialogButtonBase button in new OpenDialogButtonBase[] { FindObjectOfType<CareMealButton>(), FindObjectOfType<CarePlayButton>() })
        {
            if (button == null)
            {
                continue;
            }
            button.OpenDialog();
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(Capture("choice_" + button.GetType().Name));
            DialogBase open = FindObjectOfType<DialogBase>();
            if (open != null)
            {
                open.SendMessage("CloseDialog", SendMessageOptions.DontRequireReceiver);
            }
            yield return new WaitForSeconds(1.5f);
        }
        yield return StartCoroutine(Menu(MenuCommonData.MenuType.MENU_PARAMETER_CHECK, "parameter_check"));
        yield return StartCoroutine(Menu(MenuCommonData.MenuType.MENU_GALLERY, "gallery"));
        yield return StartCoroutine(Menu(MenuCommonData.MenuType.MENU_GET_COSTUME, "get_costume"));

        // Game over letters (3 causes x 2 genders) and game clear, by setting the state in memory.
        GameController gc = SingletonMonoBehaviour<GameController>.Instance;
        object oldState = gc.GetType().GetField("gameState_", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(gc);
        object oldGender = gc.GetType().GetField("playerGender_", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(gc);
        GameController.GameState[] overs =
        {
            GameController.GameState.GAME_STATE_GAMEOVER_BY_SATIETY,
            GameController.GameState.GAME_STATE_GAMEOVER_BY_FEEL,
            GameController.GameState.GAME_STATE_GAMEOVER_BY_SICK
        };
        foreach (PlayerStatusManager.Gender g in new[] { PlayerStatusManager.Gender.GENDER_MALE, PlayerStatusManager.Gender.GENDER_FEMALE })
        {
            SetField(gc, "playerGender_", g);
            foreach (GameController.GameState s in overs)
            {
                SetField(gc, "gameState_", s);
                yield return StartCoroutine(Menu(MenuCommonData.MenuType.MENU_GAME_OVER, "gameover_" + s.ToString().Replace("GAME_STATE_GAMEOVER_BY_", "").ToLower() + (g == PlayerStatusManager.Gender.GENDER_FEMALE ? "_f" : "_m")));
            }
        }
        SetField(gc, "gameState_", GameController.GameState.GAME_STATE_CLEAR);
        yield return StartCoroutine(Menu(MenuCommonData.MenuType.MENU_GAME_CLEAR, "gameclear"));
        SetField(gc, "gameState_", oldState);
        SetField(gc, "playerGender_", oldGender);

        Debug.Log("[PortTour] done");
        Application.Quit();
    }
}

// Message dialog showing any text id, with one [tag] replaced (as the real dialogs do).
public class PortTourDialog : MessageDialogBase
{
    private string textId_;

    private string tag_;

    private string value_;

    public void Setup(string textId, string tag, string value)
    {
        textId_ = textId;
        tag_ = tag;
        value_ = value;
    }

    protected override void Start()
    {
        base.Start();
        SetAllButtonsText();
        if (tag_ != null)
        {
            ReplaceBodyTextTag(tag_, value_);
        }
    }

    protected override string GetBodyTextId()
    {
        return textId_;
    }
}
