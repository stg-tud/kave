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
using System.Diagnostics;
using EnvDTE;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Json;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.FeedbackGenerator.Utils.Names;
using KaVE.VS.FeedbackGenerator.VsIntegration;

namespace KaVE.VS.FeedbackGenerator.Generators
{
    public abstract class EventGeneratorBase
    {
        private readonly IRSEnv _env;
        private readonly IMessageBus _messageBus;
        private readonly IDateUtils _dateUtils;

        protected EventGeneratorBase([NotNull] IRSEnv env,
            [NotNull] IMessageBus messageBus,
            [NotNull] IDateUtils dateUtils)
        {
            _env = env;
            _messageBus = messageBus;
            _dateUtils = dateUtils;
        }

        [NotNull]
        protected DTE DTE
        {
            get { return _env.IDESession.DTE; }
        }

        protected TIDEEvent Create<TIDEEvent>() where TIDEEvent : IDEEvent, new()
        {
            return new TIDEEvent
            {
                KaVEVersion = _env.KaVEVersion,
                IDESessionUUID = _env.IDESession.UUID,
                ActiveWindow = DTEActiveWindow.GetName(),
                ActiveDocument = DTEActiveDocument.GetName(),
                TriggeredBy = CurrentTrigger,
                TriggeredAt = _dateUtils.Now
            };
        }

        private static IDEEvent.Trigger CurrentTrigger
        {
            get
            {
                // we cannot detect mouse click as a trigger, since
                // mouse-up is what actually triggers the action
                return IDEEvent.Trigger.Unknown;
            }
        }

        private Window DTEActiveWindow
        {
            get
            {
                try
                {
                    return DTE.ActiveWindow;
                }
                catch (NullReferenceException)
                {
                    // accessing the active window throws an NullReferenceException
                    // if no windows have been opened yet
                    return null;
                }
            }
        }

        private Document DTEActiveDocument
        {
            get
            {
                try
                {
                    return DTE.ActiveDocument;
                }
                catch (ArgumentException)
                {
                    // accessing active document throws an ArgumentException, for
                    // example, when the ActiveWindow is the properties page of a project
                    return null;
                }
            }
        }

        /// <summary>
        ///     Sets <see cref="IDEEvent.TerminatedAt" /> to the current time and delegates to <see cref="Fire{TEvent}" />.
        /// </summary>
        protected void FireNow<TEvent>([NotNull] TEvent @event) where TEvent : IDEEvent
        {
            @event.TerminatedAt = _dateUtils.Now;
            Fire(@event);
        }

        /// <summary>
        ///     Sets <see cref="IDEEvent.IDESessionUUID" /> to <see cref="IDESession.UUID" /> and publishes the event to
        ///     the underlying message channel.
        /// </summary>
        protected void Fire<TEvent>([NotNull] TEvent @event) where TEvent : IDEEvent
        {
            // TODO @Sven: why is it set here and not with the other information "on create"?
            // TODO @Seb: good question... if we always use Create it shouldn't make a difference...
            @event.IDESessionUUID = _env.IDESession.UUID;
            _messageBus.Publish<IDEEvent>(@event);
            WriteToDebugConsole(@event);
        }

        [Conditional("DEBUG")]
        private static void WriteToDebugConsole(IDEEvent @event)
        {
            Debug.WriteLine(@event.ToFormattedJson());
        }
    }
}