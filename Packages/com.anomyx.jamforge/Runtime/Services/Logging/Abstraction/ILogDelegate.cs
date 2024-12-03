namespace JamForge.Logging
{
    public interface ILogDelegate
    {
        void WriteLine(Logger logger, LogLevel logLevel, string message);
    }
}
