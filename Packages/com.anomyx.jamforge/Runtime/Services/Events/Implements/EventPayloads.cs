using System;

namespace JamForge.Events
{
    public class Payloads
    {
        public static readonly Payloads Empty = new();

        public DateTime SentTime { get; set; }

        public Payloads()
        {
            SentTime = DateTime.Now;
        }
    }
}
