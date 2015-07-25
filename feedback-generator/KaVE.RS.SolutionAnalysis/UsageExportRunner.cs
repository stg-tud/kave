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
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO;
using KaVE.Commons.Utils.ObjectUsageExport;

namespace KaVE.RS.SolutionAnalysis
{
    public class UsageExportRunner
    {
        private readonly string _dirIn;
        private readonly string _dirOut;
        private readonly UsageExtractor _usageExtractor = new UsageExtractor();

        public UsageExportRunner(string dirIn, string dirOut)
        {
            _dirIn = dirIn;
            _dirOut = dirOut;
        }

        public void Run()
        {
            var ctxZips = FindInputFiles();

            var numZips = ctxZips.Count();
            var currentZip = 1;

            var numTotalCtxs = 0;
            var numTotalUsages = 0;

            foreach (var fileName in ctxZips)
            {
                Log("### processing zip {0}/{1}: {2}", currentZip++, numZips, fileName);

                var numLocalCtxs = 0;
                var numLocalUsages = 0;

                var fullFileIn = _dirIn + fileName;
                var fullFileOut = _dirOut + fileName.Replace("-contexts.", "-usages.");

                using (var ra = new ReadingArchive(fullFileIn))
                {
                    Log("reading contexts...");
                    var ctxs = ra.GetAll<Context>();
                    var numCtxs = ctxs.Count;
                    Log("found {0} contexts", numCtxs);

                    EnsureParentExists(fullFileOut);

                    using (var wa = new WritingArchive(fullFileOut))
                    {
                        var currentCtx = 1;
                        foreach (var ctx in ctxs)
                        {
                            Log("    {0}/{1}", currentCtx++, numCtxs);
                            var usages = _usageExtractor.Export(ctx);
                            Append(" --> {0} usages", usages.Count);
                            wa.AddAll(usages);

                            numLocalCtxs++;
                            numLocalUsages += usages.Count;
                        }
                    }
                }

                Log("--> {0} contexts, {1} usages\n\n", numLocalCtxs, numLocalUsages);

                numTotalCtxs += numLocalCtxs;
                numTotalUsages += numLocalUsages;
            }

            Log("finished!");
            Log("found a total of {0} contexts and extracted {1} usages\n\n", numTotalCtxs, numTotalUsages);
        }

        private static void EnsureParentExists(string filePath)
        {
            var parent = Path.GetDirectoryName(filePath);
            Asserts.NotNull(parent);
            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(parent);
            }
        }

        private static void Log(string msg, params object[] args)
        {
            Console.Write("\n[{0}] ", DateTime.Now);
            Console.Write(msg, args);
        }

        private static void Append(string msg, params object[] args)
        {
            Console.Write(msg, args);
        }

        private IList<string> FindInputFiles()
        {
            var files = Directory.EnumerateFiles(_dirIn, "*-contexts.zip", SearchOption.AllDirectories);
            return files.Select(f => f.Substring(_dirIn.Length)).ToList();
        }
    }
}