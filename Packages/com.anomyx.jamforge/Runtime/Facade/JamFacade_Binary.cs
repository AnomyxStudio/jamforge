using JamForge.Serialization;
using VContainer;

namespace JamForge
{
    public partial class Jam
    {
        [Inject] private IBinarySerializer _binary;

        public static IBinarySerializer Binary => Instance._binary;
    }
}
