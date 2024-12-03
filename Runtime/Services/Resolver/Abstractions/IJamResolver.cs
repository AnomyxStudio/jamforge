using System;
using UnityEngine;

namespace JamForge.Resolver
{
    public interface IJamResolver
    {
        public object Create(Type type);

        public T Create<T>() where T : class;

        public T Resolve<T>();

        public object Resolve(Type type);

        public void Inject<T>(T instance);

        public void InjectGameObject(GameObject gameObject);

        public T Instantiate<T>(T prefab) where T : Component;

        public T Instantiate<T>(T prefab, Transform parent) where T : Component;

        public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component;

        public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component;
    }
}
