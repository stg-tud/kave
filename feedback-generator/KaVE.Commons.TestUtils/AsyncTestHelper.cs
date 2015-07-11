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

using System.Threading;
using NUnit.Framework;

namespace KaVE.Commons.TestUtils
{
    public static class AsyncTestHelper
    {
        public delegate bool WaitCondition();

        public static void WaitForCondition(WaitCondition condition, int maxWaitMillis = 10000)
        {
            const int sleepMillis = 100;
            var millisWaited = 0;
            while (!condition() && millisWaited < maxWaitMillis)
            {
                Thread.Sleep(sleepMillis);
                millisWaited += sleepMillis;
            }

            if (millisWaited >= maxWaitMillis)
            {
                Assert.Fail("Timeout reached.");
            }
        }
    }
}