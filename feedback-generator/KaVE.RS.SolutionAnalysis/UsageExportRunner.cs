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
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO.Archives;
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
            var startTime = DateTime.Now;
            var ctxZips = FindInputFiles();

            var numZips = ctxZips.Count();
            var currentZip = 1;

            var numTotalCtxs = 0;
            var numTotalUsages = 0;

            using (var cache = new ZipFolderLRUCache<CoReTypeName>(_dirOut, 1000))
            {
                foreach (var fileName in ctxZips)
                {
                    try
                    {
                        Log("### processing zip {0}/{1}: {2}", currentZip++, numZips, fileName);

                        var numLocalCtxs = 0;
                        var numLocalUsages = 0;

                        var fullFileIn = _dirIn + fileName;

                        var ra = new ReadingArchive(fullFileIn);
                        Log("reading contexts...");
                        var ctxs = ra.GetAll<Context>();
                        var numCtxs = ctxs.Count;
                        Log("found {0} contexts, exporting usages...\n\n", numCtxs);

                        var colCounter = 0;
                        foreach (var ctx in ctxs)
                        {
                            if (colCounter == 25)
                            {
                                Console.WriteLine();
                                colCounter = 0;
                            }
                            colCounter++;

                            var usages = ExtractUsages(ctx);
                            var msg = usages.Count == 0
                                ? "."
                                : string.Format("{0}", usages.Count);
                            Console.Write(msg.PadLeft(5));

                            foreach (var u in usages)
                            {
                                cache.GetArchive(u.type).Add(u);
                            }

                            numLocalCtxs++;
                            numLocalUsages += usages.Count;
                        }

                        Append("\n");
                        Log(
                            "--> {0} contexts, {1} usages\n\n",
                            numLocalCtxs,
                            numLocalUsages);

                        numTotalCtxs += numLocalCtxs;
                        numTotalUsages += numLocalUsages;
                    }
                    catch (Exception e)
                    {
                        Log("oops, something went wrong...\n{0}", e);
                    }
                }
            }

            Log("finished! (started at {0})", startTime);
            Log(
                "found a total of {0} contexts and extracted {1} usages\n\n",
                numTotalCtxs,
                numTotalUsages);
        }

        private IKaVEList<Query> ExtractUsages(Context ctx)
        {
            try
            {
                return _usageExtractor.Export(ctx);
            }
            catch (Exception e)
            {
                Log("error!!\n{0}", e);
                return Lists.NewList<Query>();
            }
        }

        private static void Log(string msg, params object[] args)
        {
            Console.Write(Environment.NewLine + @"[{0}] ", DateTime.Now);
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