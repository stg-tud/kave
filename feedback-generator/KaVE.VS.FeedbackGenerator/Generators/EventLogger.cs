/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Linq;
using JetBrains.Application;
using JetBrains.DataFlow;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Concurrent;
using KaVE.VS.FeedbackGenerator.Generators.Merging;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.FeedbackGenerator.Utils.Logging;

namespace KaVE.VS.FeedbackGenerator.Generators
{
    public interface IEventLogger
    {
        void Log(IDEEvent @event);
        void Shutdown(IDEStateEvent shutdownEvent);
    }

    [ShellComponent]
    public class EventLogger : IEventLogger
    {
        private static readonly IList<IEventMergeStrategy> MergeStrategies = new List<IEventMergeStrategy>
        {
            new CompletionEventMergingStrategy()
        };

        private readonly BlockingCollection<IDEEvent> _eventQueue;
        private IDEEvent _lastEvent;
        private readonly ILogManager _logManager;

        public EventLogger(IMessageBus messageBus, ILogManager logManager, Lifetime lifetime)
        {
            _logManager = logManager;
            _eventQueue = new BlockingCollection<IDEEvent>();
            messageBus.Subscribe<IDEEvent>(Log);
            Task.StartNewLongRunning(ProcessEvents);
        }

        public void Log(IDEEvent @event)
        {
            if (@event == null)
            {
                return;
            }

            _eventQueue.Add(@event);
        }

        private void ProcessEvents()
        {
            foreach (var @event in _eventQueue.GetConsumingEnumerable())
            {
                ProcessEvent(@event);
            }
        }

        public void Shutdown(IDEStateEvent shutdownEvent)
        {
            LogEvent(_lastEvent);
            LogEvent(shutdownEvent);
        }

        private void ProcessEvent(IDEEvent @event)
        {
            if (_lastEvent == null)
            {
                _lastEvent = @event;
            }
            else
            {
                var merger = MergeStrategies.FirstOrDefault(strategy => strategy.AreMergable(_lastEvent, @event));
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
            try
            {
                _logManager.CurrentLog.Append(@event);
            }
                // ReSharper disable once EmptyGeneralCatchClause
                // if logging fails, there's nothing we can do.
            catch {}
        }
    }
}