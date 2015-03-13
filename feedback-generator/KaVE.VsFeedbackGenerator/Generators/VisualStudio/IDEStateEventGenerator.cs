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

using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.DataFlow;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class IDEStateEventGenerator : EventGeneratorBase
    {
        private readonly EventLogger _logger;

        public IDEStateEventGenerator(IRSEnv env,
            IMessageBus messageBus,
            Lifetime lifetime,
            IDateUtils dateUtils,
            EventLogger logger)
            : base(env, messageBus, dateUtils)
        {
            _logger = logger;
            FireIDEStateEvent(IDEStateEvent.LifecyclePhase.Startup);
            lifetime.AddAction(FireShutdownEvent);
        }

        private void FireShutdownEvent()
        {
            _logger.Log(CreateIDEStateEvent(IDEStateEvent.LifecyclePhase.Shutdown));
        }

        private void FireIDEStateEvent(IDEStateEvent.LifecyclePhase phase)
        {
            FireNow(CreateIDEStateEvent(phase));
        }

        private IDEStateEvent CreateIDEStateEvent(IDEStateEvent.LifecyclePhase phase)
        {
            var ideStateEvent = Create<IDEStateEvent>();
            ideStateEvent.IDELifecyclePhase = phase;

            ideStateEvent.OpenWindows = GetVisibleWindows().GetNames();
            ideStateEvent.OpenDocuments = DTE.Documents.GetNames();
            return ideStateEvent;
        }

        private IEnumerable<Window> GetVisibleWindows()
        {
            return from Window window in DTE.Windows where window.Visible select window;
        }
    }
}