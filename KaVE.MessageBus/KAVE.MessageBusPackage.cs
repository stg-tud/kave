using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using KAVE.KAVE_MessageBus.Json;
using KAVE.KAVE_MessageBus.MessageBus;
using KaVE.Model.Events;
using KaVE.Utils.Assertion;
using Microsoft.VisualStudio.Shell;

#if !DEBUG
using System.IO.Compression;
#endif

namespace KAVE.KAVE_MessageBus
{
    [PackageRegistration(UseManagedResourcesOnly = true),
     InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400),
     Guid(GuidList.GuidKaveMessageBusPkg), ProvideService(typeof (SMessageBus))]
    // ReSharper disable once InconsistentNaming
    public sealed class KAVE_MessageBusPackage : Package
    {
        private const string LogFileExtension = ".log";
        private const string ProjectName = "KAVE";
        private static readonly string EventLogScopeName = typeof (KAVE_MessageBusPackage).Assembly.GetName().Name;

        private SMessageBus _messageChannel;
        private IDEEvent _lastEvent;

        public KAVE_MessageBusPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", ToString()));
            var serviceContainer = this as IServiceContainer;
            var serviceCreatorCallback = new ServiceCreatorCallback(CreateMessageChannelService);
            serviceContainer.AddService(typeof (SMessageBus), serviceCreatorCallback, true);
        }

        private object CreateMessageChannelService(IServiceContainer container, Type servicetype)
        {
            if (typeof (SMessageBus) == servicetype)
            {
                return new TinyMessengerMessageBus();
            }
            return null;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _messageChannel = GetService(typeof (SMessageBus)) as SMessageBus;
            Asserts.NotNull(_messageChannel, "message bus unavailable");
            _messageChannel.Subscribe<IDEEvent>(LogIDEEvent);
        }

        private void LogIDEEvent(IDEEvent ce)
        {
            lock (_messageChannel)
            {
                var logPath = GetSessionEventLogFilePath(ce);
                EnsureLogDirectoryExists(logPath);
                Debug.WriteLine("Logging IDE Events to: '" + logPath + "'");
                using (var logWriter = NewLogWriter(logPath))
                {
                    logWriter.Write(ce);
                }
            }
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

        private static void EnsureLogDirectoryExists(string logPath)
        {
            var logDir = Path.GetDirectoryName(logPath);
            Asserts.NotNull(logDir, "could not determine log directly from path '{0}'", logPath);
            Directory.CreateDirectory(logDir);
        }

        private static JsonLogWriter NewLogWriter(string logFilePath)
        {
            Stream logStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write);
            try
            {
#if !DEBUG
                logStream = new GZipStream(logStream, CompressionMode.Compress);
#endif
                return new JsonLogWriter(logStream);
            }
            catch (Exception)
            {
                logStream.Close();
                throw;
            }
        }
    }
}