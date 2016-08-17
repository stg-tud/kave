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
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Json;

namespace KaVE.Commons.TestUtils.Utils
{
    public class SolutionFinder
    {
        public const string IndexFile = "index.json";
        public const string StartedFile = "started.json";
        public const string EndedFile = "ended.json";
        public const string CrashedFile = "crashed.json";

        public string Root { get; private set; }

        private IEnumerable<string> _solutions;

        public IEnumerable<string> Solutions
        {
            get
            {
                if (_solutions != null)
                {
                    return _solutions;
                }
                if (HasIndex())
                {
                    return _solutions = Read(IndexFile);
                }
                _solutions = FindSolutions();
                Write(_solutions, IndexFile);
                return _solutions;
            }
        }

        private void Write(IEnumerable<string> slns, string file)
        {
            var json = slns.ToFormattedJson();
            var fullPath = GetFullPath(file);
            File.WriteAllText(fullPath, json);
        }


        private ISet<string> Read(string file)
        {
            var fullPath = GetFullPath(file);
            if (!File.Exists(fullPath))
            {
                return new HashSet<string>();
            }
            var json = File.ReadAllText(fullPath);
            return json.ParseJsonTo<HashSet<string>>();
        }

        private bool HasIndex()
        {
            return File.Exists(GetFullPath(IndexFile));
        }

        private ISet<string> FindSolutions()
        {
            var all = Directory.GetFiles(Root, "*.sln", SearchOption.AllDirectories);
            var filtered = all.Where(sln => !sln.Contains(@"\test\data\"));
            var shortened = filtered.Select(sln => sln.Substring(Root.Length));
            var ordered = shortened.OrderBy(sln => sln);
            return Sets.NewHashSetFrom(ordered);
        }

        public SolutionFinder(string root)
        {
            if (!root.EndsWith("\\"))
            {
                root += "\\";
            }
            Root = root;
            Asserts.That(Directory.Exists(Root));
        }


        public IEnumerable<string> ReadOrCreateIndex()
        {
            try
            {
                var indexFile = Path.Combine(Root, "index.json");
                if (File.Exists(indexFile))
                {
                    Console.WriteLine("Reading index... {0}", DateTime.Now);
                    var json = File.ReadAllText(indexFile);
                    return json.ParseJsonTo<IEnumerable<string>>();
                }

                Console.WriteLine("Finding solutions... {0}", DateTime.Now);
                var all = Directory.GetFiles(Root, "*.sln", SearchOption.AllDirectories);
                var filtered = all.Where(sln => !sln.Contains(@"\test\data\"));
                var shortened = filtered.Select(sln => sln.Substring(Root.Length));

                Console.WriteLine("Creating index... {0}", DateTime.Now);
                File.WriteAllText(indexFile, shortened.ToCompactJson());
                return shortened;
            }
            catch (IOException e)
            {
                return new string[0];
            }
        }

        public string GetFullPath(string file)
        {
            return Path.Combine(Root, file);
        }

        public void Start(string sln)
        {
            Asserts.That(Solutions.Contains(sln));
            var started = Read(StartedFile);
            Asserts.Not(started.Contains(sln));
            started.Add(sln);
            Write(started, StartedFile);
        }

        public void End(string sln)
        {
            Asserts.That(Solutions.Contains(sln));

            var started = Read(StartedFile);
            Asserts.That(started.Remove(sln));
            Write(started, StartedFile);

            var ended = Read(EndedFile);
            ended.Add(sln);
            Write(ended, EndedFile);
        }

        public void Crash(string sln)
        {
            Asserts.That(Solutions.Contains(sln));

            var started = Read(StartedFile);
            Asserts.That(started.Remove(sln));
            Write(started, StartedFile);

            var crashed = Read(CrashedFile);
            crashed.Add(sln);
            Write(crashed, CrashedFile);
        }

        public bool ShouldIgnore(string sln)
        {
            Asserts.That(Solutions.Contains(sln));
            var isStarted = Read(StartedFile).Contains(sln);
            var isEnded = Read(EndedFile).Contains(sln);
            return isStarted || isEnded || Read(CrashedFile).Contains(sln);
        }

        public IEnumerable<string[]> GetTestData()
        {
            var count = 0;

            foreach (var sln in Solutions)
            {
                var label = string.Format("{0:0000}", count++);
                yield return new[] {label, sln};
            }
        }
    }
}