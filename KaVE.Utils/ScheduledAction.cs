using System;
using System.Timers;

namespace KaVE.Utils
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
        /// <param name="millisToDelay">The number of milliseconds to delay exection.</param>
        public ScheduledAction(Action actionToSchedule, int millisToDelay)
        {
            _scheduledAction = (s, evtArgs) =>
            {
                lock (_timer)
                {
                    if (_timer.Enabled)
                    {
                        _timer.Stop();
                        actionToSchedule.Invoke();
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