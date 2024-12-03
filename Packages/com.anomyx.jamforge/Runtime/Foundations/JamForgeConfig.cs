using UnityEngine;

namespace JamForge
{
    public class JamForgeConfig : ScriptableObject
    {
        [Header("Logging")]
        [SerializeField] private bool persistLogs;
        [SerializeField] private bool timestampFormatted;
        [SerializeField] private string[] additionalFormatters;

        public bool PersistLogs => persistLogs;
        public bool TimestampFormatted => timestampFormatted;
        public string[] AdditionalFormatters => additionalFormatters;

        public static JamForgeConfig Create()
        {
            return CreateInstance<JamForgeConfig>();
        }

        public static JamForgeConfig Load()
        {
            return Resources.Load<JamForgeConfig>(nameof(JamForgeConfig));
        }
    }
}
