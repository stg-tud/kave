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

using System;
using System.Threading;

namespace KaVE.Commons.Utils.Exceptions
{
    public class TimeLimitRunner
    {
        public static void Run<TResult>(Func<TResult> task,
            int limitInMs,
            CancellationToken token,
            Action<TResult> onSuccess,
            Action onTimeout,
            Action<Exception> onError) where TResult : class
        {
            TResult result = null;
            Exception exception = null;

            var execDoneHandle = new ManualResetEvent(false);

            var thread = new Thread(
                () =>
                {
                    try
                    {
                        result = task();
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                    execDoneHandle.Set();
                });

            thread.Start();
            WaitHandle.WaitAny(new[] {execDoneHandle, token.WaitHandle}, limitInMs);

            if (!token.IsCancellationRequested)
            {
                if (result != null)
                {
                    onSuccess(result);
                }
                else if (exception != null)
                {
                    // TODO think over selective rethrow (e.g., on thread interrupt)
                    onError(exception);
                }
                else
                {
                    onTimeout();
                }
            }

            thread.Abort();
        }
    }
}