using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace JamForge.Utils.Audios
{
    namespace Utilities
    {
        public class AnimalCrossingSpeech : MonoBehaviour
        {
            [Header("Display")]
            [SerializeField] private float charsPerSec = 20f;

            [Header("Audio")]
            [SerializeField] private AudioSource audioSrc;
            [SerializeField] private List<AudioClip> speechSounds;
            [SerializeField] private int soundInterval = 1;
            [SerializeField] private float minPitch = 0.8f;
            [SerializeField] private float maxPitch = 1.2f;

            public event Action<string> TextDisplayEvent;

            // Events using Action
            public event Action OnStart;
            public event Action<char> OnChar;
            public event Action<char> OnSound;
            public event Action OnComplete;
            public event Action OnInterrupted;

            private string targetText = "";
            private string currentText = "";
            private CancellationTokenSource cts;
            private bool isPaused;
            private bool isProcessing;

            // State properties
            public bool IsPlaying => isProcessing;
            public bool IsPaused => isPaused;
            public float Progress => targetText.Length > 0 ? (float)currentText.Length / targetText.Length : 0f;
            public string CurrentText => currentText;
            public string TargetText => targetText;
            public float Duration => targetText.Length > 0 ? targetText.Length / charsPerSec : 0f;

            private void Awake()
            {
                audioSrc ??= GetComponentInChildren<AudioSource>();
                ValidateSetup();
            }

            private void OnDestroy()
            {
                StopSpeech();
            }

            private void ValidateSetup()
            {
                if (TextDisplayEvent == null)
                {
                    Debug.LogWarning("TextDisplayEvent is not hooked up");
                }

                if (!audioSrc)
                {
                    throw new MissingComponentException("AudioSource component is required");
                }

                if (speechSounds == null || speechSounds.Count == 0)
                {
                    Debug.LogWarning("No speech sounds assigned");
                }
            }

            public async UniTask<bool> SpeakAsync(string text, CancellationToken externalToken = default)
            {
                if (string.IsNullOrEmpty(text)) return false;

                // Handle concurrent speech requests
                if (isProcessing)
                {
                    OnInterrupted?.Invoke();
                    StopSpeech();
                    // Small delay to ensure cleanup is complete
                    await UniTask.Yield(PlayerLoopTiming.Update);
                }

                isProcessing = true;
                cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
                targetText = text;
                currentText = "";

                try
                {
                    await SpeechLoopAsync(cts.Token);
                    return true;
                }
                catch (OperationCanceledException)
                {
                    return false;
                }
                finally
                {
                    cts?.Dispose();
                    cts = null;
                    isProcessing = false;
                }
            }

            private async UniTask SpeechLoopAsync(CancellationToken token)
            {
                OnStart?.Invoke();

                var charCount = 0;
                var charDelay = 1f / charsPerSec;

                foreach (var c in targetText)
                {
                    token.ThrowIfCancellationRequested();

                    while (isPaused)
                    {
                        await UniTask.Yield(token);
                    }

                    currentText += c;
                    TextDisplayEvent?.Invoke(currentText);
                    OnChar?.Invoke(c);

                    charCount++;
                    if (charCount % soundInterval == 0)
                    {
                        PlaySound(c);
                    }

                    await UniTask.Delay(TimeSpan.FromSeconds(charDelay), cancellationToken: token);
                }

                OnComplete?.Invoke();
            }

            private void PlaySound(char character)
            {
                if (speechSounds == null || speechSounds.Count == 0) return;

                var hash = character.GetHashCode();

                // Deterministic sound selection
                var soundIndex = Mathf.Abs(hash) % speechSounds.Count;
                var clip = speechSounds[soundIndex];

                // Deterministic pitch variation
                var normalizedHash = (hash % 1000) / 1000f;
                var pitch = Mathf.Lerp(minPitch, maxPitch, normalizedHash);

                audioSrc.pitch = pitch;
                audioSrc.PlayOneShot(clip);

                OnSound?.Invoke(character);
            }

            public void StopSpeech()
            {
                if (cts?.IsCancellationRequested == false)
                {
                    cts.Cancel();
                    cts.Dispose();
                    cts = null;
                }
            }

            public void Pause() => isPaused = true;
            public void Resume() => isPaused = false;

            public async UniTask<bool> SkipToEndAsync()
            {
                if (!isProcessing) return false;

                StopSpeech();
                currentText = targetText;
                TextDisplayEvent?.Invoke(currentText);
                OnComplete?.Invoke();
                isProcessing = false;

                return true;
            }

            // Configuration methods
            public void SetSpeed(float charsPerSecond) =>
                charsPerSec = Mathf.Max(0.1f, charsPerSecond);

            public void SetSoundInterval(int interval) =>
                soundInterval = Mathf.Max(1, interval);

            public void SetPitchRange(float min, float max)
            {
                minPitch = Mathf.Min(min, max);
                maxPitch = Mathf.Max(min, max);
            }
        }
    }
}