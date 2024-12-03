using JamForge.Serialization;
using VContainer;

namespace JamForge
{
    public partial class Jam
    {
        [Inject]
        private IJsonSerializer _json;

        public static IJsonSerializer Json => Instance._json;
    }
}
