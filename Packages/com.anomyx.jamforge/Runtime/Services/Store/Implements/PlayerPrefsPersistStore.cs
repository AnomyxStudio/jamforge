using System.Collections.Generic;
using System.Linq;
using JamForge.Serialization;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;

namespace JamForge.Store
{
    [UnityEngine.Scripting.Preserve]
    public class PlayerPrefsStoreVendor : IPersistStoreVendor
    {
        private const string StoreNamesKey = "JamForge.Stores.StoreNames";

        public int Count => _stores.Count;

        private readonly IObjectResolver _resolver;
        private readonly Dictionary<string, PlayerPrefsPersistStore> _stores;

        public PlayerPrefsStoreVendor(IObjectResolver resolver)
        {
            _resolver = resolver;
            _stores = new Dictionary<string, PlayerPrefsPersistStore>();

            var storeNames = PlayerPrefs.GetString(StoreNamesKey, "");
            if (string.IsNullOrEmpty(storeNames))
            {
                return;
            }

            var storeList = storeNames.Split(",");

            foreach (var storeName in storeList)
            {
                var storeInstance = resolver.Resolve<PlayerPrefsPersistStore>();
                storeInstance.StoreName = storeName;
                _stores[storeName] = storeInstance;
            }
        }

        public IStore Get(string storeName)
        {
            if (_stores.TryGetValue(storeName, out var store))
            {
                return store;
            }

            store = _resolver.Resolve<PlayerPrefsPersistStore>();
            store.StoreName = storeName;
            _stores[storeName] = store;

            var storeNames = PlayerPrefs.GetString(StoreNamesKey, "");
            if (string.IsNullOrEmpty(storeNames))
            {
                return store;
            }

            var storeList = storeNames.Split(",");
            var newArray = new string[storeList.Length + 1];
            storeList.CopyTo(newArray, 0);
            newArray[storeList.Length] = storeName;

            PlayerPrefs.SetString(StoreNamesKey, string.Join(",", newArray));
            PlayerPrefs.Save();

            return store;
        }

        public bool Has(string storeName)
        {
            return _stores.ContainsKey(storeName);
        }

        public void Destroy(string storeName)
        {
            if (!_stores.TryGetValue(storeName, out var store)) { return; }

            store.DeleteAll();
            _stores.Remove(storeName);

            var storeNames = PlayerPrefs.GetString(StoreNamesKey, "");
            if (string.IsNullOrEmpty(storeNames))
            {
                return;
            }

            var storeList = storeNames.Split(",").ToList();
            storeList.Remove(storeName);
            PlayerPrefs.SetString(StoreNamesKey, string.Join(",", storeList));
            PlayerPrefs.Save();
        }

        [UsedImplicitly]
        public class PlayerPrefsPersistStore : IStore
        {
            private readonly IJsonSerializer _json;

            private readonly Dictionary<string, string> _keyCaches;

            private readonly List<string> _keys;

            public PlayerPrefsPersistStore(IJsonSerializer json)
            {
                _json = json;
                _keyCaches = new Dictionary<string, string>();
                _keys = new List<string>();
            }

            public string StoreName { get; set; }

            private string ProcessKey(string key)
            {
                if (key.Contains(StoreName)) { return key; }

                if (_keyCaches.TryGetValue(key, out var cache))
                {
                    return cache;
                }

                cache = $"{StoreName}_{key}";
                _keyCaches[key] = cache;
                return cache;
            }

            public void Set<T>(string key, T value)
            {
                key = ProcessKey(key);

                if (!_keys.Contains(key)) { _keys.Add(key); }

                PlayerPrefs.SetString(key, _json.To(value));
                PlayerPrefs.Save();
            }

            public T Get<T>(string key)
            {
                key = ProcessKey(key);
                return _json.From<T>(PlayerPrefs.GetString(key));
            }

            public bool Has(string key)
            {
                key = ProcessKey(key);
                return PlayerPrefs.HasKey(key);
            }

            public bool Has<T>(string key)
            {
                key = ProcessKey(key);
                return PlayerPrefs.HasKey(key);
            }

            public bool TryGet<T>(string key, out T value)
            {
                key = ProcessKey(key);
                if (Has<T>(key))
                {
                    value = Get<T>(key);
                    return true;
                }

                value = default;
                return false;
            }

            public bool Delete(string key)
            {
                key = ProcessKey(key);
                if (!Has(key)) { return false; }

                PlayerPrefs.DeleteKey(key);
                PlayerPrefs.Save();
                return true;
            }

            public bool Delete<T>(string key)
            {
                key = ProcessKey(key);
                if (!Has<T>(key)) { return false; }

                PlayerPrefs.DeleteKey(key);
                PlayerPrefs.Save();
                return true;

            }

            public void DeleteAll()
            {
                foreach (var key in _keys)
                {
                    PlayerPrefs.DeleteKey(key);
                }

                PlayerPrefs.Save();
            }
        }
    }
}
