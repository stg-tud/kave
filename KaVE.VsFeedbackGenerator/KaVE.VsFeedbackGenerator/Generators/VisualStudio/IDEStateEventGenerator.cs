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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System.Reflection;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.DataFlow;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Names;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class IDEStateEventGenerator : EventGeneratorBase
    {
        private readonly EventLogger _logger;

        public IDEStateEventGenerator(IIDESession session,
            IMessageBus messageBus,
            Lifetime lifetime,
            IDateUtils dateUtils,
            EventLogger logger)
            : base(session, messageBus, dateUtils)
        {
            _logger = logger;
            FireIDEStateEvent(IDEStateEvent.LifecyclePhase.Startup);
            lifetime.AddAction(FireShutdownEvent);
        }

        private void FireShutdownEvent()
        {
            // Sven: I found no way to ensure that the message bus is still
            // running and the logger attachted to it, when we reach this point.
            var ideStateEvent = CreateIDEStateEvent(IDEStateEvent.LifecyclePhase.Shutdown);
            var process = typeof (EventLogger).GetMethod(
                EventLogger.ProcessMethodName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            process.Invoke(_logger, new object[] {ideStateEvent});
        }

        private void FireIDEStateEvent(IDEStateEvent.LifecyclePhase phase)
        {
            FireNow(CreateIDEStateEvent(phase));
        }

        private IDEStateEvent CreateIDEStateEvent(IDEStateEvent.LifecyclePhase phase)
        {
            var ideStateEvent = Create<IDEStateEvent>();
            ideStateEvent.IDELifecyclePhase = phase;
            ideStateEvent.OpenWindows = DTE.Windows.GetNames();
            ideStateEvent.OpenDocuments = DTE.Documents.GetNames();
            return ideStateEvent;
        }
    }
}