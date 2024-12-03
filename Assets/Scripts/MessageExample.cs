using System;
using JamForge;
using MessagePipe;
using UnityEngine;

/// <summary>
/// Demonstrates message subscription and handling using MessagePipe.
/// This example shows how to:
/// - Subscribe to messages
/// - Handle messages with custom logic
/// - Publish messages to trigger handlers
/// </summary>
public class MessageExample : MonoBehaviour
{
    private ISubscriber<GameStartMessage> _subscriber;
    private IPublisher<GameStartMessage> _publisher;
    
    private IDisposable disposable;

    private void Awake()
    {
        // Resolve subscriber and publisher for GameStartMessage
        _subscriber = Jam.Resolver.Resolve<ISubscriber<GameStartMessage>>();
        _publisher = Jam.Resolver.Resolve<IPublisher<GameStartMessage>>();
    }

    private void Start()
    {
        var bag = DisposableBag.CreateBuilder(); // composite disposable for manage subscription
        // Subscribe to GameStartMessage
        bag.Add(_subscriber.Subscribe(OnGameStarted));

        // Dispose the composite disposable when the component is destroyed
        disposable = bag.Build();
        
        // Example: Publish a message to trigger the subscriber
        PublishGameStartMessage("Adventure Game");
    }

    /// <summary>
    /// Handles GameStartMessage when received.
    /// </summary>
    /// <param name="value">The GameStartMessage payload</param>
    public void OnGameStarted(GameStartMessage value)
    {
        Jam.Logger.Debug($"Received GameStartMessage: {value.GameName}");
    }

    /// <summary>
    /// Publishes a GameStartMessage to notify subscribers.
    /// </summary>
    /// <param name="gameName">The name of the game to start</param>
    private void PublishGameStartMessage(string gameName)
    {
        var message = new GameStartMessage(gameName);
        _publisher.Publish(message);
    }

    // Additional Example: Unsubscribe from messages
    private void OnDestroy()
    {
        // Unsubscribe from GameStartMessage
        disposable?.Dispose();
    }
}