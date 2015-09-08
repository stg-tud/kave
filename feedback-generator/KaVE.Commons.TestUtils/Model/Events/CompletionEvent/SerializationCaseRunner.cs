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

            var allLines = ReadAllLines(source).RemoveEmptyLines().ToArray();
            var sources = allLines.ToList().Take(allLines.Length - 1).ToArray();
            var target = allLines[allLines.Length - 1];

            var cases = new Tuple<string, string>[sources.Length];
            for (var i = 0; i < sources.Length; i++)
            {
                cases[i] = new Tuple<string, string>(sources[i], target);
            }

            return cases;
        }

        private static string[] ReadAllLines(string source)
        {
            return File.ReadAllLines(source);
        }

        private static IEnumerable<string> RemoveEmptyLines(this IEnumerable<string> strings)
        {
            return strings.Where(line => !string.IsNullOrWhiteSpace(line));
        }
    }
}