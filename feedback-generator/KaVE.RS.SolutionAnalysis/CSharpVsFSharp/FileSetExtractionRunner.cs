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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.IO.Archives;

namespace KaVE.RS.SolutionAnalysis.CSharpVsFSharp
{
    public class FileSetExtractionRunner
    {
        private readonly string _root;

        public FileSetExtractionRunner(string root)
        {
            if (!root.EndsWith(@"\"))
            {
                root += @"\";
            }
            _root = root;
            Console.WriteLine(@"From: {0}", _root);
        }

        public void Run()
        {
            var res = ExtractActiveFilesPerUser();
            PrintFileSets(res);
            PrintStats(res);
        }

        private IDictionary<string, ISet<string>> ExtractActiveFilesPerUser()
        {
            var res = new Dictionary<string, ISet<string>>();

            var userZips = FindUserZips();

            var current = 1;
            var total = userZips.Count;

            foreach (var userZip in userZips)
            {
                Console.WriteLine(@"### {0} ({1}/{2}) -- {3} ###", userZip, current++, total, DateTime.Now);
                var fileSet = new HashSet<string>();
                res.Add(userZip, fileSet);

                foreach (var @event in ReadEventsForUser(userZip))
                {
                    Console.Write('.');
                    var doc = @event.ActiveDocument;
                    if (doc != null)
                    {
                        fileSet.Add(doc.FileName);
                    }
                }
                Console.WriteLine();
            }
            return res;
        }

        private IList<string> FindUserZips()
        {
            return new List<string>(
                Directory.EnumerateFiles(_root, "*.zip", SearchOption.AllDirectories)
                         .Select(f => f.Replace(_root, "")));
        }

        private IEnumerable<IDEEvent> ReadEventsForUser(string userZip)
        {
            var ra = new ReadingArchive(_root + userZip);
            while (ra.HasNext())
            {
                yield return ra.GetNext<IDEEvent>();
            }
        }

        private static void PrintFileSets(IDictionary<string, ISet<string>> res)
        {
            foreach (var user in res.Keys)
            {
                Console.WriteLine(@"### {0} ###", user);
                foreach (var file in res[user])
                {
                    Console.WriteLine(@"{0}", file);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static void PrintStats(IDictionary<string, ISet<string>> res)
        {
            foreach (var user in res.Keys)
            {
                var numCs = 0;
                var numFs = 0;
                var numFsx = 0;
                var numPhp = 0;
                var numOther = 0;

                foreach (var file in res[user])
                {
                    if (file.EndsWith(".cs"))
                    {
                        numCs++;
                    }
                    else if (file.EndsWith(".fs"))
                    {
                        numFs++;
                    }
                    else if (file.EndsWith(".fsx"))
                    {
                        numFsx++;
                    }
                    else if (file.EndsWith(".php"))
                    {
                        numPhp++;
                    }
                    else
                    {
                        //Console.WriteLine(@"other: {0}", file);
                        numOther++;
                    }
                }

                Console.WriteLine(@"### {0} ###", user);
                Console.WriteLine(@"cs: {0}", numCs);
                Console.WriteLine(@"fs: {0}", numFs);
                Console.WriteLine(@"fsx: {0}", numFsx);
                Console.WriteLine(@"php: {0}", numPhp);
                Console.WriteLine(@"other: {0}", numOther);
                Console.WriteLine();
            }
        }
    }
}