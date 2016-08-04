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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO.Archives;

namespace KaVE.RS.SolutionAnalysis
{
    internal class JustReadRunner
    {
        private readonly string _dirIn;

        public JustReadRunner(string dirIn)
        {
            _dirIn = dirIn;
            Asserts.That(Directory.Exists(_dirIn), "directory does not exist {0}".FormatEx(_dirIn));
        }

        public void Run()
        {
            Console.WriteLine(@"Processing all .zip files found in '{0}'.", _dirIn);
            foreach (var zip in FindZips(_dirIn))
            {
                Console.WriteLine();
                Console.WriteLine(@"## {0} ##", zip);

                foreach (var e in ReadEventsFromZip(zip))
                {
                    Console.Write('.');
                }
            }
        }

        private static IEnumerable<string> FindZips(string dirIn)
        {
            return Directory.EnumerateFiles(dirIn, "*.zip", SearchOption.AllDirectories);
        }

        private static IEnumerable<IDEEvent> ReadEventsFromZip(string zip)
        {
            var ra = new ReadingArchive(zip);
            while (ra.HasNext())
            {
                yield return ra.GetNext<IDEEvent>();
            }
        }
    }
}