using JamForge.Logging;
using UnityEngine;
using VContainer;
using ILogger = JamForge.Logging.ILogger;

namespace JamForge
{
    internal static class InternalLog
    {
        internal static ILogger Logger { get; set; }
        
#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static void Trace(string message) => Logger.Trace(message);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static void Debug(string message) => Logger.Debug(message);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static void Info(string message) => Logger.Info(message);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static void Warn(string message) => Logger.Warn(message);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static void Error(string message) => Logger.Error(message);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public static void Fatal(string message) => Logger.Fatal(message);
        
        public static void Assert(bool condition, string message) => Logger.Assert(condition, message);
    }

    public partial class Jam
    {
        #region Logs

        [Inject]
        private ILogger _logger;

        [Inject]
        private ILogManager _logManager;

        public static ILogger Logger => Instance._logger;

        public static ILogManager LogManager => Instance._logManager;

        #endregion
    }
}
