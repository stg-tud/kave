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
        private const char Separator = '|';

        public static void RunSerializationExamples(Action<string, string> assertion, string source)
        {
            foreach (var testCase in GetTestCases(source))
            {
                try
                {
                    assertion(testCase.Input, testCase.Expected);
                }
                catch (Exception exception)
                {
                    throw new Exception(
                        string.Format("An exception occured in case '{0}'", testCase.Name),
                        exception);
                }
            }
        }

        private static IEnumerable<TestCase> GetTestCases(string source)
        {
            /*
             *  Expected source content is:
             *  
             *  <case_name>|<case_input>
             *  <case_name>|<case_input>
             *  ...
             *  <expected>
             *  
             *  Empty lines are ignored.
             *  case_name is optional.
             */

            var lines = ReadAllLines(source).RemoveEmptyLines().ToArray();
            var caseLines = lines.Take(lines.Length - 1);
            var expectedLine = lines[lines.Length - 1];

            var testCases =
                caseLines.Select(
                    nameAndInput =>
                        new TestCase(GetTestCaseName(nameAndInput), GetTestCaseInput(nameAndInput), expectedLine));
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

        private static string GetTestCaseName(string sourceLine)
        {
            return sourceLine.Split(Separator)[0];
        }

        private static string GetTestCaseInput(string sourceLine)
        {
            try
            {
                return sourceLine.Split(Separator)[1];
            }
            catch
            {
                // For unnamed cases name and input are the same
                return GetTestCaseName(sourceLine);
            }
        }

        private struct TestCase
        {
            public readonly string Input;
            public readonly string Expected;
            public readonly string Name;

            public TestCase(string name, string input, string expected)
            {
                Expected = expected;
                Input = input;
                Name = name;
            }
        }
    }
}