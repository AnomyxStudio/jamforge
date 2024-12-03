using JamForge.Resolver;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace JamForge
{
    public partial class Jam
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Reset()
        {
            if (_facadeScope)
            {
                Object.Destroy(_facadeScope.gameObject);
            }

            _instance = null;
            _facadeScope = null;
        }

        private static LifetimeScope _facadeScope;

        private static Jam _instance;

        private static Jam Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                if (!_facadeScope)
                {
                    _facadeScope = LifetimeScope.Create(FacadeScopeConstruction);
                    _facadeScope.name = "JamFacadeScope";
                    _facadeScope.gameObject.hideFlags = HideFlags.HideInHierarchy;
                }

                _instance = _facadeScope.Container.Resolve<Jam>();

                return _instance;
            }
        }

        #region Resolver

        private static void FacadeScopeConstruction(IContainerBuilder builder)
        {
            builder.Register<Jam>(Lifetime.Singleton);
        }

        [Inject]
        private IJamResolver _resolver;

        public static IJamResolver Resolver => Instance._resolver;

        internal static void OverrideResolver(IJamResolver resolver)
        {
            Instance._resolver = resolver;
        }

        #endregion
    }
}
