using System;

namespace JamForge.Messages
{
    public interface IMessageCenter
    {
        void Subscribe<T>(Action<T> handler) where T : IMessage;
        void Unsubscribe<T>(Action<T> handler) where T : IMessage;
        void Publish<T>(T message, bool async = false) where T : IMessage;
        void Clear();
    }
}