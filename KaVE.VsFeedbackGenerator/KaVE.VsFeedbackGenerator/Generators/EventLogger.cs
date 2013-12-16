#if !DEBUG
using System.IO.Compression;
#endif
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Application;
using KaVE.Model.Events;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.VsFeedbackGenerator.Generators
{
    [ShellComponent]
    internal class EventLogger
    {
        private static readonly IList<IEventMergeStrategy> MergeStrategies = new List<IEventMergeStrategy> {};

        private readonly IMessageBus _messageChannel;
        private readonly JsonLogFileManager _logFileManager;

        private IDEEvent _lastEvent;

        public EventLogger(IMessageBus messageBus, JsonLogFileManager logFileManager)
        {
            _messageChannel = messageBus;
            _logFileManager = logFileManager;
            _messageChannel.Subscribe<IDEEvent>(ProcessEvent);
        }

        private void ProcessEvent(IDEEvent @event)
        {
            lock (_messageChannel)
            {
                if (IsIDEShutdownEvent(@event))
                {
                    LogEvent(_lastEvent);
                    LogEvent(@event);
                    return;
                }

                ProcessNormalEvent(@event);
            }
        }

        private static bool IsIDEShutdownEvent(IDEEvent @event)
        {
            var ideStateEvent = @event as IDEStateEvent;
            return ideStateEvent != null && ideStateEvent.IDELifecyclePhase == IDEStateEvent.LifecyclePhase.Shutdown;
        }

        private void ProcessNormalEvent(IDEEvent @event)
        {
            if (_lastEvent == null)
            {
                _lastEvent = @event;
            }
            else
            {
                var merger = MergeStrategies.First(strategy => strategy.Mergable(_lastEvent, @event));
                if (merger != null)
                {
                    _lastEvent = merger.Merge(_lastEvent, @event);
                    return;
                }
                LogEvent(_lastEvent);
                _lastEvent = @event;
            }
        }

        private void LogEvent(IDEEvent @event)
        {
            if (@event == null)
            {
                return;
            }
            var logFileName = _logFileManager.GetLogFileName(@event.IDESessionUUID);
            using (var logWriter = _logFileManager.NewLogWriter(logFileName))
            {
                logWriter.Write(@event);
            }
        }
    }
}