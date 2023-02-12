using System;
using UnityEngine.Scripting;

namespace JamForge.Events
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SubscribeAttribute : PreserveAttribute
    {
        public string Endpoint { get; set; } = EventBroker.DefaultEndpoint;

        public ThreadMode Mode { get; set; } = ThreadMode.Main;

        public short Priority { get; set; } = 0;
    }
}