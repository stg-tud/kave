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
 *    - Mattis Manfred Kämmerer
 */

using System;

namespace KaVE.FeedbackProcessor.Tests.TestUtils
{
    internal class DateTimeFactory
    {
        public static readonly Random Random = new Random();

        public static DateTime SomeDateTime()
        {
            try
            {
                return new DateTime(Random.Next());
            }
            catch
            {
                return new DateTime();
            }
        }

        public static DateTime SomeDateTime(int max)
        {
            try
            {
                return new DateTime(Random.Next(max));
            }
            catch
            {
                return new DateTime();
            }
            
        }

        public static DateTime SomeDateTime(int min, int max)
        {
            try
            {
                return new DateTime(Random.Next(min, max));
            }
            catch
            {
                return new DateTime();
            }
        }
    }
}