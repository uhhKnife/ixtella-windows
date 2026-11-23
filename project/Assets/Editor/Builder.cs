// Unity.exe -batchmode -quit -projectPath <project> -executeMethod Builder.BuildWin64 -logFile <log>
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class Builder
{
    private const string ExeName = "IkustellaLink.exe";
    private const string IconPath = "Assets/PC/app_icon.png";

    public static void BuildWin32() { Build(BuildTarget.StandaloneWindows, "win-x86"); }
    public static void BuildWin64() { Build(BuildTarget.StandaloneWindows64, "win-x64"); }

    private static void Build(BuildTarget target, string folder)
    {
        PlayerSettings.SplashScreen.show = false;
        PlayerSettings.SplashScreen.showUnityLogo = false;

        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath);
        if (icon == null)
        {
            Debug.LogError("[Builder] icon missing: " + IconPath);
            EditorApplication.Exit(1);
            return;
        }
        PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new Texture2D[] { icon }, IconKind.Any);

        string[] scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        string outDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../../build/" + folder));
        if (Directory.Exists(outDir)) Directory.Delete(outDir, true);
        Directory.CreateDirectory(outDir);
        Debug.Log("[Builder] target=" + target + " scenes=" + string.Join(", ", scenes) + " out=" + outDir);

        BuildReport report = BuildPipeline.BuildPlayer(scenes, Path.Combine(outDir, ExeName), target, BuildOptions.None);
        if (report.summary.result != BuildResult.Succeeded)
        {
            Debug.LogError("[Builder] FAILED: " + report.summary.result + " errors=" + report.summary.totalErrors);
            EditorApplication.Exit(1);
            return;
        }
        Debug.Log("[Builder] OK: " + outDir + " splash=" + PlayerSettings.SplashScreen.show + " size=" + report.summary.totalSize);
    }
}
