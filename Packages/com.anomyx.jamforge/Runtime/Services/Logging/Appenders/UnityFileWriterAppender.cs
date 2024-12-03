using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JamForge.Logging
{
    public class UnityFileWriterAppender : FormattedAppender
    {
        private readonly object _lock = new();
        private readonly string _filePath;

        public UnityFileWriterAppender(string filePath, List<ILogFormatter> formatters) : base(formatters)
        {
            _filePath = Path.Combine(Application.persistentDataPath, filePath);
        }

        protected override void WriteFormattedLine(Logger logger, LogLevel logLevel, string message)
        {
            lock (_lock)
            {
                using var writer = new StreamWriter(_filePath, true);
                writer.WriteLine(message);
            }
        }
    }
}
