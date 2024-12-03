namespace JamForge.Logging
{
    public interface ILogFormatter
    {
        string FormatMessage(Logger logger, LogLevel logLevel, string message);
    }
}
