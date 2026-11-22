// [PC port] On-screen debug panel (the "DBG" button), shown only while Options > Debug is ON.
// Same features as the Android debug mod: it drives the game's own leftover debug code
// (DebugMenu overlay, DebugPlayerActionSender, DebugConsole, EventScheduler.ForwardTimer, ...).
// F2 also opens/closes it.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PortDebugPanel : MonoBehaviour
{
    public enum Tab { Info, Time, Stats, Costume, Day, Anim, Misc }
    private static readonly string[] TabNames = { "Info", "Time", "Stats", "Costume", "Day", "Anim", "Misc" };

    private static PortDebugPanel instance_;

    private DebugMenu menu_;
    private DebugPlayerActionSender sender_;
    private Assets.Scripts.Common.DebugConsole console_;

    private bool open_;
    private Tab tab_ = Tab.Info;
    private Vector2 scroll_;
    private float scale_ = 1f;
    private float styleScale_ = -1f;
    private string toast_ = "";
    private float toastUntil_;
    private bool confirmReset_;
    private int overlayFont_ = 20;

    // mouse drag-scroll
    private bool dragging_;
    private bool dragMoved_;
    private Vector2 lastTouch_;
    private Rect contentScreenRect_;
    private int lastW_, lastH_;

    // cached status (SaveData deserialization is not free)
    private PlayerStatusManager.PlayerStatus status_;
    private float nextStatusRead_;
    private float nextRefRefresh_;

    private GUIStyle btn_, tabBtn_, tabOn_, label_, head_, box_;
    private Texture2D bgPanel_, bgButton_, bgButtonOn_, bgOverlay_;

    // uGUI blocker so clicks on the panel don't press game buttons underneath
    private GameObject blockerCanvas_;
    private RectTransform buttonBlocker_;
    private GameObject panelBlocker_;

    // ------------------------------------------------------------------ static API

    public static void TogglePanel()
    {
        if (instance_ != null && PortDebug.Enabled)
        {
            instance_.open_ = !instance_.open_;
            instance_.confirmReset_ = false;
        }
    }

    public static void Close()
    {
        if (instance_ != null)
        {
            instance_.open_ = false;
            instance_.confirmReset_ = false;
        }
    }

    // PortTour
    public static void ShowTab(Tab tab)
    {
        if (instance_ != null)
        {
            instance_.open_ = true;
            instance_.tab_ = tab;
            instance_.scroll_ = Vector2.zero;
        }
    }

    public static void SetOverlay(bool on)
    {
        if (instance_ != null && instance_.menu_ != null)
        {
            instance_.menu_.isShow_ = on;
        }
    }

    // ------------------------------------------------------------------ lifecycle

    private void Awake()
    {
        instance_ = this;
        menu_ = GetComponent<DebugMenu>();
        sender_ = GetComponent<DebugPlayerActionSender>();
        console_ = GetComponent<Assets.Scripts.Common.DebugConsole>();
        // DebugMenu's F2 handler references actionPanel_; give it a harmless dummy.
        GameObject dummy = new GameObject("DebugActionPanelDummy");
        dummy.transform.SetParent(transform, false);
        dummy.SetActive(false);
        menu_.actionPanel_ = dummy;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        bgPanel_ = MakeTex(new Color(0.08f, 0.08f, 0.12f, 0.92f));
        bgButton_ = MakeTex(new Color(0.25f, 0.27f, 0.35f, 1f));
        bgButtonOn_ = MakeTex(new Color(0.85f, 0.35f, 0.25f, 1f));
        bgOverlay_ = MakeTex(new Color(1f, 1f, 1f, 0.75f));
        UpdateScale();
        overlayFont_ = Mathf.RoundToInt(22 * scale_);
        ApplyOverlayStyle();
        CreateBlocker();
        RefreshRefs();
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        RefreshRefs();
    }

    private void RefreshRefs()
    {
        if (menu_ == null)
        {
            return;
        }
        menu_.timeObserber_ = FindObjectOfType<DateTimeObserber>();
        GameObject gc = null;
        try { gc = GameObject.FindGameObjectWithTag("GameController"); } catch (Exception) { }
        menu_.eventScheduler_ = gc != null ? gc.GetComponent<EventScheduler>() : null;
    }

    private bool InMainGame
    {
        get { return menu_ != null && menu_.eventScheduler_ != null && menu_.timeObserber_ != null; }
    }

    private void UpdateScale()
    {
        scale_ = Mathf.Max(0.5f, Mathf.Min(Screen.width, Screen.height) / 900f);
    }

    private void Update()
    {
        bool visible = PortDebug.Enabled;
        if (!visible)
        {
            open_ = false;
        }
        if (Time.unscaledTime > nextRefRefresh_)
        {
            nextRefRefresh_ = Time.unscaledTime + 1f;
            if (menu_.eventScheduler_ == null || menu_.timeObserber_ == null)
            {
                RefreshRefs();
            }
        }
        if (Screen.width != lastW_ || Screen.height != lastH_)
        {
            lastW_ = Screen.width;
            lastH_ = Screen.height;
            UpdateScale();
            if (buttonBlocker_ != null)
            {
                LayoutBlockers();
            }
        }
        if (buttonBlocker_ != null && buttonBlocker_.gameObject.activeSelf != visible)
        {
            buttonBlocker_.gameObject.SetActive(visible);
        }
        if (panelBlocker_ != null && panelBlocker_.activeSelf != open_)
        {
            panelBlocker_.SetActive(open_);
        }
        UpdateDragScroll();
    }

    // ------------------------------------------------------------------ GUI

    private void EnsureStyles()
    {
        if (btn_ != null && styleScale_ == scale_)
        {
            return;
        }
        styleScale_ = scale_;
        int fs = Mathf.RoundToInt(24 * scale_);
        btn_ = new GUIStyle(GUI.skin.button);
        btn_.fontSize = fs;
        btn_.wordWrap = true;
        btn_.normal.background = bgButton_;
        btn_.hover.background = bgButton_;
        btn_.active.background = bgButtonOn_;
        btn_.normal.textColor = btn_.hover.textColor = btn_.active.textColor = Color.white;
        btn_.padding = new RectOffset(6, 6, 6, 6);
        btn_.margin = new RectOffset(4, 4, 4, 4);
        tabBtn_ = new GUIStyle(btn_);
        tabBtn_.fontSize = Mathf.RoundToInt(22 * scale_);
        tabOn_ = new GUIStyle(tabBtn_);
        tabOn_.normal.background = tabOn_.hover.background = bgButtonOn_;
        label_ = new GUIStyle(GUI.skin.label);
        label_.fontSize = fs;
        label_.wordWrap = true;
        label_.normal.textColor = Color.white;
        head_ = new GUIStyle(label_);
        head_.fontStyle = FontStyle.Bold;
        head_.normal.textColor = new Color(1f, 0.75f, 0.4f);
        box_ = new GUIStyle(GUI.skin.box);
        box_.normal.background = bgPanel_;
    }

    // Right edge, just above the panel, so the panel never covers it.
    private Rect ToggleRect()
    {
        float w = 110 * scale_, h = 56 * scale_, m = 8 * scale_;
        return new Rect(Screen.width - w - m, PanelRect().y - h - m, w, h);
    }

    private Rect PanelRect()
    {
        float m = 8 * scale_;
        bool portrait = Screen.height > Screen.width;
        float h = Screen.height * (portrait ? 0.55f : 0.85f);
        float w = portrait ? Screen.width - 2 * m : Screen.width * 0.6f;
        return new Rect(Screen.width - w - m, Screen.height - h - m, w, h);
    }

    private void OnGUI()
    {
        if (!PortDebug.Enabled)
        {
            return;
        }
        EnsureStyles();
        GUI.depth = -1000;
        if (GUI.Button(ToggleRect(), open_ ? "X DBG" : "DBG", open_ ? tabOn_ : tabBtn_))
        {
            open_ = !open_;
            confirmReset_ = false;
        }
        if (open_)
        {
            DrawPanel();
        }
        if (Time.unscaledTime < toastUntil_)
        {
            Rect r = new Rect(0, Screen.height * 0.08f, Screen.width, 50 * scale_);
            GUI.Box(r, "", box_);
            GUIStyle center = new GUIStyle(head_);
            center.alignment = TextAnchor.MiddleCenter;
            GUI.Label(r, toast_, center);
        }
    }

    private void DrawPanel()
    {
        Rect p = PanelRect();
        GUI.Box(p, "", box_);
        float pad = 10 * scale_;
        Rect area = new Rect(p.x + pad, p.y + pad, p.width - 2 * pad, p.height - 2 * pad);
        GUILayout.BeginArea(area);
        int perRow = 4;
        for (int row = 0; row * perRow < TabNames.Length; row++)
        {
            GUILayout.BeginHorizontal();
            for (int i = row * perRow; i < Mathf.Min(TabNames.Length, (row + 1) * perRow); i++)
            {
                if (GUILayout.Button(TabNames[i], (int)tab_ == i ? tabOn_ : tabBtn_, GUILayout.Height(48 * scale_)))
                {
                    tab_ = (Tab)i;
                    scroll_ = Vector2.zero;
                    confirmReset_ = false;
                }
            }
            // Last row has a free slot: close button (DBG button and F2 also close).
            if ((row + 1) * perRow >= TabNames.Length)
            {
                if (GUILayout.Button("Close X", tabOn_, GUILayout.Height(48 * scale_)))
                {
                    open_ = false;
                    confirmReset_ = false;
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.Space(6 * scale_);
        if (!InMainGame && tab_ != Tab.Info && tab_ != Tab.Costume && tab_ != Tab.Misc)
        {
            GUILayout.Label("Start or continue a game first. Most of these only work on the main screen.", head_);
        }
        contentScreenRect_ = area;
        scroll_ = GUILayout.BeginScrollView(scroll_, false, true);
        switch (tab_)
        {
            case Tab.Info: TabInfo(); break;
            case Tab.Time: TabTime(); break;
            case Tab.Stats: TabStats(); break;
            case Tab.Costume: TabCostume(); break;
            case Tab.Day: TabDay(); break;
            case Tab.Anim: TabAnim(); break;
            case Tab.Misc: TabMisc(); break;
        }
        GUILayout.Space(30 * scale_);
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    // A button that ignores the release at the end of a drag-scroll.
    private bool Btn(string text)
    {
        return GUILayout.Button(text, btn_, GUILayout.MinHeight(52 * scale_)) && !dragMoved_;
    }

    private void Head(string t)
    {
        GUILayout.Space(8 * scale_);
        GUILayout.Label(t, head_);
    }

    private void Text(string t)
    {
        GUILayout.Label(t, label_);
    }

    private void Toast(string msg)
    {
        toast_ = msg;
        toastUntil_ = Time.unscaledTime + 2f;
        Debug.Log("[PortDebug] " + msg);
    }

    // ------------------------------------------------------------------ tabs

    private void TabInfo()
    {
        Head("Info overlay (original DebugMenu)");
        GUILayout.BeginHorizontal();
        if (Btn(menu_.isShow_ ? "Overlay: ON" : "Overlay: OFF")) menu_.isShow_ = !menu_.isShow_;
        if (Btn("< Mode")) menu_.BeforeMode();
        if (Btn("Mode >")) menu_.NextMode();
        GUILayout.EndHorizontal();
        Text("Modes: stats + event timers / resume log / per-phase parameter log. Shown on the main screen only. Keys: F1 on/off, Left/Right mode, mouse wheel text size.");
        GUILayout.BeginHorizontal();
        if (Btn("Text -")) { overlayFont_ = Mathf.Max(10, overlayFont_ - 2); ApplyOverlayStyle(); }
        if (Btn("Text +")) { overlayFont_ = Mathf.Min(80, overlayFont_ + 2); ApplyOverlayStyle(); }
        if (Btn("Clear resume log")) { menu_.ClearResumeLog(); Toast("Resume log cleared"); }
        GUILayout.EndHorizontal();

        Head("Log console (original DebugConsole)");
        GUILayout.BeginHorizontal();
        if (Btn(console_.Show ? "Console button: ON" : "Console button: OFF"))
        {
            console_.Show = !console_.Show;
            float s = scale_;
            console_.ButtonRect = new Rect(8 * s, Screen.height * 0.5f - 28 * s, 180 * s, 56 * s);
            console_.PosRect = new Rect(0, Screen.height * 0.5f + 36 * s, Screen.width, Screen.height * 0.5f - 36 * s);
            console_.ViewRect = new Rect(0, 0, Screen.width - 20, Screen.height * 3);
        }
        if (Btn(console_.StackTrace ? "Stack traces: ON" : "Stack traces: OFF")) console_.StackTrace = !console_.StackTrace;
        GUILayout.EndHorizontal();
        Text("Shows a \"Console\" button on the left that opens the game's Debug.Log output (` key also toggles it).");

        Head("Status");
        ReadStatus();
        Text("Scene: " + SceneManager.GetActiveScene().name + "   FPS: " + GetComponent<FpsObserber>().FrameRate.ToString("0.0") + "   Speed: x" + Time.timeScale.ToString("0.#"));
        if (status_ != null)
        {
            Text(string.Format("Weight {0:0.00}kg  Satiety {1}/6  Mood {2}/6", status_.weight_, status_.satiety_, status_.feel_));
            Text(string.Format("STR {0:0.0}  INT {1:0.0}  MAG {2:0.0}", status_.strength_, status_.intelligence_, status_.magic_));
        }
    }

    private void TabTime()
    {
        DateTimeObserber dto = menu_.timeObserber_;
        Head("Game speed (current x" + Time.timeScale.ToString("0.#") + (dto != null && dto.isTimePause_ ? ", PAUSED" : "") + ")");
        GUILayout.BeginHorizontal();
        if (Btn("x1")) SetRate(1f);
        if (Btn("-5")) SetRate(Mathf.Max(1f, CurRate() - 5f));
        if (Btn("+5")) SetRate(Mathf.Min(100f, CurRate() + 5f));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (Btn("x10")) SetRate(10f);
        if (Btn("x50")) SetRate(50f);
        if (Btn("x100")) SetRate(100f);
        GUILayout.EndHorizontal();
        if (Btn(dto != null && dto.isTimePause_ ? "Resume" : "Pause"))
        {
            if (dto != null)
            {
                dto.isTimePause_ = !dto.isTimePause_;
                Time.timeScale = dto.isTimePause_ ? 0f : Mathf.Max(1f, dto.timeRate_);
            }
            else
            {
                Time.timeScale = Time.timeScale == 0f ? 1f : 0f;
            }
        }

        Head("Skip ahead (all event timers)");
        GUILayout.BeginHorizontal();
        if (Btn("+1 min")) Skip(60f);
        if (Btn("+10 min")) Skip(600f);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (Btn("+1000 s")) Skip(1000f);
        if (Btn("+1 hour")) Skip(3600f);
        GUILayout.EndHorizontal();
        Text("Moves the hunger and mood timers, the current phase, and the cooking, singing, sickness and diaper checks closer. Keys: PageUp/PageDown speed, Home x100, End x1, Pause, Ctrl+Up skip 1000 s.");

        if (InMainGame)
        {
            Head("Next events (seconds)");
            EventScheduler es = menu_.eventScheduler_;
            try
            {
                Text("Satiety in " + es.GetNextSatietyEventTIme().ToString("0") + "   Mood in " + es.GetNextFeelEventTime().ToString("0"));
                Text("Phase " + es.GetCurrentPhaseIndex() + " " + es.GetCurrentPhaseState() + "   Day state: " + es.GetCurrentDayStateString());
            }
            catch (Exception)
            {
            }
        }
    }

    private float CurRate()
    {
        DateTimeObserber dto = menu_.timeObserber_;
        return dto != null ? dto.timeRate_ : Time.timeScale;
    }

    private void SetRate(float r)
    {
        DateTimeObserber dto = menu_.timeObserber_;
        if (dto != null)
        {
            dto.timeRate_ = r;
            dto.isTimePause_ = false;
        }
        Time.timeScale = r;
        Toast("Game speed x" + r.ToString("0"));
    }

    private void Skip(float sec)
    {
        MessageSender.SendWithTag(Define.GetTagName(Define.TagId.TAG_GAME_CONTROLLER), "ForwardTimer", sec);
        Toast("Skipped " + sec.ToString("0") + " s");
    }

    private void TabStats()
    {
        ReadStatus();
        StatRow("Weight", PlayerStatusManager.PlayerStatusType.TYPE_WEIGHT, status_ != null ? status_.weight_.ToString("0.00") + " kg" : "?", false);
        StatRow("Satiety (Tummy)", PlayerStatusManager.PlayerStatusType.TYPE_SATIETY, status_ != null ? status_.satiety_ + " / 6" : "?", true);
        StatRow("Mood (Happiness)", PlayerStatusManager.PlayerStatusType.TYPE_FEEL, status_ != null ? status_.feel_ + " / 6" : "?", true);
        StatRow("Strength", PlayerStatusManager.PlayerStatusType.TYPE_STRENGTH, status_ != null ? status_.strength_.ToString("0.0") : "?", false);
        StatRow("Intelligence", PlayerStatusManager.PlayerStatusType.TYPE_INTELLIGENCE, status_ != null ? status_.intelligence_.ToString("0.0") : "?", false);
        StatRow("Magic", PlayerStatusManager.PlayerStatusType.TYPE_MAGIC, status_ != null ? status_.magic_.ToString("0.0") : "?", false);
        Text("Values are clamped to the game's own min and max. Keys: hold F5 (satiety) / F6 (mood) / F9 (strength)... and press Up/Down.");
    }

    private void StatRow(string name, PlayerStatusManager.PlayerStatusType type, string value, bool isInt)
    {
        Head(name + ": " + value);
        GUILayout.BeginHorizontal();
        int[] steps = isInt ? new[] { -6, -1, 1, 6 } : new[] { -10, -1, 1, 10 };
        foreach (int st in steps)
        {
            if (Btn((st > 0 ? "+" : "") + st))
            {
                // PlayerStatusManager.AddPlayerStatus unboxes an int for satiety/feel and a float for the rest.
                object amount = isInt ? (object)st : (object)(float)st;
                GameObject go = GameObject.FindWithTag("Player");
                if (go != null)
                {
                    go.SendMessage("AddPlayerStatus", new object[] { type, amount });
                    nextStatusRead_ = 0f;
                }
                else
                {
                    Toast("Player not found (main screen only)");
                }
            }
        }
        GUILayout.EndHorizontal();
    }

    private void ReadStatus()
    {
        if (Time.unscaledTime < nextStatusRead_)
        {
            return;
        }
        nextStatusRead_ = Time.unscaledTime + 0.5f;
        try { status_ = SaveData.GetClass("PlayerStatus", new PlayerStatusManager.PlayerStatus()); }
        catch (Exception) { status_ = null; }
    }

    private static readonly string[] CostumeIds =
    {
        "CHID_NERO", "CHID_TAMAMO", "CHID_ATTILA", "CHID_ELIZA", "CHID_MUMEI", "CHID_KARNA",
        "CHID_GILGAMESH", "CHID_CUCHULAIN", "CHID_ALTORIA", "CHID_CHARLE", "CHID_MASTER_M", "CHID_MASTER_F"
    };

    private void TabCostume()
    {
        Head("Gallery unlocks (saved)");
        GUILayout.BeginHorizontal();
        if (Btn("Unlock all costumes")) { sender_.UnlockAllCostume(); SaveData.Save(); Toast("All costumes unlocked"); }
        if (Btn("Lock all costumes")) { sender_.LockAllCostume(); SaveData.Save(); Toast("All costumes locked"); }
        GUILayout.EndHorizontal();
        Head("Change costume now (main screen)");
        Grid(CostumeIds, 2, delegate (string id) { sender_.ChangeCostume(id); Toast("Costume: " + Pretty(id)); });
        Head("Room background");
        GUILayout.BeginHorizontal();
        if (Btn("Male room")) sender_.SetCharacterBackground(true);
        if (Btn("Female room")) sender_.SetCharacterBackground(false);
        GUILayout.EndHorizontal();
    }

    private void TabDay()
    {
        Head("Force time of day");
        if (InMainGame) Text("Current: " + menu_.eventScheduler_.GetCurrentDayStateString());
        Grid(new[] { "STATE_DAYTIME", "STATE_SLEEPABLE", "STATE_NIGHT" }, 3, delegate (string s) { sender_.OverWriteDayState(s); Toast("Day state forced: " + Pretty(s)); });
        if (Btn("Stop forcing (use real clock)")) { sender_.EndOverWriteDayState(); Toast("Day state follows the clock again"); }
        Head("Flags");
        if (Btn("Allow cooking now")) { sender_.RaiseCookableFlag(); Toast("Cookable flag set"); }
    }

    private void TabAnim()
    {
        Text("Plays on the character. To give control back, use Actions > DEFAULT.");
        Head("Actions (full reactions)");
        Grid(EnumNames(typeof(PlayerController.PlayerActionType)), 2, delegate (string a) { sender_.PlayAction(a); });
        Head("Animations");
        Grid(EnumNames(typeof(PlayerController.PlayerAnimationType)), 2, delegate (string a) { sender_.PlayAnimation(a); });
        Head("Emotion icons");
        Grid(EnumNames(typeof(PlayerController.EmoteIconType)), 2, delegate (string a) { sender_.PlayeEmotion(a); });
        Head("Item icons");
        Grid(EnumNames(typeof(PlayerController.ItemIconType)), 2, delegate (string a) { sender_.PlayItem(a); });
    }

    private void TabMisc()
    {
        Head("Reset game");
        Text("Same as the debug Esc+Delete shortcut: ends the current run and resets the pet. With debug ON it first writes a parameter log to the cache folder.");
        if (!confirmReset_)
        {
            if (Btn("Reset game...")) confirmReset_ = true;
        }
        else
        {
            GUILayout.BeginHorizontal();
            if (Btn("YES, reset")) { sender_.GameReset(); SaveData.Save(); confirmReset_ = false; Toast("Game reset"); }
            if (Btn("Cancel")) confirmReset_ = false;
            GUILayout.EndHorizontal();
        }
        Head("Save");
        if (Btn("Force save now")) { SaveData.Save(); Toast("Saved"); }
        Head("Keyboard shortcuts (debug ON)");
        Text("F1 overlay | F2 this panel | Left/Right overlay mode | PageUp/PageDown speed +-5 | Home x100 | End x1 | Pause | Ctrl+Up skip 1000 s | hold F5 (satiety) / F6 (mood) / F9 (strength)... + Up/Down | Esc+Delete reset | ` console");
    }

    // ------------------------------------------------------------------ helpers

    private void Grid(string[] items, int cols, Action<string> onClick)
    {
        for (int i = 0; i < items.Length; i += cols)
        {
            GUILayout.BeginHorizontal();
            for (int j = i; j < i + cols; j++)
            {
                if (j < items.Length)
                {
                    if (Btn(Pretty(items[j]))) onClick(items[j]);
                }
                else
                {
                    GUILayout.Label("", label_);
                }
            }
            GUILayout.EndHorizontal();
        }
    }

    private static string[] EnumNames(Type t)
    {
        List<string> list = new List<string>();
        foreach (string n in Enum.GetNames(t))
        {
            if (!n.EndsWith("_MAX")) list.Add(n);
        }
        return list.ToArray();
    }

    private static string Pretty(string id)
    {
        int u = id.IndexOf('_');
        return u >= 0 ? id.Substring(u + 1) : id;
    }

    private static Texture2D MakeTex(Color c)
    {
        Texture2D t = new Texture2D(1, 1);
        t.SetPixel(0, 0, c);
        t.Apply();
        t.hideFlags = HideFlags.HideAndDontSave;
        return t;
    }

    private void ApplyOverlayStyle()
    {
        if (menu_ == null) return;
        foreach (GUIStyle st in new[] { menu_.style, menu_.emphasis })
        {
            st.fontSize = overlayFont_;
            st.normal.background = bgOverlay_;
            st.padding = new RectOffset(4, 4, 1, 1);
            st.margin = new RectOffset(0, 0, 0, 0);
        }
    }

    private void UpdateDragScroll()
    {
        if (!open_)
        {
            dragging_ = false;
            return;
        }
        Vector2 pos = Input.mousePosition;
        pos.y = Screen.height - pos.y; // to GUI coordinates
        if (Input.GetMouseButtonDown(0))
        {
            dragging_ = contentScreenRect_.Contains(pos);
            dragMoved_ = false;
            lastTouch_ = pos;
        }
        else if (Input.GetMouseButton(0) && dragging_)
        {
            Vector2 d = pos - lastTouch_;
            if (!dragMoved_ && d.magnitude > 18f * scale_) dragMoved_ = true;
            if (dragMoved_)
            {
                scroll_.y = Mathf.Max(0f, scroll_.y - d.y);
                lastTouch_ = pos;
            }
        }
        else if (!Input.GetMouseButton(0) && dragging_)
        {
            // keep dragMoved_ for this frame so the release doesn't count as a click
            dragging_ = false;
        }
        // mouse wheel scrolls the panel too
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel != 0f && contentScreenRect_.Contains(pos))
        {
            scroll_.y = Mathf.Max(0f, scroll_.y - wheel * 400f * scale_);
        }
    }

    private void CreateBlocker()
    {
        blockerCanvas_ = new GameObject("DebugPanelBlocker");
        blockerCanvas_.transform.SetParent(transform, false);
        Canvas canvas = blockerCanvas_.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;
        blockerCanvas_.AddComponent<GraphicRaycaster>();
        buttonBlocker_ = MakeBlockerImage("ButtonBlocker").rectTransform;
        panelBlocker_ = MakeBlockerImage("PanelBlocker").gameObject;
        panelBlocker_.SetActive(false);
        buttonBlocker_.gameObject.SetActive(PortDebug.Enabled);
        LayoutBlockers();
    }

    private Image MakeBlockerImage(string name)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(blockerCanvas_.transform, false);
        Image img = go.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0f);
        img.raycastTarget = true;
        return img;
    }

    private void LayoutBlockers()
    {
        SetRectFromGui(buttonBlocker_, ToggleRect());
        SetRectFromGui(panelBlocker_.GetComponent<RectTransform>(), PanelRect());
    }

    private static void SetRectFromGui(RectTransform rt, Rect r)
    {
        rt.anchorMin = new Vector2(r.xMin / Screen.width, 1f - r.yMax / Screen.height);
        rt.anchorMax = new Vector2(r.xMax / Screen.width, 1f - r.yMin / Screen.height);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
