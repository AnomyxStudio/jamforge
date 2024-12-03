using UnityEngine.Scripting;

namespace JamForge.Logging
{
    [Preserve]
    public class StandardLogFormatter : ILogFormatter
    {
        private readonly string _format;
        
        private const string TraceColor = "#85B7B1";
        private const string DebugColor = "#917A98";
        private const string InfoColor = "#AFB866";
        private const string WarnColor = "#E6C071";
        private const string ErrorColor = "#CC6766";
        private const string FatalColor = "magenta";

        public StandardLogFormatter(string format = "[{1}] {0}: {2}") => _format = format;
        
        public static string GetColoredLevel(LogLevel logLevel)
        {
            var logLevelString = logLevel.ToString();
            return logLevel switch
            {
                LogLevel.Trace => logLevelString.Colorize(TraceColor),
                LogLevel.Debug => logLevelString.Colorize(DebugColor),
                LogLevel.Info => logLevelString.Colorize(InfoColor),
                LogLevel.Warn => logLevelString.Colorize(WarnColor),
                LogLevel.Error => logLevelString.Colorize(ErrorColor),
                LogLevel.Fatal => logLevelString.Colorize(FatalColor),
                _ => logLevelString
            };
        }

        public string FormatMessage(Logger logger, LogLevel logLevel, string message)
        {
            return string.Format(_format, logger.Name, GetColoredLevel(logLevel), message);
        }
    }
}
