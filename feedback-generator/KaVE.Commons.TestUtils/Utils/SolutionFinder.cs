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
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.TestUtils.Utils
{
    public class SolutionFinder
    {
        public const string IndexFile = "index.json";
        public const string StartedFile = "started.json";
        public const string EndedFile = "ended.json";
        public const string CrashedFile = "crashed.json";

        public string Root { get; private set; }

        private readonly IDictionary<string, IList<string>> _cache = new Dictionary<string, IList<string>>();

        [NotNull]
        public IEnumerable<string> Solutions
        {
            get
            {
                if (_cache.ContainsKey(IndexFile))
                {
                    return _cache[IndexFile];
                }

                if (HasIndex())
                {
                    var slns = Read(IndexFile);
                    _cache[IndexFile] = slns;
                    return slns;
                }
                else
                {
                    var slns = _funFindSolution();
                    Write(slns, IndexFile);
                    return slns;
                }
            }
        }

        private void Write(IList<string> slns, string file)
        {
            _cache[file] = slns;

            var json = slns.ToFormattedJson();
            var fullPath = GetFullPath(file);
            File.WriteAllText(fullPath, json);
        }


        private IList<string> Read(string file)
        {
            if (_cache.ContainsKey(file))
            {
                return _cache[file];
            }

            var fullPath = GetFullPath(file);
            if (!File.Exists(fullPath))
            {
                return new List<string>();
            }
            var json = File.ReadAllText(fullPath);
            return json.ParseJsonTo<List<string>>();
        }

        private bool HasIndex()
        {
            return File.Exists(GetFullPath(IndexFile));
        }

        private readonly Func<IList<string>> _funFindSolution;

        private IList<string> FindSolutions_OrderedAlphabetically()
        {
            var all = Directory.GetFiles(Root, "*.sln", SearchOption.AllDirectories);
            var filtered = all.Where(sln => !sln.Contains(@"\test\data\"));
            var shortened = filtered.Select(sln => sln.Substring(Root.Length));
            var ordered = shortened.OrderBy(sln => sln);
            return Lists.NewListFrom(ordered.Distinct());
        }

        private IList<string> FindSolutions_Random()
        {
            var rng = new Random(DateTime.Now.GetHashCode());
            var slns = FindSolutions_OrderedAlphabetically();
            var oslns = slns.OrderBy(sln => rng.Next());
            return Lists.NewListFrom(oslns);
        }

        /// <summary>
        ///     be aware that the order parameter is ignored if an index already exists
        /// </summary>
        /// <param name="root"></param>
        /// <param name="orderBy"></param>
        public SolutionFinder(string root, OrderBy orderBy)
        {
            switch (orderBy)
            {
                case OrderBy.Alphabetical:
                    _funFindSolution = FindSolutions_OrderedAlphabetically;
                    break;
                case OrderBy.Random:
                    _funFindSolution = FindSolutions_Random;
                    break;
                default:
                    Asserts.Fail("not implemented");
                    break;
            }
            if (!root.EndsWith("\\"))
            {
                root += "\\";
            }
            Root = root;
            Asserts.That(Directory.Exists(Root));
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

    public enum OrderBy
    {
        Alphabetical,
        Random
    }
}