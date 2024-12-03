using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace JamForge.Logging
{
    [Preserve]
    public class Logger : ILogger
    {
        public event LogDelegate LogDelegate;

        public string Name { get; internal set; }

        public LogLevel LogLevel { get; internal set; }

        public Logger()
        {
            LogLevel = LogLevel.Debug;
        }

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public void Trace(string message) => Log(LogLevel.Trace, message);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public void Debug(string message) => Log(LogLevel.Debug, message);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public void Info(string message) => Log(LogLevel.Info, message);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public void Warn(string message) => Log(LogLevel.Warn, message);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public void Error(string message) => Log(LogLevel.Error, message);

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public void Fatal(string message) => Log(LogLevel.Fatal, message);

        public void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new JamAssertException(message);
            }
        }

#if JAMLOG_OFF
        [System.Diagnostics.Conditional("false")]
#endif
#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        internal void Log(LogLevel logLvl, string message)
        {
            if (logLvl >= LogLevel)
            {
                LogDelegate?.Invoke(this, logLvl, message);
            }
        }

        public void Reset() => LogDelegate = null;

        internal void SetAppender(LogDelegate logDelegate)
        {
            LogDelegate = logDelegate;
        }
    }
}

public class JamAssertException : Exception
{
    public JamAssertException(string message) : base(message) { }
}
