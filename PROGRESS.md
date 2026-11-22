# Windows port: progress log

## Status
Phases 1–3 (investigation + install checklist) are DONE (2026-11-22).
Waiting for the user to confirm that the tools are installed before Phase 4.

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
