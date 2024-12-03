namespace JamForge.Store
{
    public interface IMemoryStoreVendor : IStoreVendor { }
    
    public interface IPersistStoreVendor : IStoreVendor { }

    public interface IStoreVendor
    {
        int Count { get; }

        IStore Get(string storeName);

        bool Has(string storeName);

        void Destroy(string storeName);
    }
}
