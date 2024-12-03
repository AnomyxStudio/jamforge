using System.Collections.Generic;
using UnityEngine;

namespace JamForge.Logging
{
    public abstract class FormattedAppender : ILogDelegate
    {
        private readonly List<ILogFormatter> _formatters;

        protected FormattedAppender(List<ILogFormatter> formatters)
        {
            _formatters = formatters;
        }
#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        public void WriteLine(Logger logger, LogLevel logLevel, string message)
        {
            for (var i = 0; i < _formatters.Count; i++)
            {
                var formatter = _formatters[i];
                message = formatter.FormatMessage(logger, logLevel, message);
            }

            WriteFormattedLine(logger, logLevel, message);
        }

        protected abstract void WriteFormattedLine(Logger logger, LogLevel logLevel, string message);
    }
}
