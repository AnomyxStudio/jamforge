using System.Collections.Generic;
using UnityEngine;

namespace JamForge.Timers
{
    public class TimerController : MonoBehaviour
    {
        private static TimerController instance;
        private readonly List<Timer> _activeTimers = new();

        public static TimerController Instance
        {
            get
            {
                if (instance) { return instance; }

                var go = new GameObject("TimerController");
                instance = go.AddComponent<TimerController>();
                DontDestroyOnLoad(go);
                return instance;
            }
        }

        private void Awake()
        {
            if (instance && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            for (var i = _activeTimers.Count - 1; i >= 0; i--)
            {
                if (_activeTimers[i].IsRunning) { _activeTimers[i].UpdateTimer(Time.deltaTime); }
            }
        }

        public void RegisterTimer(Timer timer)
        {
            if (!_activeTimers.Contains(timer)) { _activeTimers.Add(timer); }
        }

        public void UnregisterTimer(Timer timer) => _activeTimers.Remove(timer);
    }
}
