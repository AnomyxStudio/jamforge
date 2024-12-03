namespace JamForge.Audio
{
    public interface IAudioHandle
    {
        public bool IsValid { get; }

        public bool IsPlaying { get; }
        
        public void Stop();

        public void Pause();

        public void Resume();
    }
}
