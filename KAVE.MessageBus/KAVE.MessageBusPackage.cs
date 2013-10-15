using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using CodeCompletion.Utils.Assertion;
using KAVE.KAVE_MessageBus.Json;
using KAVE.KAVE_MessageBus.MessageBus;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using CodeCompletion.Model;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

#if !DEBUG
using System.IO.Compression;
#endif

namespace KAVE.KAVE_MessageBus
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.guidKAVE_MessageBusPkgString)]
    [ProvideService(typeof(SMessageBus))]
    // ReSharper disable once InconsistentNaming
    public sealed class KAVE_MessageBusPackage : Package
    {
        private const string LogFileExtension = ".log";
        private const string ProjectName = "KAVE";
        private static readonly string EventLogScopeName = typeof (KAVE_MessageBusPackage).Assembly.GetName().Name;

        public KAVE_MessageBusPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", ToString()));
            var serviceContainer = this as IServiceContainer;
            var serviceCreatorCallback = new ServiceCreatorCallback(CreateMessageChannelService);
            serviceContainer.AddService(typeof(SMessageBus), serviceCreatorCallback, true);
        }

        private object CreateMessageChannelService(IServiceContainer container, Type servicetype)
        {
            if (typeof(SMessageBus) == servicetype)
            {
                return new TinyMessengerMessageBus();
            }
            return null;
        }

        protected override void Initialize()
        {
            base.Initialize();
            var messageChannel = GetService(typeof(SMessageBus)) as SMessageBus;
            Asserts.NotNull(messageChannel, "message bus unavailable");

            messageChannel.Subscribe<IDEEvent>(
                ce =>
                {
                    lock (messageChannel)
                    {
                        var logPath = GetSessionEventLogFilePath(ce);
                        EnsureLogDirectoryExists(logPath);
                        Debug.WriteLine("Logging IDE Events to: '" + logPath + "'");
                        using (var logStream = new FileStream(logPath, FileMode.Append, FileAccess.Write))
                        {
#if DEBUG
                            new JsonLogWriter(logStream).Write(ce);
#else
                            using (var compressedLogStream = new GZipStream(logStream, CompressionMode.Compress))
                            {
                                new JsonLogWriter(compressedLogStream).Write(ce);
                            }
#endif
                        }
                    }
                });
        }

        private static void EnsureLogDirectoryExists(string logPath)
        {
            var logDir = Path.GetDirectoryName(logPath);
            Asserts.NotNull(logDir, "could not determine log directly from path '{0}'", logPath);
            Directory.CreateDirectory(logDir);
        }

        private static string GetSessionEventLogFilePath(IDEEvent evt)
        {
            return Path.Combine(EventLogsPath, evt.IDESessionUUID + LogFileExtension);
        }

        private static string EventLogsPath
        {
            get
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(appDataPath, ProjectName, EventLogScopeName);
            }
        }
    }
}
