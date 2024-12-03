using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace JamForge
{
    public sealed class LogInterceptor
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            _current = new LogInterceptor();
        }

        private static LogInterceptor _current;
        private static LogInterceptor Current => _current ??= new LogInterceptor();

        private readonly FieldInfo _activeTextInfo;
        private readonly FieldInfo _consoleWindowInfo;
        private readonly MethodInfo _setActiveEntry;
        private readonly object[] _setActiveEntryArgs;
        private object _consoleWindow;

        private LogInterceptor()
        {
            var consoleWindowType = Type.GetType("UnityEditor.ConsoleWindow,UnityEditor");
            _activeTextInfo = consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
            _consoleWindowInfo = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            _setActiveEntry = consoleWindowType.GetMethod("SetActiveEntry", BindingFlags.Instance | BindingFlags.NonPublic);
            _setActiveEntryArgs = new object[] { null };
        }

        [OnOpenAsset(0)]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            var instance = EditorUtility.InstanceIDToObject(instanceID);
            return AssetDatabase.GetAssetOrScenePath(instance).EndsWith(".cs") && Current.OpenAsset();
        }

        private bool TraceFilter(string stackTrace)
        {
            return true;
            // return stackTrace.Contains("[Info]") || stackTrace.Contains("[Warn]") || stackTrace.Contains("[Error]");
        }

        private bool PathFilter(string path)
        {
            return !path.Contains("UnityDebugAppender.cs") && !path.Contains("JamFacade_Log4Net.cs") && path.Contains(" (at ");
        }

        private bool OpenAsset()
        {
            var stackTrace = GetStackTrace();
            if (string.IsNullOrEmpty(stackTrace)) { return false; }

            if (!TraceFilter(stackTrace)) { return false; }

            var paths = stackTrace.Split('\n');

            return (from t in paths where PathFilter(t) select OpenScriptAsset(t)).FirstOrDefault();
        }

        private bool OpenScriptAsset(string path)
        {
            var startIndex = path.IndexOf(" (at ", StringComparison.Ordinal) + 5;
            var endIndex = path.IndexOf(".cs:", StringComparison.Ordinal) + 3;
            var filePath = path.Substring(startIndex, endIndex - startIndex);
            var lineStr = path.Substring(endIndex + 1, path.Length - endIndex - 2);
            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);

            if (asset == null) { return false; }
            if (!int.TryParse(lineStr, out var line)) { return false; }

            var consoleWindow = GetConsoleWindow();
            _setActiveEntry.Invoke(consoleWindow, _setActiveEntryArgs);

            EditorGUIUtility.PingObject(asset);
            AssetDatabase.OpenAsset(asset, line);
            return true;
        }

        private string GetStackTrace()
        {
            var consoleWindow = GetConsoleWindow();

            if (consoleWindow == null) { return ""; }

            if ((EditorWindow)consoleWindow != EditorWindow.focusedWindow) { return ""; }

            var value = _activeTextInfo.GetValue(consoleWindow);
            return value != null ? value.ToString() : "";
        }

        private object GetConsoleWindow()
        {
            return _consoleWindow ??= _consoleWindowInfo.GetValue(null);
        }
    }
}
