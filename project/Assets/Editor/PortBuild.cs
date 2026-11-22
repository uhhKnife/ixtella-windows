// [PC port] Batch-mode build entry points.
//   Unity.exe -batchmode -quit -projectPath <project> -executeMethod PortBuild.BuildWin32 -logFile <log>
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class PortBuild
{
    private const string ExeName = "IkustellaLink.exe";

    public static void BuildWin32() { Build(BuildTarget.StandaloneWindows, "win-x86"); }
    public static void BuildWin64() { Build(BuildTarget.StandaloneWindows64, "win-x64"); }

    private static void Build(BuildTarget target, string folder)
    {
        string[] scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        string outDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../../build/" + folder));
        if (Directory.Exists(outDir)) Directory.Delete(outDir, true);
        Directory.CreateDirectory(outDir);
        Debug.Log("[PortBuild] target=" + target + " scenes=" + string.Join(", ", scenes) + " out=" + outDir);

        string error = BuildPipeline.BuildPlayer(scenes, Path.Combine(outDir, ExeName), target, BuildOptions.None);
        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError("[PortBuild] FAILED: " + error);
            EditorApplication.Exit(1);
            return;
        }
        Debug.Log("[PortBuild] OK: " + outDir);
    }
}
