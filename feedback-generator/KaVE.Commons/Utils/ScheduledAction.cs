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
using System.Timers;

namespace KaVE.Commons.Utils
{
    /// <summary>
    /// An action that is executed once, after a defined time.
    /// </summary>
    public class ScheduledAction
    {
        /// <summary>
        /// A scheduled action that does nothing. Can be used as a NullObject.
        /// </summary>
        public static readonly ScheduledAction NoOp = new ScheduledAction();

        private readonly ElapsedEventHandler _scheduledAction;
        private readonly Timer _timer;


        /// <summary>
        /// Creation schedules the given action for execution after the given number of milliseconds.
        /// </summary>
        /// <param name="actionToSchedule">The action to schedule.</param>
        /// <param name="millisToDelay">The number of milliseconds to delay exection. Expected to be greater than 0.</param>
        public ScheduledAction(Action actionToSchedule, long millisToDelay)
            : this(actionToSchedule, millisToDelay, () => { })
        {
        }

        /// <summary>
        /// Creation schedules the given action for execution after the given number of milliseconds.
        /// </summary>
        /// <param name="actionToSchedule">The action to schedule.</param>
        /// <param name="millisToDelay">The number of milliseconds to delay exection. Expected to be greater than 0.</param>
        /// <param name="finishedAction">Action that is called after scheduled action is invoked.</param>
        public ScheduledAction(Action actionToSchedule, long millisToDelay, Action finishedAction)
        {
            _scheduledAction = (s, evtArgs) =>
            {
                lock (_timer)
                {
                    if (_timer.Enabled)
                    {
                        _timer.Stop();
                        actionToSchedule.Invoke();
                        finishedAction.Invoke();
                    }
                }
            };

            _timer = new Timer(millisToDelay);
            _timer.Elapsed += _scheduledAction;
            _timer.Start();
        }

        /// <summary>
        /// Creates a NOP.
        /// </summary>
        private ScheduledAction()
        {
            _scheduledAction = (s, evtArgs) => { };
        }

        /// <summary>
        /// Runs the scheduled action immediately, if it has not been run by schedule yet. The action will not be
        /// executed again, when the originally scheduled delay passes.
        /// </summary>
        public void RunNow()
        {
            _scheduledAction(null, null);
        }

        /// <summary>
        /// Cancels the action. Does nothing, if the action has already been executed.
        /// </summary>
        public void Cancel()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
        }
    }
}