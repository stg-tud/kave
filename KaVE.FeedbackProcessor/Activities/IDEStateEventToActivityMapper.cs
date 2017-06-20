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

using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.FeedbackProcessor.Activities.Model;

namespace KaVE.FeedbackProcessor.Activities
{
    internal class IDEStateEventToActivityMapper : BaseToActivityMapper
    {
        public IDEStateEventToActivityMapper()
        {
            RegisterFor<IDEStateEvent>(ProcessIDEStateEvent);
        }

        private void ProcessIDEStateEvent(IDEStateEvent @event)
        {
            if (IsIntermediateRuntimeEvent(@event))
            {
                DropCurrentEvent();
            }
            else
            {
                var activity = IsStartup(@event) ? Activity.EnterIDE : Activity.LeaveIDE;
                InsertActivity(@event, activity);
            }
        }

        private static bool IsIntermediateRuntimeEvent(IDEStateEvent @event)
        {
            return @event.IDELifecyclePhase == IDELifecyclePhase.Runtime;
        }

        private static bool IsStartup(IDEStateEvent @event)
        {
            return @event.IDELifecyclePhase == IDELifecyclePhase.Startup;
        }
    }
}