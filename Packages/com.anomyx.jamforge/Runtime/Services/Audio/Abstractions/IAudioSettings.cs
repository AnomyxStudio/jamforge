namespace JamForge.Audio
{
    public interface IAudioSettings
    {
        bool Loop { get; }

        bool OverrideVolume { get; }

        bool OverridePitch { get; }

        float CrossFade { get; }
    }
}
