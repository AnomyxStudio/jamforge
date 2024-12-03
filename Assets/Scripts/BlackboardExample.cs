using UnityEngine;
using JamForge.Blackboards;

/// <summary>
/// Example demonstrating the usage of the Blackboard system for storing and managing game state.
/// Shows how to:
/// - Create and initialize a blackboard
/// - Set and get values of different types
/// - Subscribe to value change events
/// - Handle key creation and removal events
/// </summary>
public class BlackboardExample : MonoBehaviour
{
    private Blackboard _blackboard;
    
    private void Awake()
    {
        // Initialize the blackboard
        _blackboard = new Blackboard();
        
        // Subscribe to blackboard events
        _blackboard.OnKeyCreated += OnBlackboardKeyCreated;
        _blackboard.OnKeyRemoved += OnBlackboardKeyRemoved;
    }

    private void Start()
    {
        // Example: Store player stats
        _blackboard.SetValue("PlayerHealth", 100);
        _blackboard.SetValue("PlayerName", "Hero");
        _blackboard.SetValue("IsInvincible", false);
        
        // Example: Read values
        if (_blackboard.HasKey("PlayerHealth"))
        {
            int health = _blackboard.GetValue<int>("PlayerHealth");
            Debug.Log($"Player Health: {health}");
        }
        
        // Example: Safe value retrieval
        if (_blackboard.TryGetValue("PlayerName", out string playerName))
        {
            Debug.Log($"Player Name: {playerName}");
        }
        
        // Example: Update value
        _blackboard.SetValue("PlayerHealth", 90); // This will trigger the value changed event
    }
    
    private void OnBlackboardKeyCreated(string key)
    {
        Debug.Log($"New key created in blackboard: {key}");
    }
    
    private void OnBlackboardKeyRemoved(string key)
    {
        Debug.Log($"Key removed from blackboard: {key}");
    }
    
    // Example: Using blackboard in game mechanics
    public void DamagePlayer(int damage)
    {
        if (_blackboard.TryGetValue("IsInvincible", out bool isInvincible) && isInvincible)
        {
            Debug.Log("Player is invincible, no damage taken!");
            return;
        }
        
        int currentHealth = _blackboard.GetValue<int>("PlayerHealth");
        int newHealth = Mathf.Max(0, currentHealth - damage);
        _blackboard.SetValue("PlayerHealth", newHealth);
        
        Debug.Log($"Player took {damage} damage. Health: {newHealth}");
    }
}
