using JamForge;
using JamForge.Events;
using UnityEngine;

/// <summary>
/// Demonstrates event handling using JamForge's event system.
/// This example shows how to:
/// - Register and unregister event handlers
/// - Fire events with payloads
/// - Use context menus for testing
/// </summary>
public class EventExample : MonoBehaviour
{
    // Custom payload class for demonstration
    public class TestPayload : Payloads { }

    private void Start()
    {
        // Register event handlers
        Jam.Events.Register(this); // Register using attribute-based subscription
        Jam.Events.Register<Payloads>(OnClickEvent2); // Register specific handler
        Jam.Events.Register<Payloads>(EventBroker.DefaultPath, OnClickEvent3); // Register with specific path

        // Fire an initial event
        OnClick();
    }

    private void OnDestroy()
    {
        // Unregister event handlers to prevent memory leaks
        Jam.Events.Unregister(this);
        Jam.Events.Unregister<Payloads>(OnClickEvent2);
        Jam.Events.Unregister<Payloads>(EventBroker.DefaultPath, OnClickEvent3);
    }

    [ContextMenu("Click")]
    public void OnClick()
    {
        // Fire a generic event with a basic payload
        Jam.Events.Fire(new Payloads());
    }

    [ContextMenu("Click2")]
    public void OnClick2()
    {
        // Fire an event with a custom payload
        Jam.Events.Fire(new TestPayload());
    }

    [Subscribe]
    private void OnClickEvent(Payloads payloads)
    {
        // Handle event with attribute-based subscription
        Jam.Logger.Debug("Event received!");
    }

    private void OnClickEvent2(Payloads payloads)
    {
        // Handle event with specific handler
        Jam.Logger.Debug("Event handler 2!");
    }

    private void OnClickEvent3(Payloads payloads)
    {
        // Handle event with specific path
        Jam.Logger.Debug("Event handler 3!");
    }
}
