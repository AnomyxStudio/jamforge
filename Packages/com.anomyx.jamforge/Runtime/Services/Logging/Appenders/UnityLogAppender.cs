using System;
using System.Collections.Generic;
using UnityEngine;

namespace JamForge.Logging
{
    public class UnityLogAppender : FormattedAppender
    {
        public UnityLogAppender(List<ILogFormatter> formatters) : base(formatters) { }

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        protected override void WriteFormattedLine(Logger logger, LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                case LogLevel.Info:
                    Debug.Log(message);
                    return;
                case LogLevel.Warn:
                    Debug.LogWarning(message);
                    break;
                case LogLevel.Error:
                case LogLevel.Fatal:
                    Debug.LogError(message);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }
    }
}
