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
using System.IO;
using System.Linq;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.TestUtils.Model.Events.CompletionEvent
{
    public static class SerializationCaseRunner
    {
        public static void RunSerializationExamples(Action<string, string> assertion, string caseSource)
        {
            var sourcesToTarget = GetSerializationStrings(caseSource);

            foreach (var source in sourcesToTarget.Item1)
            {
                assertion(source, sourcesToTarget.Item2);
            }
        }

        private static Tuple<string[], string> GetSerializationStrings(string source)
        {
            /*
             *  Expected source content is:
             *  
             *  <source>
             *  <source>
             *  etc...
             *  <empty line>
             *  <target>
             */

            var allLines = ReadAllLines(source);
            var sources = allLines.ToList().Take(allLines.Length - 2).ToArray();
            var target = allLines[allLines.Length - 1];

            return new Tuple<string[], string>(sources, target);
        }

        [Pure]
        private static string[] ReadAllLines(string source)
        {
            return File.ReadAllLines(source);
        }
    }
}