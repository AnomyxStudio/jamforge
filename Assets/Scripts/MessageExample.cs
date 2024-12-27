using System;
using JamForge;
using UnityEngine;

public class MessageExample : MonoBehaviour
{
    private Action<GameStartMessage> _handler;

    private void Start()
    {
        _handler = OnGameStarted;
        Jam.Messages.Subscribe(_handler);
        
        PublishGameStartMessage("Adventure Game");
    }

    private void PublishGameStartMessage(string gameName)
    {
        Jam.Messages.Publish(new GameStartMessage(gameName));
    }

    public void OnGameStarted(GameStartMessage value)
    {
        Debug.Log($"Received GameStartMessage: {value.GameName}");
    }

    private void OnDestroy()
    {
        Jam.Messages.Unsubscribe(_handler);
    }
}
