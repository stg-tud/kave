using System;
using System.ComponentModel.Composition;
using System.IO;
using CodeCompletion.Model;
using CompletionEventSerializer;

namespace CompletionEventBus
{
    public class EventBusInitializer
    {
        private const string LogFileExtension = ".log";
        private const string ProjectName = "KAVE";
        private static readonly string FeedbackGeneratorScopeName = typeof (EventBusInitializer).Assembly.GetName().Name;
        
        // TODO ensure this is getting called at "the beginning"...
        [ImportingConstructor]
        public EventBusInitializer(IMessageChannel messageChannel, ISerializer serializer)
        {
            messageChannel.Subscribe<IDEEvent>(
                ce =>
                {
                    lock (messageChannel)
                    {
                        var logPath = GetSessionEventLogFilePath(ce);
                        using (var logStream = new FileStream(logPath, FileMode.Append, FileAccess.Write))
                        {
#if DEBUG
                            new EventLogWriter(logStream).Append(ce);
#else
                            new CompressedEventLogWriter(logStream).Append(ce);
#endif
                        }
                    }
                });
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
                return Path.Combine(appDataPath, ProjectName, FeedbackGeneratorScopeName);
            }
        }
    }
}