using JamForge.Messages;

public class GameStartMessage : IMessage
{
    public string GameName { get; }

    public GameStartMessage(string gameName)
    {
        GameName = gameName;
    }
}