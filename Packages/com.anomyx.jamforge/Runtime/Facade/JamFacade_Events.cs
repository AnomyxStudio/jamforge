using JamForge.Events;
using VContainer;

namespace JamForge
{
    public partial class Jam
    {
        #region Events

        [Inject]
        private IEventBrokerFacade _eventBroker;

        public static IEventBrokerFacade Events => Instance._eventBroker;

        #endregion
    }
}
