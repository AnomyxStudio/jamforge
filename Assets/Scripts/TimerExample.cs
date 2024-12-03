using UnityEngine;
using JamForge.Timers;

/// <summary>
/// Example demonstrating the usage of the Timer system for managing time-based game events.
/// Shows how to:
/// - Create and start timers
/// - Handle timer completion callbacks
/// - Create repeating timers
/// - Pause and resume timers
/// - Cancel running timers
/// </summary>
public class TimerExample : MonoBehaviour
{
    [SerializeField] private Timer serializableTimer;
    
    private Timer _countdownTimer;
    private Timer _repeatingTimer;
    
    private void Start()
    {
        // Example 0: Using a serializable timer
        serializableTimer.SetDuration(3f);
        serializableTimer.Start();
        
        // Example 1: Simple countdown timer (3 seconds)
        _countdownTimer = Timer.Create(3f, OnCountdownComplete);
        _countdownTimer.Start();
        
        // Example 2: Repeating timer that ticks every 1 second
        _repeatingTimer = Timer.Create(1f, OnTick, true);
        _repeatingTimer.Start();
        
        // Example 3: Timer with progress callback
        Timer.Create(5f, progress =>
        {
            Debug.Log($"Loading progress: {progress * 100}%");
        }, () =>
        {
            Debug.Log("Loading complete!");
        }).Start();
    }
    
    private void OnCountdownComplete()
    {
        Debug.Log("Countdown timer completed!");
        
        // Example 4: Create a delayed action
        Timer.Create(1f, () =>
        {
            Debug.Log("Delayed action executed!");
        }).Start();
    }
    
    private void OnTick()
    {
        Debug.Log("Tick!");
    }
    
    // Example: Game mechanic using timer
    public void StartPowerUpEffect()
    {
        Debug.Log("Power-up activated!");
        
        // Create a timer for power-up duration
        Timer powerUpTimer = Timer.Create(10f, () =>
        {
            Debug.Log("Power-up expired!");
        });
        
        // Store the timer reference if you need to cancel it early
        powerUpTimer.Start();
    }
    
    private void OnDestroy()
    {
        // Clean up timers when the component is destroyed
        _countdownTimer?.Cancel();
        _repeatingTimer?.Cancel();
    }
}
