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

namespace KaVE.RS.SolutionAnalysis
{
    public class FailingRepoFinder
    {
        private readonly IFailingRepoLogger _log;

        public FailingRepoFinder(IFailingRepoLogger log)
        {
            _log = log;
        }

        public void Run(string rootDir)
        {
            _log.Start(rootDir);

            foreach (var user in GetSubdirs(rootDir))
            {
                _log.User(user);
                foreach (var repo in GetSubdirs(Path.Combine(rootDir, user)))
                {
                    _log.Repo(repo);

                    var zips = GetArchives(Path.Combine(rootDir, user, repo));
                    foreach (var zip in zips)
                    {
                        _log.Zip(zip);
                    }
                }
            }

            _log.End();
        }

        private static IEnumerable<string> GetSubdirs(string dir)
        {
            return
                Directory.EnumerateDirectories(dir)
                         .Select(d => d.Replace(dir + @"\", ""))
                         .Select(d => d.Replace(dir, ""));
        }

        private static IEnumerable<string> GetArchives(string dir)
        {
            return
                Directory.EnumerateFiles(dir, "*.zip", SearchOption.AllDirectories)
                         .Select(f => f.Replace(dir + @"\", ""))
                         .Select(f => f.Replace(dir, ""));
        }
    }

    public interface IFailingRepoLogger
    {
        void Start(string rootDir);
        void User(string user);
        void Repo(string repo);
        void Zip(string zip);
        void End();
    }

    public class FailingRepoLogger : IFailingRepoLogger
    {
        public void Start(string rootDir)
        {
            Console.WriteLine("processing: {0}", rootDir);
        }

        private string _curUser;
        private string _curRepo;

        public void User(string user)
        {
            _curUser = user;
            Console.WriteLine();
            Console.WriteLine("###### {0} #######################", user);
        }

        private bool _hasZips = true;

        public void Repo(string repo)
        {
            CheckZipFound();

            _curRepo = repo;
            _hasZips = false;
            Console.WriteLine();
            Console.WriteLine("== {0} ==", repo);
        }

        private readonly ISet<string> _noZips = new HashSet<string>();

        private void CheckZipFound()
        {
            if (!_hasZips)
            {
                _noZips.Add(_curUser + "/" + _curRepo);
                Console.WriteLine("No ZIP found in {0}/{1}!!", _curUser, _curRepo);
            }
        }

        public void Zip(string zip)
        {
            _hasZips = true;
            Console.WriteLine("  - {0}", zip);
        }

        public void End()
        {
            CheckZipFound();

            Console.WriteLine("no zips found for:");
            foreach (var s in _noZips)
            {
                Console.WriteLine("* {0}", s);
            }
        }
    }
}