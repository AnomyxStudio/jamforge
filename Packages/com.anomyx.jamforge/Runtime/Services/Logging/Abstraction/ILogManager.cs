using System;

namespace JamForge.Logging
{
    public interface ILogManager
    {
        public void SetGlobalLogLevel(LogLevel logLevel);

        public void AddAppender(LogDelegate appender);

        public void RemoveAppender(LogDelegate appender);

        public void ClearAppenders();

        public ILogger GetLogger<T>();

        public ILogger GetLogger(Type type);

        public ILogger GetLogger(string name);
    }
}
