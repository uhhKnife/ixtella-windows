# Windows port: progress log

## Status (2026-11-22)
- Phase 1–3 done. Tools installed: Unity Hub, Unity 2017.2.3f1 (licensed), AssetRipper 2.0.0.
- Phase 4 done: project/ imports and compiles cleanly in 2017.2.3f1 (0 errors). Fixes listed in README.md.
- Phase 5 done: Windows x86 build OK (build/win-x86, PE32 i386). x64 not needed.
- Phase 6 in progress. Verified so far:
  - startup on D3D11, title screen renders correctly (screenshot logs/test/10_relaunch.png)
  - offline check passes ("Time Check is succeed")
  - new save created; quit + relaunch -> "Data Loaded"; save decrypts to valid JSON
  - title -> user policy -> gender select -> Main scene transition (driven by real mouse input from the user;
    the log shows the menu changes)
  - BgIcon tiled-image mesh overflow found in Main and fixed (tools/fix_sprites.py). NEEDS RE-CHECK in Main.
  - Automated clicking via PostMessage does not reach Unity input (it reads the real cursor), so the
    remaining checks are a manual pass by the user plus log review.

## Key facts (Phase 1)
- Unity 2017.2.3f1, changeset 372229934efd (from libunity.so). Mono backend (libmono.so), .NET 3.5 profile (mscorlib 2.0).
- ABIs: armeabi-v7a, x86 (Android ELF; not usable on Windows). No third-party native plugins.
- Data: data.unity3d (UnityFS, LZ4) holds globalgamemanagers, level0 (Title, 258 objs), level1 (Main, 471 objs),
  sharedassets0/1, resources.assets (1604 objs, 530 Resources paths); plus resources.resource / sharedassets0.resource (audio).
- Shaders: only Unity built-ins (Sprites/Default, Sprites/Mask, UI/Default, Hidden/*). No custom shaders.
- Textures: 66 RGBA32, 5 RGB24, 38 ETC2_RGBA8 (lossy on the phone already). Audio: 7 clips, Vorbis.
- Java: Unity player, Simple Android Notifications (hippogames) plugin. C# calls it through AndroidJavaClass in
  Assets.SimpleAndroidNotifications/NotificationManager.cs (used by LocalPushManager). Needs a Windows stub.
- Android-only C#: GameController.Update / GameQuitDialog (back button = Esc, gated on RuntimePlatform.Android).
- Input: Input.GetMouseButton*/mousePosition plus uGUI. No Input.touches, so mouse already works.
- Save: AES-128-CBC (key/IV hardcoded in GameDataCrypter) JSON at persistentDataPath/FixlSavedata.json.
- Network: playable-date check only (patched in offline build); Unity services disabled.
- Portrait 1080x1920 (defaultScreenOrientation 0 = portrait).
- Windows: IL2CPP for Windows was added in 2018.2, so Mono is required. 2017.2 builds Windows x86 + x86_64 out of the box.

## Plan (Phase 4+)
AssetRipper -> Unity 2017.2.3f1 project (port/project) -> stub notifications, Esc = back, windowed portrait
-> build Windows x86 (port/build/win-x86) -> test.

## Unity 6 upgrade (branch unity6, 2026-11-22)
- Worktree port-unity6, Unity 6000.6.10f1. Upgrade compiled except PortBuild (BuildReport API); fixed.
- Splash off, APK icon set, unused service packages removed, Unity Connect off.
- Found via user report: room background (bg_01/bg_02) was never drawn in the 2017 build and drawn ~250 px low in Unity 6. Cause: Resources.Load<Sprite>(texture) vs exported "<texture> 0" sprites. Fixed with PortSprites.Load.
- Verified: x64 + x86 builds, tours en/ja/x86 (57 shots each, only the known GET_COSTUME tour NRE), no splash in launch captures, exe icon extracted and checked.
- Code cleanup (2026-11-22): folders flattened (Scripts/Game, Scripts/SimpleAndroidNotifications), AssemblyInfo + NotificationExample removed, deprecated APIs replaced (0 warnings), decompiler locals renamed in ~90 methods, inverted ternaries flipped, empty Unity callbacks removed, GrowthDatabase loaders folded, DateTimeObserber/GameDataCrypter/SaveData IO rewritten. Verified: x64+x86 build clean; tours en/ja/x86 shots 1-36 pixel-identical to before, later shots differ only by animation/clock/elapsed-time hearts; old/new time logic equal on 400k random cases; save loads and new save decrypts with original key (same 34 keys).
- Removed the screenshot tour (PortTour, -porttour) and its tour-only helpers (Localization/PortDebug.OverrideForSession, PortDebugPanel.ShowTab/SetOverlay).
