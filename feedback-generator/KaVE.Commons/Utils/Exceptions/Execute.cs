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
using System.Linq;
using KaVE.Commons.Utils.Assertion;
using Newtonsoft.Json;

namespace KaVE.Commons.Utils.Exceptions
{
    public static class Execute
    {
        private static readonly Type[] SupressedExceptionTypes =
        {
            typeof(AssertException),
            typeof(JsonException),
            typeof(NullReferenceException),
            typeof(ArgumentException),
            typeof(ArithmeticException),
            typeof(IndexOutOfRangeException),
            typeof(FormatException),
            typeof(InvalidCastException),
            typeof(InvalidOperationException),
            typeof(NotSupportedException)
        };

        public static void WithExceptionLogging(ILogger logger, Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                if (SupressedExceptionTypes.Contains(e.GetType()))
                {
                    logger.Error(e, "executing action failed");
                    return;
                }
                throw;
            }
        }

        public static void WithExceptionCallback(Action action, Action<Exception> cb)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                if (SupressedExceptionTypes.Contains(e.GetType()))
                {
                    cb(e);
                    return;
                }
                throw;
            }
        }

        public static void AndSupressExceptions(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                if (!SupressedExceptionTypes.Contains(e.GetType()))
                {
                    throw;
                }
            }
        }
    }
}