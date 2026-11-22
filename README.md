# フェイト／育ステラ りんく: native Windows port

A Windows x86 (32-bit) build of the Android game, rebuilt from the original APK data
with Unity 2017.2.3f1 (the game's own engine version), Mono backend, .NET 3.5 profile.

**Run:** `build/win-x86/IkustellaLink.exe` (copy the whole `win-x86` folder to move it).
Mouse = touch, drag = swipe, Esc on the title screen = Android back (quit dialog).
Save: `%USERPROFILE%\AppData\LocalLow\Marvelous Inc_\育ステラ\FixlSavedata.json`
(same AES-encrypted format as the phone).

## Layout
| Path | What |
|---|---|
| `project/` | Unity 2017.2.3f1 project (AssetRipper 2.0.0 export + fixes below) |
| `project/Assets/Editor/PortBuild.cs` | Batch build entry points (x86 / x64) |
| `tools/fix_sprites.py` | Restores sprite geometry from the APK (see below) |
| `tools/wincap.ps1` | Test helper: captures the game window to PNG |
| `build/` | Build output (not in git) |
| `logs/` | Unity / player logs and test screenshots (not in git) |
| `PROGRESS.md` | Progress log |

## Build
```
"C:\Program Files\Unity\Hub\Editor\2017.2.3f1\Editor\Unity.exe" -batchmode -quit ^
  -projectPath project -executeMethod PortBuild.BuildWin32 -logFile logs\build-x86.log
```
(`PortBuild.BuildWin64` for x64.) Output: `build/win-x86/`.

## Changes from the original game
All game logic is the original code (decompiled by AssetRipper and compiled unchanged with
Unity's C# 4 compiler), except:
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
- `tools/fix_sprites.py`: AssetRipper exported 574 trimmed sprites with their trimmed rect as the
  sprite rect, with borders and pivot shifted. That broke 9-sliced and tiled images (a tiled button
  background generated >65000 vertices). The original rect, border, pivot and offsets are copied
  back from the APK.

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
Back up `FixlSavedata.json` first: it changes game state in memory and the game may autosave.
`tools/contact_sheet.ps1 -Dir <dir>` tiles them for review.

## Known differences / notes
- Textures are stored uncompressed (164 MB build, against a 30 MB APK). 38 textures were ETC2 on the phone,
  so they keep the phone's compression artifacts. There is no higher-quality source.
- Log warning `Shader Unsupported: 'Hidden/VideoDecodeAndroid'`: Unity's Android-only video
  shader is in the always-included list. Harmless; the game has no video.
- No OS notifications on PC (the phone version scheduled care reminders).
