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
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils.Names;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class IDEStateEventGenerator : EventGeneratorBase, IDisposable
    {
        public IDEStateEventGenerator(IIDESession session, IMessageBus messageBus)
            : base(session, messageBus)
        {
            FireIDEStateEvent(IDEStateEvent.LifecyclePhase.Startup);
        }

        public void Dispose()
        {
            FireIDEStateEvent(IDEStateEvent.LifecyclePhase.Shutdown);
        }

        private void FireIDEStateEvent(IDEStateEvent.LifecyclePhase phase)
        {
            var ideStateEvent = Create<IDEStateEvent>();
            ideStateEvent.IDELifecyclePhase = phase;
            ideStateEvent.OpenWindows = DTE.Windows.GetNames();
            ideStateEvent.OpenDocuments = DTE.Documents.GetNames();
            FireNow(ideStateEvent);
        }
    }
}
