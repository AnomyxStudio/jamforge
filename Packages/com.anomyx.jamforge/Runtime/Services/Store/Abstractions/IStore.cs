namespace JamForge.Store
{
    public interface IStore
    {
        public string StoreName { get; }

        public void Set<T>(string key, T value);

        public T Get<T>(string key);
        
        public bool Has(string key);

        public bool Has<T>(string key);

        public bool TryGet<T>(string key, out T value);
        
        public bool Delete(string key);

        public bool Delete<T>(string key);

        public void DeleteAll();
    }
}
