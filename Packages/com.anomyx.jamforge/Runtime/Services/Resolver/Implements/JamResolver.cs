using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace JamForge.Resolver
{
    public class JamResolver: IJamResolver
    {
        private readonly IObjectResolver _resolver;

        public JamResolver(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        [UnityEngine.Scripting.Preserve]
        public void EnqueueScope(LifetimeScope scope)
        {
            LifetimeScope.EnqueueParent(scope);
        }

        [UnityEngine.Scripting.Preserve]
        public object Create(Type type)
        {
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                var parameterInstances = new object[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    try
                    {
                        parameterInstances[i] = _resolver.Resolve(parameter.ParameterType);
                    } catch (Exception)
                    {
                        // ignored
                    }
                }
                return constructor.Invoke(parameterInstances);
            }

            return null;
        }

        [UnityEngine.Scripting.Preserve]
        public T Create<T>() where T : class
        {
            return (T)Create(typeof(T));
        }

        [UnityEngine.Scripting.Preserve]
        public T Resolve<T>()
        {
            return _resolver.Resolve<T>();
        }

        [UnityEngine.Scripting.Preserve]
        public object Resolve(Type type)
        {
            return _resolver.Resolve(type);
        }

        [UnityEngine.Scripting.Preserve]
        public void Inject<T>(T instance)
        {
            _resolver.Inject(instance);
        }

        [UnityEngine.Scripting.Preserve]
        public void InjectGameObject(GameObject gameObject)
        {
            _resolver.InjectGameObject(gameObject);
        }

        [UnityEngine.Scripting.Preserve]
        public T Instantiate<T>(T prefab) where T : Component
        {
            return _resolver.Instantiate(prefab);
        }

        [UnityEngine.Scripting.Preserve]
        public T Instantiate<T>(T prefab, Transform parent) where T : Component
        {
            return _resolver.Instantiate(prefab, parent);
        }

        [UnityEngine.Scripting.Preserve]
        public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            return _resolver.Instantiate(prefab, position, rotation);
        }

        [UnityEngine.Scripting.Preserve]
        public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component
        {
            return _resolver.Instantiate(prefab, position, rotation, parent);
        }
    }
}
