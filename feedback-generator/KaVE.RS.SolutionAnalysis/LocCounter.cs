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
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Utils.IO.Archives;

namespace KaVE.RS.SolutionAnalysis
{
    public class LocCounter
    {
        public void Run(string rootDir)
        {
            int numRepos = 0;
            int numSolutions = 0;
            int numSSTs = 0;
            long loc = 0;

            int i = 0;
            foreach (var user in GetSubdirs(rootDir))
            {
                foreach (var repo in GetSubdirs(Path.Combine(rootDir, user)))
                {
                    numRepos++;

                    Console.Write("##### {0}/{1} ##############################", user, repo);

                    var repoPath = Path.Combine(rootDir, user, repo);

                    foreach (var zip in GetArchives(repoPath))
                    {
                        numSolutions++;

                        if (i++ >= 20)
                        {
                            Console.WriteLine("fancy abort via goto :D");
                            goto ENDE;
                        }

                        Console.WriteLine();
                        Console.WriteLine("@@ {0} @@", zip);
                        var zipPath = Path.Combine(repoPath, zip);
                        var ra = new ReadingArchive(zipPath);
                        while (ra.HasNext())
                        {
                            numSSTs++;

                            Console.Write('.');
                            var ctx = ra.GetNext<Context>();

                            var sstloc = CountLoc(ctx.SST);
                            loc += sstloc;
                        }
                    }
                }
            }

            ENDE:
            Console.WriteLine("#repos: {0}", numRepos);
            Console.WriteLine("#solutions: {0}", numSolutions);
            Console.WriteLine("#types: {0}", numSSTs);
            Console.WriteLine("loc: {0}", loc);
        }

        private readonly LinesOfCodeVisitor _locVisitor = new LinesOfCodeVisitor();

        private int CountLoc(ISST sst)
        {
            return sst.Accept(_locVisitor, 0);
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
}