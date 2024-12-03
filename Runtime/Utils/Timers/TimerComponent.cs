using UnityEngine;
using UnityEngine.Events;

namespace JamForge.Timers
{
    public class TimerComponent : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Timer timer;
        [SerializeField] private bool autoStart;

        [Header("Events")]
        [SerializeField] private UnityEvent onStart;
        [SerializeField] private UnityEvent onStop;
        [SerializeField] private UnityEvent onComplete;
        [SerializeField] private UnityEvent onPause;
        [SerializeField] private UnityEvent onResume;

        private void OnEnable()
        {
            if (timer == null) { return; }

            timer.Completed += OnTimerComplete;
            timer.Started += OnTimerStart;
            timer.Stopped += OnTimerStop;
            timer.Paused += OnTimerPause;
            timer.Resumed += OnTimerResume;
        }

        private void OnDisable()
        {
            if (timer == null) { return; }

            timer.Completed -= OnTimerComplete;
            timer.Started -= OnTimerStart;
            timer.Stopped -= OnTimerStop;
            timer.Paused -= OnTimerPause;
            timer.Resumed -= OnTimerResume;
            timer.Cancel();
        }

        private void Start()
        {
            if (timer == null) { return; }
            if (autoStart) { timer.Start(); }
        }

        public void SetDuration(float duration) => timer?.SetDuration(duration);
        public void SetLoop(bool loop) => timer?.SetLoop(loop);

        public void StartTimer() => timer?.Start();
        public void StopTimer() => timer?.Stop();
        public void CancelTimer() => timer?.Cancel();
        public void RestartTimer() => timer?.Restart();
        public void PauseTimer() => timer?.Pause();
        public void ResumeTimer() => timer?.Resume();

        private void OnTimerStart() => onStart?.Invoke();
        private void OnTimerStop() => onStop?.Invoke();
        private void OnTimerComplete() => onComplete?.Invoke();
        private void OnTimerPause() => onPause?.Invoke();
        private void OnTimerResume() => onResume?.Invoke();
    }
}
