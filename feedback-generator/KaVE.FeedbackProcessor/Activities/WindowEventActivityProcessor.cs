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
 *    - Sebastian Proksch
 */

using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.FeedbackProcessor.Activities.Model;

namespace KaVE.FeedbackProcessor.Activities
{
    internal class WindowEventActivityProcessor : BaseActivityProcessor
    {
        public WindowEventActivityProcessor()
        {
            RegisterFor<WindowEvent>(ProcessWindowEvent);
        }

        private void ProcessWindowEvent(WindowEvent @event)
        {
            if (IsOpen(@event) || IsMove(@event) || IsClose(@event))
            {
                InsertActivity(@event, Activity.LocalConfiguration);
            }
            else if (IsActivate(@event))
            {
                // TODO add real handling here
                DropCurrentEvent();
            }
            DropCurrentEvent();
        }

        private bool IsActivate(WindowEvent @event)
        {
            return @event.Action == WindowEvent.WindowAction.Activate;
        }

        private bool IsOpen(WindowEvent @event)
        {
            return @event.Action == WindowEvent.WindowAction.Create;
        }

        private bool IsMove(WindowEvent @event)
        {
            return @event.Action == WindowEvent.WindowAction.Move;
        }

        private bool IsClose(WindowEvent @event)
        {
            return @event.Action == WindowEvent.WindowAction.Close;
        }
    }
}