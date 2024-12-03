using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using VContainer;

namespace JamForge.Store
{
    [UnityEngine.Scripting.Preserve]
    public class InMemoryDictionaryStore : IMemoryStoreVendor
    {
        public int Count => _stores.Count;

        private readonly IObjectResolver _resolver;
        private readonly Dictionary<string, DictionaryMemoryStore> _stores;

        public InMemoryDictionaryStore(IObjectResolver resolver)
        {
            _resolver = resolver;
            _stores = new Dictionary<string, DictionaryMemoryStore>();
        }

        public IStore Get(string storeName)
        {
            if (_stores.TryGetValue(storeName, out var store))
            {
                return store;
            }

            store = _resolver.Resolve<DictionaryMemoryStore>();
            _stores[storeName] = store;

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
        }

        [UsedImplicitly]
        public class DictionaryMemoryStore : IStore
        {
            public string StoreName { get; set; }
            
            private readonly List<IDictionary> _dictionaries = new();
            private readonly Dictionary<Type, IDictionary> _typedDictionaries = new();
            // private readonly Dictionary<string, byte[]> _store;

            public void Set<T>(string key, T value)
            {
                var typedDictionary = GetOrCreateDictionary<T>();
                typedDictionary[key] = value;
            }

            public T Get<T>(string key)
            {
                var typedDictionary = GetOrCreateDictionary<T>();
                if (typedDictionary.TryGetValue(key, out var value))
                {
                    return value;
                }

                throw new KeyNotFoundException($"Key {key} not found");
            }

            public bool Has(string key)
            {
                for (var i = 0; i < _dictionaries.Count; i++)
                {
                    var dictionary = _dictionaries[i];
                    if (dictionary.Contains(key))
                    {
                        return true;
                    }
                }
                
                return false;
            }

            public bool Has<T>(string key)
            {
                var typedDictionary = GetOrCreateDictionary<T>();
                return typedDictionary.ContainsKey(key);
            }

            public bool TryGet<T>(string key, out T value)
            {
                var typedDictionary = GetOrCreateDictionary<T>();
                if (typedDictionary.TryGetValue(key, out value))
                {
                    return true;
                }

                value = default;
                return false;
            }

            public bool Delete(string key)
            {
                for (var i = 0; i < _dictionaries.Count; i++)
                {
                    var dictionary = _dictionaries[i];
                    if(dictionary.Contains(key))
                    {
                        dictionary.Remove(key);
                        return true;
                    }
                }

                return true;
            }

            public bool Delete<T>(string key)
            {
                var typedDictionary = GetOrCreateDictionary<T>();
                return typedDictionary.Remove(key);
            }

            public void DeleteAll()
            {
                foreach (var dictionary in _dictionaries)
                {
                    dictionary.Clear();
                }
            }

            private Dictionary<string, T> GetOrCreateDictionary<T>()
            {
                var type = typeof(T);
                if (_typedDictionaries.TryGetValue(type, out var dictionary))
                {
                    return dictionary as Dictionary<string, T>;
                }

                dictionary = new Dictionary<string, T>();
                _typedDictionaries[type] = dictionary;
                _dictionaries.Add(dictionary);

                return (Dictionary<string, T>)dictionary;
            }
        }
    }
}
