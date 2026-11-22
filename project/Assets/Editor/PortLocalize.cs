// [PC port] Editor batch step for the English localization.
//   Unity.exe -batchmode -quit -projectPath <project> -executeMethod PortLocalize.Run -logFile <log>
//
// 1. Adds the language row (BtnOption_7) to Resources/AreaButtonVertical, cloned from the SE row.
// 2. Adds LocalizedStaticText to every UI Text whose typed-in Japanese is in static_text_en.json
//    (prefabs + build scenes). Re-running is safe: existing components are updated.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class PortLocalize
{
    [Serializable]
    private class Entry
    {
        public string ja;
        public string en;
    }

    [Serializable]
    private class Table
    {
        public Entry[] entries;
    }

    private const string TablePath = "Assets/Editor/static_text_en.json";
    private const string OptionPrefabPath = "Assets/Resources/AreaButtonVertical.prefab";
    private static readonly Regex Japanese = new Regex("[぀-ヿ一-鿿＀-￯]");

    private static Dictionary<string, string> table_;
    private static readonly List<string> untranslated_ = new List<string>();
    private static int added_;

    public static void Run()
    {
        LoadTable();
        AddLanguageToggle();
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null && LocalizeTexts(prefab.GetComponentsInChildren<Text>(true), path) > 0)
            {
                EditorUtility.SetDirty(prefab);
            }
        }
        foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
        {
            Scene scene = EditorSceneManager.OpenScene(s.path, OpenSceneMode.Single);
            int n = 0;
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                n += LocalizeTexts(root.GetComponentsInChildren<Text>(true), s.path);
            }
            if (n > 0)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log("[PortLocalize] LocalizedStaticText added/updated: " + added_);
        foreach (string u in untranslated_)
        {
            Debug.Log("[PortLocalize] untranslated static text: " + u);
        }
        Debug.Log("[PortLocalize] done");
    }

    private static string Key(string s)
    {
        return s.Replace("\r", string.Empty).Trim();
    }

    private static void LoadTable()
    {
        Table t = JsonUtility.FromJson<Table>(File.ReadAllText(TablePath));
        table_ = new Dictionary<string, string>();
        foreach (Entry e in t.entries)
        {
            table_[Key(e.ja)] = e.en;
        }
    }

    private static int LocalizeTexts(Text[] texts, string where)
    {
        int n = 0;
        foreach (Text text in texts)
        {
            if (string.IsNullOrEmpty(text.text) || !Japanese.IsMatch(text.text))
            {
                continue;
            }
            string en;
            if (!table_.TryGetValue(Key(text.text), out en))
            {
                untranslated_.Add(where + " / " + text.gameObject.name + " : " + text.text.Replace("\r", "").Replace("\n", "\\n"));
                continue;
            }
            LocalizedStaticText loc = text.GetComponent<LocalizedStaticText>();
            if (loc == null)
            {
                loc = text.gameObject.AddComponent<LocalizedStaticText>();
            }
            loc.Setup(text.text, en);
            EditorUtility.SetDirty(loc);
            n++;
            added_++;
        }
        return n;
    }

    private static void AddLanguageToggle()
    {
        AddOptionRow("BtnOption_7", typeof(LanguageSettingToggle));
        AddOptionRow("BtnOption_8", typeof(DebugSettingToggle));
    }

    // Clone the SE toggle row (BtnOption_6) as a new last row with a different toggle script.
    private static void AddOptionRow(string rowName, Type toggleType)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(OptionPrefabPath);
        if (prefab.transform.Find(rowName) != null)
        {
            Debug.Log("[PortLocalize] " + rowName + " already present");
            return;
        }
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        Transform se = instance.transform.Find("BtnOption_6");
        GameObject row = UnityEngine.Object.Instantiate(se.gameObject, instance.transform, false);
        row.name = rowName;
        row.transform.SetAsLastSibling();
        UnityEngine.Object.DestroyImmediate(row.GetComponent<SeSettingToggle>());
        row.AddComponent(toggleType);
        PrefabUtility.ReplacePrefab(instance, prefab, ReplacePrefabOptions.ConnectToPrefab);
        UnityEngine.Object.DestroyImmediate(instance);
        Debug.Log("[PortLocalize] added " + rowName + " (" + toggleType.Name + ")");
    }
}
