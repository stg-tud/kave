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
using System.Threading;
using System.Windows;

namespace KaVE.Commons.Utils
{
    // TODO write tests for this class (some tests exists transitively through the CallbackManager; these should be moved!)
    public static class Invoke
    {
        /// <summary>
        ///     Invokes the passed action after some delay. Returns immediately.
        /// </summary>
        /// <param name="action">The action to run delayed.</param>
        /// <param name="delayInMillis">The delay in milliseconds. For values smaller than 0 the action is scheduled immediately.</param>
        public static ScheduledAction Later(Action action, long delayInMillis)
        {
            return Later(action, delayInMillis, () => { });
        }

        public static ScheduledAction Later(Action action, long delayInMillis, Action finishedAction)
        {
            delayInMillis = Math.Max(delayInMillis, 1);
            return new ScheduledAction(action, delayInMillis, finishedAction);
        }

        /// <summary>
        ///     Invokes the passed action on the given date. When the date is in the past, the action is executed immediately.
        /// </summary>
        /// <param name="action">The action to run on the given date.</param>
        /// <param name="executionDateTime">The date to execute the action on. For dates older than DateTime.Now the action is scheduled immediately.</param>
        public static ScheduledAction Later(Action action, System.DateTime executionDateTime)
        {
            var date = executionDateTime - System.DateTime.Now;
            return new ScheduledAction(action, Math.Max((long) date.TotalMilliseconds, 1));
        }

        public static ScheduledAction Later(Action action, System.DateTime executionDateTime, Action finishedAction)
        {
            var date = executionDateTime - System.DateTime.Now;
            return new ScheduledAction(action, Math.Max((long) date.TotalMilliseconds, 1), finishedAction);
        }

        /// <summary>
        ///     Invokes the passed action asynchronuously. Returns immediately.
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
            var application = Application.Current;
            return application != null && application.Dispatcher.Thread.GetApartmentState() == ApartmentState.STA;
        }

        // ReSharper disable once InconsistentNaming
        private static TResult OnExistingSTA<TResult>(Func<TResult> action)
        {
            var dispatcher = Application.Current.Dispatcher;
            var result = default(TResult);
            var exception = default(Exception);
            dispatcher.Invoke(
                new Action(
                    delegate
                    {
                        try
                        {
                            result = action.Invoke();
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