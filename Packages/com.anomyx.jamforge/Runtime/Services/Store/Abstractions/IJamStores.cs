namespace JamForge.Store
{
    public interface IJamStores
    {
        public IPersistStoreVendor Persist { get; }
        
        public IMemoryStoreVendor Memory { get; }
    }
}
