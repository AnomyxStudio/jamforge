using System;
using UnityEngine;

namespace JamForge.Timers
{
    [Serializable]
    public class Timer
    {
        [SerializeField] private float duration;
        [SerializeField] private bool loop;
        public float Duration => duration;
        public float RemainingTime => Mathf.Max(0, duration - ElapsedTime);
        public float Progress => Mathf.Clamp01(ElapsedTime / duration);
        public float ElapsedTime { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsCompleted { get; private set; }

        public event Action<float> ProgressChanged;
        public event Action Started;
        public event Action Stopped;
        public event Action Completed;
        public event Action Paused;
        public event Action Resumed;

        private Action _onComplete;
        private Action<float> _onProgress;

        public Timer(float duration, bool loop = false)
        {
            this.duration = duration;
            this.loop = loop;
        }

        /// <summary>
        /// Creates a timer with the specified duration and completion callback.
        /// </summary>
        /// <param name="duration">Duration in seconds</param>
        /// <param name="onComplete">Callback when timer completes</param>
        /// <param name="loop">Whether the timer should loop</param>
        public static Timer Create(float duration, Action onComplete, bool loop = false)
        {
            var timer = new Timer(duration, loop);
            timer._onComplete = onComplete;
            return timer;
        }

        /// <summary>
        /// Creates a timer with progress updates and completion callback.
        /// </summary>
        /// <param name="duration">Duration in seconds</param>
        /// <param name="onProgress">Callback for progress updates (0-1)</param>
        /// <param name="onComplete">Callback when timer completes</param>
        /// <param name="loop">Whether the timer should loop</param>
        public static Timer Create(float duration, Action<float> onProgress, Action onComplete, bool loop = false)
        {
            var timer = new Timer(duration, loop);
            timer._onProgress = onProgress;
            timer._onComplete = onComplete;
            return timer;
        }

        public Timer SetDuration(float overrideDuration)
        {
            duration = overrideDuration;
            if (IsCompleted && ElapsedTime >= duration) { ElapsedTime = duration; }
            return this;
        }

        public Timer SetLoop(bool overrideLoop)
        {
            loop = overrideLoop;
            return this;
        }

        public void Start()
        {
            if (IsRunning || IsPaused) { return; }

            IsRunning = true;
            IsPaused = false;
            IsCompleted = false;
            TimerController.Instance.RegisterTimer(this);
            Started?.Invoke();
        }

        public void Stop()
        {
            if (!IsRunning && !IsPaused) { return; }

            IsRunning = false;
            IsPaused = false;
            IsCompleted = true;
            Stopped?.Invoke();
        }

        public void Cancel()
        {
            if (!IsRunning && !IsPaused) { return; }

            IsRunning = false;
            IsPaused = false;
            IsCompleted = false;
            ElapsedTime = 0f;
            TimerController.Instance.UnregisterTimer(this);
            Stopped?.Invoke();
        }

        public void Restart()
        {
            Cancel();
            Start();
        }

        public void Pause()
        {
            if (!IsRunning || IsPaused) { return; }

            IsRunning = false;
            IsPaused = true;
            Paused?.Invoke();
        }

        public void Resume()
        {
            if (!IsPaused) { return; }

            IsRunning = true;
            IsPaused = false;
            Resumed?.Invoke();
        }

        internal void UpdateTimer(float deltaTime)
        {
            if (!IsRunning) { return; }

            ElapsedTime += deltaTime;
            float currentProgress = Progress;
            ProgressChanged?.Invoke(currentProgress);
            _onProgress?.Invoke(currentProgress);

            if (ElapsedTime >= duration)
            {
                if (loop)
                {
                    ElapsedTime = 0f;
                    _onComplete?.Invoke();
                    Completed?.Invoke();
                }
                else
                {
                    IsRunning = false;
                    IsCompleted = true;
                    ElapsedTime = duration;
                    TimerController.Instance.UnregisterTimer(this);
                    _onComplete?.Invoke();
                    Completed?.Invoke();
                }
            }
        }
    }
}
