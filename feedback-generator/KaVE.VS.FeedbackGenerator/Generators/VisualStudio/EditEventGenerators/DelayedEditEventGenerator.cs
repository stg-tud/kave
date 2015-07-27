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

using System;
using EnvDTE;
using JetBrains.Application;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Concurrency;
using KaVE.VS.FeedbackGenerator.Generators.VisualStudio.EditEventGenerators.EventContext;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.FeedbackGenerator.Utils.Names;

namespace KaVE.VS.FeedbackGenerator.Generators.VisualStudio.EditEventGenerators
{
    [ShellComponent]
    public class DelayedEditEventGenerator : EventGeneratorBase
    {
        private readonly IContextProvider _contextProvider;
        private readonly IRetryRunner _retryRunner;

        private readonly BlockingCollection<EditEvent> _delayedEditEvents = new BlockingCollection<EditEvent>();

        public DelayedEditEventGenerator(IRSEnv env,
            IMessageBus messageBus,
            IDateUtils dateUtils,
            IRetryRunner retryRunner,
            IContextProvider contextProvider)
            : base(env, messageBus, dateUtils)
        {
            _retryRunner = retryRunner;
            _contextProvider = contextProvider;

          //  Task.StartNewLongRunning(FireDelayedEvents);
        }

        public void TryFireWithContext(Document document)
        {
            // ReSharper disable once ObjectCreationAsStatement
         /*   new DelayedEditEventHandler(
                _retryRunner,
                _contextProvider,
                Create<EditEvent>(),
                document,
                _delayedEditEvents);*/
        }

        private void FireDelayedEvents()
        {
            foreach (var delayedEditEvent in _delayedEditEvents.GetConsumingEnumerable())
            {
                Fire(delayedEditEvent);
            }
        }

        private class DelayedEditEventHandler
        {
            private static readonly TimeSpan RetryInterval = TimeSpan.FromMilliseconds(500);
            private const int NumberOfTries = 10;

            private readonly IContextProvider _contextProvider;
            private readonly EditEvent _editEvent;
            private readonly Document _document;
            private readonly BlockingCollection<EditEvent> _delayedEventQueue;

            internal DelayedEditEventHandler(IRetryRunner retryRunner,
                IContextProvider contextProvider,
                EditEvent editEvent,
                Document document,
                BlockingCollection<EditEvent> delayedEventQueue)
            {
                _contextProvider = contextProvider;
                _editEvent = editEvent;
                _document = document;
                _delayedEventQueue = delayedEventQueue;

                retryRunner.Try(
                    TryGetContext,
                    RetryInterval,
                    NumberOfTries,
                    AddDelayedEventToQueue);
            }

            private Context TryGetContext()
            {
                var currentContext = _contextProvider.GetCurrentContext(_document);
                Asserts.Not(currentContext.Equals(new Context()), "context is not yet valid");
                return currentContext;
            }

            private void AddDelayedEventToQueue(Context context)
            {
                _editEvent.Context2 = context;
                _editEvent.ActiveDocument = _document.GetName();
                _delayedEventQueue.Add(_editEvent);
            }
        }
    }
}