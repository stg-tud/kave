using System;
using System.Threading;
using System.Windows;

namespace KaVE.Utils
{
    public static class Invoke
    {
        /// <summary>
        ///     Invokes the passed action after some delay. Returns immediately.
        /// </summary>
        /// <param name="action">The action to run delayed.</param>
        /// <param name="delayInMillis">The delay in milliseconds. Expected to be greater than 0.</param>
        public static ScheduledAction Later(Action action, long delayInMillis)
        {
            return new ScheduledAction(action, delayInMillis);
        }

        /// <summary>
        ///     Invokes the passed action on the given date. When the date is in the past, the action is executed immediately.
        /// </summary>
        /// <param name="action">The action to run on the given date.</param>
        /// <param name="executionDateTime">The date to execute the action on.</param>
        public static ScheduledAction Later(Action action, System.DateTime executionDateTime)
        {
            var date = executionDateTime - System.DateTime.Now;
            return new ScheduledAction(action, Math.Max((long) date.TotalMilliseconds, 1));
        }

        /// <summary>
        ///     Invokes the passed action asynchonuously. Returns immediately.
        /// </summary>
        /// <param name="action">The action to run.</param>
        public static void Async(Action action)
        {
            action.BeginInvoke(null, null);
        }

        /// <summary>
        ///     Runs an <see cref="Action" /> in a single-threaded apartment
        ///     (STA). Waits for the action to finish. Rethrows any exception
        ///     thrown by the action.
        /// </summary>
        /// <param name="action">The action to run.</param>
        // ReSharper disable once InconsistentNaming
        public static void OnSTA(Action action)
        {
            OnSTA<object>(
                () =>
                {
                    action.Invoke();
                    return null;
                });
        }

        /// <summary>
        ///     Runs a <see cref="Func{TResult}" /> in a single-threaded apartment
        ///     (STA). Waits for the execution to finish and returns its result.
        ///     Rethrows any exception thrown by the function.
        /// </summary>
        /// <param name="action">The function to run.</param>
        /// <returns>The function's result.</returns>
        // ReSharper disable once InconsistentNaming
        public static TResult OnSTA<TResult>(Func<TResult> action)
        {
            return IsSTAInitialized() ? OnExistingSTA(action) : OnNewSTA(action);
        }

        // ReSharper disable once InconsistentNaming
        private static bool IsSTAInitialized()
        {
            return Application.Current != null;
        }

        // ReSharper disable once InconsistentNaming
        private static TResult OnExistingSTA<TResult>(Func<TResult> actionAsFunc)
        {
            var dispatcher = Application.Current.Dispatcher;
            if (dispatcher.CheckAccess())
            {
                return actionAsFunc.Invoke();
            }

            var result = default(TResult);
            var exception = default(Exception);
            dispatcher.Invoke(
                new Action(
                    delegate
                    {
                        try
                        {
                            result = actionAsFunc.Invoke();
                        }
                        catch (Exception e)
                        {
                            exception = e;
                        }
                    }));
            if (exception != null)
            {
                throw new Exception("dispatched action threw exception", exception);
            }
            return result;
        }

        // ReSharper disable once InconsistentNaming
        private static TResult OnNewSTA<TResult>(Func<TResult> action)
        {
            var result = default(TResult);
            var exception = default(Exception);
            var thread = new Thread(
                () =>
                {
                    try
                    {
                        result = action.Invoke();
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            if (exception != null)
            {
                throw new Exception("dispatched action threw exception", exception);
            }
            return result;
        }

        /// <summary>
        ///     Runs an <see cref="Action" /> in the dispatcher thread.
        ///     Returns immediately.
        /// </summary>
        /// <param name="action">The action to run.</param>
        public static void OnDispatcherAsync(Action action)
        {
            // if this should fail (Application.Current == null), this should be solved like the synchronous case above
            Application.Current.Dispatcher.BeginInvoke(action);
        }
    }
}