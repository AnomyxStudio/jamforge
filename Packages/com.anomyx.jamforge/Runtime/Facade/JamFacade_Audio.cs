using JamForge.Audio;
using VContainer;

namespace JamForge
{
    public partial class Jam
    {
        [Inject]
        private IAudioController _audioController;
        
        public static IAudioController Audio => Instance._audioController;
    }
}
