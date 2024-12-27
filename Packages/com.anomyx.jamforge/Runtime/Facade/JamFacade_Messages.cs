using JamForge.Messages;
using VContainer;

namespace JamForge
{
    public partial class Jam
    {
        #region Events

        [Inject]
        private IMessageCenter _messageCenter;

        public static IMessageCenter Messages => Instance._messageCenter;

        #endregion
    }
}