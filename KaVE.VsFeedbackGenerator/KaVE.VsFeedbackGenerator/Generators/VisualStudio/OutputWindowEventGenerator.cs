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
using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class OutputWindowEventGenerator : EventGeneratorBase
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly OutputWindowEvents _outputWindowEvents;

        public OutputWindowEventGenerator(IIDESession session, IMessageBus messageBus)
            : base(session, messageBus)
        {
            _outputWindowEvents = DTE.Events.OutputWindowEvents;
            _outputWindowEvents.PaneAdded += OutputWindowEvents_PaneAdded;
            _outputWindowEvents.PaneUpdated += _outputWindowEvents_PaneUpdated;
            _outputWindowEvents.PaneClearing += _outputWindowEvents_PaneClearing;
        }

        void OutputWindowEvents_PaneAdded(OutputWindowPane pPane)
        {
            Fire(OutputWindowEvent.OutputWindowAction.AddPane, pPane);
        }

        void _outputWindowEvents_PaneUpdated(OutputWindowPane pPane)
        {
            Fire(OutputWindowEvent.OutputWindowAction.UpdatePane, pPane);
        }

        void _outputWindowEvents_PaneClearing(OutputWindowPane pPane)
        {
            Fire(OutputWindowEvent.OutputWindowAction.ClearPane, pPane);
        }

        private void Fire(OutputWindowEvent.OutputWindowAction action, OutputWindowPane pPane)
        {
            var outputWindowEvent = Create<OutputWindowEvent>();
            outputWindowEvent.Action = action;
            outputWindowEvent.PaneName = pPane.Name;
            FireNow(outputWindowEvent);
        }
    }
}