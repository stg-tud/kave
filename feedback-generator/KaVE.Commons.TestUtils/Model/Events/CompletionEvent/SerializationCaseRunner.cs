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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KaVE.Commons.TestUtils.Model.Events.CompletionEvent
{
    public static class SerializationCaseRunner
    {
        public static void RunSerializationExamples(Action<string, string> assertion, string source)
        {
            foreach (var testCase in GetTestCases(source))
            {
                try
                {
                    assertion(testCase.Item1, testCase.Item2);
                }
                catch (Exception exception)
                {
                    throw new Exception(
                        string.Format("An exception occured in case '{0}'", testCase.Item1),
                        exception);
                }
            }
        }

        private static IEnumerable<Tuple<string, string>> GetTestCases(string source)
        {
            /*
             *  Expected source content is:
             *  
             *  <source>
             *  <source>
             *  etc...
             *  <target>
             *  
             *  Empty lines are ignored.
             */

            var lines = ReadAllLines(source).RemoveEmptyLines().ToArray();
            var inputs = lines.Take(lines.Length - 1);
            var expected = lines[lines.Length - 1];

            var testCases = inputs.Select(input => new Tuple<string, string>(input, expected));
            return testCases;
        }

        private static IEnumerable<string> ReadAllLines(string source)
        {
            try
            {
                return File.ReadAllLines(source);
            }
            catch (Exception exception)
            {
                throw new IOException(string.Format("Could not find test source in {0}", source), exception);
            }
        }

        private static IEnumerable<string> RemoveEmptyLines(this IEnumerable<string> strings)
        {
            return strings.Where(line => !string.IsNullOrWhiteSpace(line));
        }
    }
}