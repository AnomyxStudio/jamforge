using JamForge;
using UnityEngine;

/// <summary>
/// Demonstrates store usage for memory and persistent data management using JamForge's store system.
/// This example shows how to:
/// - Access memory and persistent stores
/// - Set and retrieve values from stores
/// - Handle missing keys gracefully
/// </summary>
public class StoreExample : MonoBehaviour
{
    private void Start()
    {
        // Access the memory store
        var store = Jam.Stores.Memory.Get("TestStore");

        // Example: Set a value in the memory store
        store.Set("TestKey", "TestValue");
        Jam.Logger.Debug(store.TryGet<string>("TestKey", out var value) ? value : "No value found for TestKey");

        // Access the persistent store
        var persistStore = Jam.Stores.Persist.Get("TestPersistStore");

        // Example: Set a value in the persistent store
        persistStore.Set("TestKey", "TestValue");
        Jam.Logger.Debug(persistStore.TryGet("TestKey", out value) ? value : "No value found for TestKey");

        // Additional Example: Remove a key from the memory store
        store.Delete("TestKey");
        Jam.Logger.Debug(store.TryGet<string>("TestKey", out value) ? value : "Key removed successfully");

        // Additional Example: Clear all keys from the persistent store
        persistStore.DeleteAll();
        Jam.Logger.Debug(persistStore.TryGet("TestKey", out value) ? value : "All keys cleared");
    }
}
