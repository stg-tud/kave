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

using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class ParallelIDEInstancesStatisticCalculator : BaseEventProcessor
    {
        private readonly ILogger _logger;

        public ParallelIDEInstancesStatisticCalculator(ILogger logger)
        {
            _logger = logger;
            RegisterFor<IDEStateEvent>(ProcessIDEStateEvent);
            RegisterFor<WindowEvent>(ProcessWindowEvent);
        }

        public override void OnStreamStarts(Developer developer)
        {
            _logger.Info("Starting stream of developer {0}", developer.Id);
        }

        private void ProcessIDEStateEvent(IDEStateEvent @event)
        {
            switch (@event.IDELifecyclePhase)
            {
                case IDEStateEvent.LifecyclePhase.Startup:
                    _logger.Info("  START at {0}", @event.GetTriggeredAt());
                    break;
                case IDEStateEvent.LifecyclePhase.Shutdown:
                    _logger.Info("  STOP  at {0}", @event.GetTriggeredAt());
                    break;
            }
        }

        private void ProcessWindowEvent(WindowEvent @event)
        {
            if ("vsWindowTypeMainWindow".Equals(@event.Window.Type))
            {
                switch (@event.Action)
                {
                    case WindowEvent.WindowAction.Activate:
                        _logger.Info("  ACT   at {0} - {1}", @event.GetTriggeredAt(), @event.Window.Caption);
                        break;
                    case WindowEvent.WindowAction.Deactivate:
                        _logger.Info("  DEACT at {0} - {1}", @event.GetTriggeredAt(), @event.Window.Caption);
                        break;
                }
            }
        }
    }
}