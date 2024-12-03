using System;
using System.Collections.Generic;
using JamForge.Audio;
using JamForge.Events;
using JamForge.Logging;
using JamForge.Resolver;
using JamForge.Serialization;
using JamForge.Store;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace JamForge
{
    [DefaultExecutionOrder(-10000)]
    public class JamForgeCoreScope : LifetimeScope
    {
        [SerializeField] private TextAsset packageJson;

        private static JamForgeConfig GetConfig() => JamForgeConfig.Load();

        protected override void Configure(IContainerBuilder builder)
        {
            // Event broker
            builder.RegisterComponentOnNewGameObject<MainThreadDispatcher>(Lifetime.Singleton).AsImplementedInterfaces();
            // Includes IEventBrokerFacade, IAsyncEventBroker, IStickyEventBroker, IEventBroker
            builder.Register<EventBroker>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            // MessagePipe
            builder.RegisterMessagePipe(cfg =>
            {
                cfg.RequestHandlerLifetime = InstanceLifetime.Singleton;
                cfg.DefaultAsyncPublishStrategy = AsyncPublishStrategy.Parallel;
            });
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));

            // Serialization
            builder.Register<IJsonSerializer, CatJsonSerializer>(Lifetime.Singleton);
            builder.Register<IBinarySerializer, NaniBinarySerializer>(Lifetime.Singleton);

            // Store
            builder.Register<IPersistStoreVendor, PlayerPrefsStoreVendor>(Lifetime.Singleton);
            builder.Register<IMemoryStoreVendor, InMemoryDictionaryStore>(Lifetime.Singleton);
            builder.Register<PlayerPrefsStoreVendor.PlayerPrefsPersistStore>(Lifetime.Transient);
            builder.Register<InMemoryDictionaryStore.DictionaryMemoryStore>(Lifetime.Transient);
            builder.Register<IJamStores, JamStores>(Lifetime.Singleton);

            // Config
            var config = Resources.Load<JamForgeConfig>(nameof(JamForgeConfig));
            if (config) { builder.RegisterInstance(config); }

            // Package info
            var packageInfo = JsonUtility.FromJson<PackageInfo>(packageJson.text);
            if (packageInfo != null) { builder.RegisterInstance(packageInfo); }

            // Logging
            AssembleLogging(builder);

            // Resolver
            builder.Register<IJamResolver, JamResolver>(Lifetime.Scoped);

            // Audios
            builder.RegisterEntryPoint<AudioController>();

            InternalLog.Info($"JamForge core services registered!");

            builder.RegisterBuildCallback(OnCoreServicesRegistered);
        }

        private static void OnCoreServicesRegistered(IObjectResolver resolver)
        {
            var packageInfo = resolver.Resolve<PackageInfo>();
            var version = packageInfo.Version;
            InternalLog.Debug($"JamForge initialized! Version: {version}".DyeCyan());
        }

        private static void AssembleLogging(IContainerBuilder builder)
        {
            var jamLogManager = new LogManager();
            var formatters = new List<ILogFormatter> { new StandardLogFormatter() };
            var unityLogAppender = new UnityLogAppender(formatters);
            var config = GetConfig();
            // TODO: Read from separated log config later
            if (config)
            {
                if (config.TimestampFormatted)
                {
                    formatters.Add(new TimestampLogFormatter());
                }

                if (config.AdditionalFormatters is { Length: > 0 })
                {
                    var formatterTypes = TypeFinder.GetSubclassesOf(typeof(ILogFormatter));
                    for (var i = 0; i < config.AdditionalFormatters.Length; i++)
                    {
                        var formatterName = config.AdditionalFormatters[i];
                        var formatterType = formatterTypes.Find(t => t.Name == formatterName);
                        var formatterInstance = (ILogFormatter)Activator.CreateInstance(formatterType);
                        if (formatterInstance == null)
                        {
                            throw new Exception($"Could not create instance of formatter type {formatterName}");
                        }
                        formatters.Add(formatterInstance);
                    }
                }

                if (config.PersistLogs)
                {
                    var logFileName = $"Logs/{DateTime.Now:yyyyMMddHHmmss}.log";
                    var fileLogAppender = new UnityFileWriterAppender(logFileName, formatters);
                    jamLogManager.AddAppender(fileLogAppender.WriteLine);
                }
            }

            jamLogManager.AddAppender(unityLogAppender.WriteLine);
            var jamLogger = jamLogManager.GetLogger("JamForge");

            InternalLog.Logger = jamLogger;

            builder.RegisterInstance(jamLogger);
            builder.RegisterInstance(jamLogManager).AsImplementedInterfaces();
        }

#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        [UnityEngine.Scripting.Preserve]
        private static void UnityLogCallback(string condition, string stacktrace, LogType type)
        {
            switch (type)
            {
                case LogType.Exception:
                case LogType.Error:
                    InternalLog.Error(condition);
                    break;
                case LogType.Assert:
                    InternalLog.Fatal(condition);
                    break;
                case LogType.Warning:
                    InternalLog.Warn(condition);
                    break;
                case LogType.Log:
                    InternalLog.Debug(condition);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
