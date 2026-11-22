# フェイト／育ステラ りんく: native Windows port

A Windows build of the Android game, rebuilt from the original APK data.

This is the `unity6` branch: the project upgraded to **Unity 6000.6.10f1** (Mono), with no
"Made with Unity" splash (allowed on Personal since Unity 6) and the APK's app icon. The `master`
branch keeps the Unity 2017.2.3f1 version.

**Run:** `build/win-x64/IkustellaLink.exe` (or `win-x86` for 32-bit; copy the whole folder to move it).
Mouse = touch, drag = swipe, Esc on the title screen = Android back (quit dialog).
Save: `%USERPROFILE%\AppData\LocalLow\Marvelous Inc_\育ステラ\FixlSavedata.json`
(same AES-encrypted format as the phone).

## Layout
| Path | What |
|---|---|
| `project/` | Unity 6 project (AssetRipper 2.0.0 export + fixes below) |
| `project/Assets/Scripts/Game/` | The game's own scripts |
| `project/Assets/Scripts/SimpleAndroidNotifications/` | Notification plugin (Android-only calls) |
| `project/Assets/Scripts/PcPort/` | PC additions: language, debug panel, sprite loading, screenshot tour |
| `project/Assets/Editor/PortBuild.cs` | Batch build entry points (x86 / x64) |
| `build/` | Build output (not in git) |
| `logs/` | Unity / player logs and test screenshots (not in git) |
| `PROGRESS.md` | Progress log |

## Build
Close the Unity editor, then run `build.bat` (or double-click it):
```
build.bat          64-bit  -> build\win-x64\IkustellaLink.exe
build.bat x86      32-bit  -> build\win-x86\
build.bat all      both
```
It expects Unity 6000.6.10f1 at `D:\Unity\Unity\6000.6.10f1`; elsewhere, `set UNITY=C:\path\to\Unity.exe`
first. Logs go to `logs\build-x64.log` / `build-x86.log`. The build turns the splash off and sets the
icon (`project/Assets/PcPort/app_icon.png`, from the APK's `res/drawable-xxxhdpi-v4`).

Editing in Visual Studio: open `project` in the Unity editor, set Edit > Preferences > External Tools >
External Script Editor to Visual Studio, then Assets > Open C# Project (Unity generates the .sln).

## Changes from the original game
The game logic is the original code, decompiled by AssetRipper and then tidied for reading:
decompiler-generated local names (`num2`, `flag`, `component`, ...) renamed, inverted
`((!x) ? a : b)` conditions flipped, duplicated loaders folded into one, empty Unity callbacks and
the plugin's unused example removed. Behaviour is unchanged: the rewritten time-away logic was
checked against the original on 400,000 random cases, and the save format is identical (old saves
load, new saves decrypt with the original key). Functional changes:
1. `AppliPlayableStateChecker.GetPlayableFlagOnCache()` returns true. The unlock server is gone
   (same change as the offline APK).
2. `NotificationManager` (Simple Android Notifications): Android plugin calls are compiled only
   for Android. On PC, scheduling or cancelling a reminder only updates the id list.
3. `GameController.Update`: Esc opens the quit dialog on Windows too (was Android-only).
4. Player settings: default window 540x960, windowed, resizable.

Export fixes (restoring the original data, not changing it):
- Scenes moved from `Assets/Scenes/Assets/Scenes/*.unity.unity` to `Assets/Scenes/`; build scene list GUIDs fixed.
- Removed two .NET Core DLLs that AssetRipper added (unused, and incompatible with Unity 2017).
- `Disabled.anim`: 4 animation paths AssetRipper couldn't name were recovered by matching their CRC32 hashes.
- Sprite geometry: AssetRipper exported 574 trimmed sprites with their trimmed rect as the
  sprite rect, with borders and pivot shifted. That broke 9-sliced and tiled images (a tiled button
  background generated >65000 vertices). The original rect, border, pivot and offsets are copied
  back from the APK.
- `PortSprites.Load`: the room backgrounds and baby sprites live in separate `<texture> 0`
  sprite assets after export. Loading them by texture name found nothing in Unity 2017 (no room
  background on the main screen) and a wrong full-texture sprite in Unity 6. They now load the
  original sprite, so the room fills the panel as on the phone.

Unity 6 upgrade: `BuildPlayer` report API in `PortBuild.cs`, `SaveAsPrefabAsset` in
`PortLocalize.cs`; unused packages the upgrader added (Purchasing, Analytics, Timeline, ...) removed
and Unity Connect left off, so the build stays offline.

## Languages (Japanese / English)
Options > Language switches between 日本語 and English. Switching rebuilds the title screen.
The choice is stored in PlayerPrefs (not the save). On first launch the game uses Japanese if
Windows' UI language is Japanese, otherwise English.
- `Resources/ixtella_system_textlist_en.txt`: English for all 113 text ids, loaded on top of the
  Japanese table (`TextDatabaseSystem`). It splits on the first comma only, so English can use commas.
- `Editor/static_text_en.json`: English for 30 strings typed into scenes and prefabs.
  `PortLocalize.Run` attaches `LocalizedStaticText` to them and adds the language row
  (BtnOption_7) to the options menu.
- Not translated, by choice: credits, terms of use and privacy policy, and logos and banners (images).
- Terminology: Master (マスター, as in the official English Fate releases), Tummy (おなか),
  Happiness (しあわせ), Strength / Intelligence / Magic (たいりょく / ちりょく / まりょく);
  Nero's 奏者 = "Praetor", Gilgamesh's 雑種 = "mongrel" (established English renderings).

Layout check: `IkustellaLink.exe -porttour -porttour-lang en|ja -porttour-out <dir>` opens every
menu, help page, dialog, choice dialog and game-over/clear screen and saves screenshots.
Back up `FixlSavedata.json` first: the tour changes game state in memory and the game may autosave.

## Debug mode (Options > Debug)
OFF by default and stored in PlayerPrefs like the language. With it ON:
- the game's own debug code runs (it was behind `Debug.isDebugBuild`, now behind `PortDebug.Enabled`):
  the DebugMenu overlay, keyboard shortcuts, resume and phase logs, and a parameter log on reset.
  Shortcuts: F1 overlay, F2 panel, Left/Right overlay mode, PageUp/PageDown speed, Home x100, End x1,
  Pause, Ctrl+Up skip 1000 s, hold F5/F6/F9... + Up/Down stats, Esc+Delete reset, ` log console.
- the DBG button (right edge) opens the panel. Its tabs are Info (overlay, log console, status),
  Time (speed, pause, skip), Stats, Costume (unlock or lock all, change costume, room),
  Day (force day or night, allow cooking), Anim (all actions, animations and icons), Misc (reset, save).
- Turning it OFF restores normal speed and hides everything.
The original overlay's labels are the developers' Japanese debug text (left as is).
DebugMenu's touch-swipe gestures are disabled on PC (they'd fire on every mouse drag).

## Known differences / notes
- Textures are stored uncompressed (164 MB build, against a 30 MB APK). 38 textures were ETC2 on the phone,
  so they keep the phone's compression artifacts. There is no higher-quality source.
- Log warning `Shader Unsupported: 'Hidden/VideoDecodeAndroid'`: Unity's Android-only video
  shader is in the always-included list. Harmless; the game has no video.
- No OS notifications on PC (the phone version scheduled care reminders).
