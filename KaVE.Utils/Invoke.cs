using System;
using System.Threading;
using System.Windows;

namespace KaVE.Utils
{
    public static class Invoke
    {
        /// <summary>
        /// Invokes the passed action after some delay. Returns immediately.
        /// </summary>
        /// <param name="action">The action to run delayed.</param>
        /// <param name="delayInMillis">The delay in milliseconds.</param>
        /// <returns>The timer that was initiated.</returns>
        public static ScheduledAction Later(Action action, long delayInMillis)
        {
            return new ScheduledAction(action, delayInMillis);
        }

        public static ScheduledAction Later(Action action, System.DateTime dateToExecute)
        {
            var date = dateToExecute - System.DateTime.Now;
            return new ScheduledAction(action, Math.Max((long)date.TotalMilliseconds, 0));
        }

        /// <summary>
        /// Invokes the passed action asynchonuously. Returns immediately.
        /// </summary>
        /// <param name="action">The action to run.</param>
        public static void Async(Action action)
        {
            action.BeginInvoke(null, null);
        }

        /// <summary>
        /// Runs an <see cref="Action"/> in a single-threaded apartment
        /// (STA). Waits for the action to finish. Rethrows any exception
        /// thrown by the action.
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
        /// Runs a <see cref="Func{TResult}"/> in a single-threaded apartment
        /// (STA). Waits for the execution to finish and returns its result.
        /// Rethrows any exception thrown by the function.
        /// </summary>
        /// <param name="action">The function to run.</param>
        /// <returns>The function's result.</returns>
        // ReSharper disable once InconsistentNaming
        public static TResult OnSTA<TResult>(Func<TResult> action)
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
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// Runs an <see cref="Action"/> in the dispatcher thread.
        /// Waits for the action to finish. Rethrows any exception
        /// thrown by the action.
        /// </summary>
        /// <param name="action">The action to run.</param>
        public static void OnDispatcher(Action action)
        {
            var exception = default(Exception);
            Application.Current.Dispatcher.Invoke(
                (Action) (() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                }));
            if (exception != null)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Runs an <see cref="Action"/> in the dispatcher thread.
        /// Returns immediately.
        /// </summary>
        /// <param name="action">The action to run.</param>
        public static void OnDispatcherAsync(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(action);
        }
    }
}