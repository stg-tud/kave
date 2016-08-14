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
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.FeedbackProcessor.Model;
using KaVE.JetBrains.Annotations;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class ParallelIDEInstancesStatisticCalculator : BaseEventProcessor
    {
        public readonly IList<string> Statistic = new List<string>();

        public ParallelIDEInstancesStatisticCalculator()
        {
            RegisterFor<IDEStateEvent>(ProcessIDEStateEvent);
            RegisterFor<WindowEvent>(ProcessWindowEvent);
        }

        [StringFormatMethod("message")]
        private void Log(string message, params object[] args)
        {
            Statistic.Add(string.Format(message, args));
        }

        public override void OnStreamStarts(Developer developer)
        {
            Log("Starting stream of developer {0}", developer.Id);
        }

        private void ProcessIDEStateEvent(IDEStateEvent @event)
        {
            switch (@event.IDELifecyclePhase)
            {
                case IDELifecyclePhase.Startup:
                    Log("  START at {0} - {1}", @event.GetTriggeredAt(), @event.IDESessionUUID);
                    break;
                case IDELifecyclePhase.Shutdown:
                    Log("  STOP  at {0} - {1}", @event.GetTriggeredAt(), @event.IDESessionUUID);
                    break;
            }
        }

        private void ProcessWindowEvent(WindowEvent @event)
        {
            if ("vsWindowTypeMainWindow".Equals(@event.Window.Type))
            {
                switch (@event.Action)
                {
                    case WindowAction.Activate:
                        Log(
                            "  ACT   at {0} - {1} - {2}",
                            @event.GetTriggeredAt(),
                            @event.IDESessionUUID,
                            @event.Window.Caption);
                        break;
                    case WindowAction.Deactivate:
                        Log(
                            "  DEACT at {0} - {1} - {2}",
                            @event.GetTriggeredAt(),
                            @event.IDESessionUUID,
                            @event.Window.Caption);
                        break;
                }
            }
        }
    }
}