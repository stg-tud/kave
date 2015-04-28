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

using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class WindowNameCollector : IEventProcessor
    {
        public WindowNameCollector()
        {
            AllWindowNames = new KaVEHashSet<WindowName>();
        }

        public void OnStreamStarts(Developer value) {}

        public void OnEvent(IDEEvent @event)
        {
            AddName(@event.ActiveWindow);

            var windowEvent = @event as WindowEvent;
            if (windowEvent != null)
            {
                AddName(windowEvent.Window);
            }

            var ideStateEvent = @event as IDEStateEvent;
            if (ideStateEvent != null)
            {
                foreach (var name in ideStateEvent.OpenWindows)
                {
                    AddName(name);
                }
            }
        }

        private void AddName(WindowName windowName)
        {
            if (windowName != null)
            {
                AllWindowNames.Add(windowName);
            }
        }

        public void OnStreamEnds() {}

        public IKaVESet<WindowName> AllWindowNames { get; set; }
    }
}