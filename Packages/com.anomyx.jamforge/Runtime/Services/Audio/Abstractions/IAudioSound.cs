using UnityEngine;

namespace JamForge.Audio
{
    public interface IAudioSound : IAudioBase, IAudioSettings
    {
        AudioClip GetClip();
    }
}
