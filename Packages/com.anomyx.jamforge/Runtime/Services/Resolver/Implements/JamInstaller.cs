using VContainer;
using VContainer.Unity;

namespace JamForge.Resolver
{
    public abstract class JamInstaller : LifetimeScope
    {
        public abstract void Install(IContainerBuilder builder);

        protected sealed override void Configure(IContainerBuilder builder)
        {
            Install(builder);
            builder.RegisterBuildCallback(OnInstallerBuilt);
        }

        private static void OnInstallerBuilt(IObjectResolver resolver)
        {
            Jam.OverrideResolver(resolver.Resolve<IJamResolver>());
        }
    }
}
