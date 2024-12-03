using JamForge.Store;
using UnityEngine.Scripting;

namespace JamForge
{
    [Preserve]
    public class JamStores : IJamStores
    {
        public IPersistStoreVendor Persist { get; }
        public IMemoryStoreVendor Memory { get; }

        public JamStores(IPersistStoreVendor persist, IMemoryStoreVendor memory)
        {
            Persist = persist;
            Memory = memory;
        }
    }
}
