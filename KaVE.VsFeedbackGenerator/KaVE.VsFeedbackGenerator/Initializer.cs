#if !DEBUG
using System.IO.Compression;
#endif
using System;
using System.Diagnostics;
using System.IO;
using JetBrains.Application;
using KaVE.Model.Events;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.VsFeedbackGenerator
{
    [ShellComponent]
    class Initializer
    {
        private readonly IMessageBus _messageChannel;
        private readonly JsonLogFileManager _logFileManager;

        public Initializer(IMessageBus messageBus, JsonLogFileManager logFileManager)
        {
            _messageChannel = messageBus;
            _logFileManager = logFileManager;
            _messageChannel.Subscribe<IDEEvent>(LogIDEEvent);
        }

        private void LogIDEEvent(IDEEvent ce)
        {
            lock (_messageChannel)
            {
                var logFileName = _logFileManager.GetLogFileName(ce.IDESessionUUID);
                Debug.WriteLine("Logging IDE Events to: '{0}'", (object) logFileName);
                using (var logWriter = _logFileManager.NewLogWriter(logFileName))
                {
                    logWriter.Write(ce);
                }
            }
        }
    }
}

