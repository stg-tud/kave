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
using System.Windows.Forms;
using JetBrains.Application;
using JetBrains.Threading;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.Generators.VisualStudio;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.Activity
{
    [ShellComponent]
    public class MouseActivityEventGenerator : EventGeneratorBase
    {
        // TODO evaluate good threshold value
        public static readonly TimeSpan InactivitySpanToBreakActivityPeriod = TimeSpan.FromSeconds(3);

        private readonly IDateUtils _dateUtils;
        private readonly IFocusHelper _focusHelper;

        private ActivityEvent _currentEvent;
        private DateTime _lastActivity;

        public MouseActivityEventGenerator([NotNull] IRSEnv env,
            [NotNull] IMessageBus messageBus,
            [NotNull] IDateUtils dateUtils,
            [NotNull] IKaVEMouseEvents mouseEvents,
            IFocusHelper focusHelper,
            IThreading threading) : base(env, messageBus, dateUtils, threading)
        {
            _dateUtils = dateUtils;
            _focusHelper = focusHelper;
            mouseEvents.MouseMove += FireMouseActivity;
            mouseEvents.MouseClick += FireMouseActivity;
            mouseEvents.MouseWheel += FireMouseActivity;

            _lastActivity = dateUtils.Now;
        }

        private void FireMouseActivity(object sender, MouseEventArgs e)
        {
            var now = _dateUtils.Now;

            if (HasOpenPeriod() && IsInactive(now))
            {
                _currentEvent.TerminatedAt = _lastActivity;
                EndPeriod();
            }

            if (_focusHelper.IsCurrentApplicationActive())
            {
                if (!HasOpenPeriod())
                {
                    _currentEvent = Create<ActivityEvent>();
                    _currentEvent.TriggeredBy = EventTrigger.Click;
                }

                _lastActivity = now;
            }
        }

        private void EndPeriod()
        {
            Fire(_currentEvent);
            _currentEvent = null;
        }

        private bool HasOpenPeriod()
        {
            return _currentEvent != null;
        }

        private bool IsInactive(DateTime now)
        {
            return (now - _lastActivity) > InactivitySpanToBreakActivityPeriod;
        }
    }
}