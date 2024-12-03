using JamForge.Store;
using VContainer;

namespace JamForge
{
    public partial class Jam
    {
        #region Stores

        [Inject] private IJamStores _stores;

        public static IJamStores Stores => Instance._stores;

        #endregion
    }
}
