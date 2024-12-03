using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JamForge
{
    public class ScriptFinder
    {
        private const string ScriptExtension = ".cs";
        private static readonly Dictionary<string, string> Scripts = new();

        static ScriptFinder()
        {       
            Scripts.Clear();
            var paths = AssetDatabase.GetAllAssetPaths();
            for (var i = 0; i < paths.Length; i++)
            {
                if (!paths[i].EndsWith(ScriptExtension)) { continue; }
                var fileName = paths[i][(paths[i].LastIndexOf("/", StringComparison.Ordinal) + 1)..].Replace(ScriptExtension, "");
                Scripts.TryAdd(fileName, paths[i]);
            }
        }

        public static bool IsExistScript(string name)
        {
            if (Scripts.ContainsKey(name)) { return Scripts.ContainsKey(name); }

            var values = name.Split('.');
            name = values[^1];

            return Scripts.ContainsKey(name);
        }

        public static void OpenScript(string name)
        {
            if (!Scripts.ContainsKey(name))
            {
                var values = name.Split('.');
                name = values[^1];
            }

            if (Scripts.TryGetValue(name, out var script))
            {
                var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(script);
                if (monoScript)
                {
                    AssetDatabase.OpenAsset(monoScript);
                }
                else
                {
                    Debug.LogError($"ScriptFinder.OpenScript: {name}.cs is not found.");
                }
            }
            else
            {
                Debug.LogError($"ScriptFinder.OpenScript: {name}.cs is not found.");
            }
        }

        public static void OpenScript(string name, int lineNumber, int columnNumber)
        {
            if (!Scripts.ContainsKey(name))
            {
                var values = name.Split('.');
                name = values[^1];
            }

            if (Scripts.TryGetValue(name, out var script))
            {
                var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(script);
                if (monoScript)
                {
                    AssetDatabase.OpenAsset(monoScript, lineNumber, columnNumber);
                }
                else
                {
                    Debug.LogError($"ScriptFinder.OpenScript: {name}.cs is not found.");
                }
            }
            else
            {
                Debug.LogError($"ScriptFinder.OpenScript: {name}.cs is not found.");
            }
        }
    }
}
