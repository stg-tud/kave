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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Intervals.Model;

namespace KaVE.FeedbackProcessor.Intervals.Transformers
{
    public class VisualStudioActiveTransformer : IEventToIntervalTransformer<VisualStudioActiveInterval>
    {
        private readonly TransformerContext _context;
        private readonly IList<VisualStudioActiveInterval> _intervals = Lists.NewList<VisualStudioActiveInterval>();
        private VisualStudioActiveInterval _cur;

        // let's assume the ide has focus by default
        private bool _hasFocus = true;

        public VisualStudioActiveTransformer(TransformerContext context)
        {
            _context = context;
        }

        public void ProcessEvent(IDEEvent e)
        {
            if (IsFocusGain(e))
            {
                _hasFocus = true;
            }

            if (!(e.TriggeredAt.HasValue && _hasFocus))
            {
                return;
            }

            if (_cur == null)
            {
                _intervals.Add(_cur = _context.CreateIntervalFromEvent<VisualStudioActiveInterval>(e));
            }

            _cur.Duration = e.TriggeredAt.Value - _cur.StartTime;

            if (IsFocusLost(e))
            {
                EndCurrentInterval();
            }
        }

        private void EndCurrentInterval()
        {
            _hasFocus = false;
            _cur = null;
        }

        private static bool IsFocusGain(IDEEvent e)
        {
            return IsFocusChange(e, WindowAction.Activate);
        }

        private static bool IsFocusLost(IDEEvent e)
        {
            return IsFocusChange(e, WindowAction.Deactivate);
        }

        private static bool IsFocusChange(IDEEvent e, WindowAction windowAction)
        {
            var we = e as WindowEvent;
            if (we == null)
            {
                return false;
            }

            return we.Window.Equals(Names.Window("main Microsoft Visual Studio")) &&
                   we.Action == windowAction;
        }

        public IEnumerable<VisualStudioActiveInterval> SignalEndOfEventStream()
        {
            EndCurrentInterval();
            return _intervals;
        }
    }
}