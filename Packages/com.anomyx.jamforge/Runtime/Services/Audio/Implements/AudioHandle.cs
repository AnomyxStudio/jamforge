using UnityEngine;
using UnityEngine.Pool;

namespace JamForge.Audio
{
    public class AudioHandle : IAudioHandle
    {
        public bool IsValid => AudioSource != null;

        public bool IsPlaying => AudioSource && AudioSource.isPlaying;

        private float CrossFade => SoundDefine.CrossFade;

        private AudioSource AudioSource { get; set; }

        private IAudioSound SoundDefine { get; }

        private IObjectPool<AudioSource> Pool { get; }

        public AudioHandle(AudioSource audioSource, IAudioSound soundDefine, IObjectPool<AudioSource> pool)
        {
            AudioSource = audioSource;
            SoundDefine = soundDefine;
            Pool = pool;

            PlayAudioClip();
        }

        public void Stop() => Dispose();

        public void Pause()
        {
            if (!IsValid) { return; }

            AudioSource.Pause();
        }

        public void Resume()
        {
            if (!IsValid) { return; }

            AudioSource.UnPause();
        }

        public void Dispose()
        {
            if (!IsValid) { return; }

            AudioSource.Stop();
            Pool.Release(AudioSource);
            AudioSource = null;
        }

        internal void Tick()
        {
            if (!IsValid) { return; }

            var remainTime = GetRemainTime();
            UpdateCrossFade(remainTime);

            if (remainTime > 0) { return; }
            if (!SoundDefine.Loop)
            {
                Dispose();
                return;
            }

            PlayAudioClip();
        }

        private void UpdateCrossFade(float remainTime)
        {
            if (CrossFade > remainTime)
            {
                var progress = (CrossFade - remainTime) / CrossFade;
                AudioSource.volume = Mathf.Lerp(SoundDefine.Volume, 0, progress);
            }
            else
            {
                var progress = AudioSource.time / CrossFade;
                AudioSource.volume = Mathf.Lerp(0, SoundDefine.Volume, progress);
            }
        }

        private void PlayAudioClip()
        {
            var audioClip = SoundDefine.GetClip();

            // Override settings
            if (SoundDefine.OverrideVolume) { AudioSource.volume = SoundDefine.Volume; }
            if (SoundDefine.OverridePitch) { AudioSource.pitch = SoundDefine.Pitch; }

            // Play
            AudioSource.clip = audioClip;
            AudioSource.Play();
        }

        public float GetRemainTime()
        {
            if (!IsValid) { return 0f; }

            var remainTime = AudioSource.clip.length / Mathf.Abs(AudioSource.pitch) - AudioSource.time;

            return remainTime > 0 ? remainTime : 0;
        }
    }
}
