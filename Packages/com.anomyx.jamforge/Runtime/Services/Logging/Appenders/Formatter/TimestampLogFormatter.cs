using System;
using System.Globalization;
using System.Text;
using UnityEngine.Scripting;

namespace JamForge.Logging
{
    [Preserve]
    public class TimestampLogFormatter : ILogFormatter
    {
        public string FormatMessage(Logger logger, LogLevel logLevel, string message)
        {
            var builder = new StringBuilder();
            builder.Append(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
            builder.Append(' ');
            builder.Append(message);
            return builder.ToString();
        }
    }
}
