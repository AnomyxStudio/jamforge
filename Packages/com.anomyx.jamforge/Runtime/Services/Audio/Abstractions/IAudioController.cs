using UnityEngine;

namespace JamForge.Audio
{
    public interface IAudioController : IAudioBase
    {
        public void PlayOneShot(IAudioSound sound);
        
        public void PlayOneShot(IAudioSound sound, Vector3 position);
        
        public IAudioHandle Play(IAudioSound sound, string channel = null);

        public void SetGlobalVolume(float volume);

        public void SetGlobalPitch(float pitch);

        public void StopAll();

        public void PauseAll();

        public void ResumeAll();
    }
}
