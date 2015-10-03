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
using KaVE.Commons.Utils.IO;
using KaVE.Commons.Utils.ObjectUsageExport;
using KaVE.RS.SolutionAnalysis.CompletionEventToMicroCommits;

namespace KaVE.RS.SolutionAnalysis
{
    public class Test
    {
        private Test _t;

        public void M() {}
    }

    internal class Program
    {
        private const string DirRoot = @"E:\";
        private const string DirEventsAll = DirRoot + @"Events\All\";
        private const string DirEventsCompletion_KeepNoTrigger = DirRoot + @"Events\OnlyCompletion\";
        private const string DirEventsCompletion_KeepNoTriggerInlined = DirRoot + @"Events\OnlyCompletion-inlined\";
        private const string DirEventsCompletion_RemoveNoTrigger = DirRoot + @"Events\OnlyCompletionWithTriggerPoint\";
        private const string DirHistories = DirRoot + @"Histories\";
        private const string DirHistories_Inlined = DirRoot + @"Histories-inlined\";
        private const string DirContexts = DirRoot + @"Contexts\";
        private const string DirContexts_Inlined = DirRoot + @"Contexts-inlined\";
        private const string DirUsages = DirRoot + @"Usages\";
        private const string DirUsages_Inlined = DirRoot + @"Usages-inlined\";
        private const string DirEpisodes = DirRoot + @"Episodes\";

        private static void Main(string[] args)
        {
            Console.WriteLine(@"{0} start", DateTime.Now);

            // data preparation
            RunUsageExport(DirContexts, DirUsages);
            RunUsageExport(DirContexts_Inlined, DirUsages_Inlined);
            RunCompletionEventFilter(
                DirEventsAll,
                DirEventsCompletion_KeepNoTrigger,
                CompletionEventFilter.NoTriggerPointOption.Keep);
            RunCompletionEventToMicroCommit(DirEventsCompletion_KeepNoTrigger, DirHistories);
            RunCompletionEventToMicroCommit(DirEventsCompletion_KeepNoTriggerInlined, DirHistories_Inlined);
            //RunEventStreamExport(DirContexts, DirEpisodes);

            // evaluations
            new EditLocationRunner(DirEventsCompletion_KeepNoTrigger).Run();


            Console.WriteLine(@"{0} finish", DateTime.Now);
        }

        private static void RunUsageExport(string dirContextsAll, string dirUsages)
        {
            CleanDirs(dirUsages);
            new UsageExportRunner(dirContextsAll, dirUsages).Run();
        }

        private static void RunCompletionEventFilter(string dirIn,
            string dirOut,
            CompletionEventFilter.NoTriggerPointOption noTriggerPointOption)
        {
            CleanDirs(dirOut);
            Console.WriteLine(@"reading from: {0}", dirIn);
            Console.WriteLine(@"writing to:   {0}", dirOut);
            Console.WriteLine(@"option: {0}", noTriggerPointOption);
            new CompletionEventFilter(
                dirIn,
                dirOut,
                noTriggerPointOption,
                new IoUtils(),
                new CompletionEventFilterLogger()).Run();
        }

        private static void RunCompletionEventToMicroCommit(string eventDir, string historiesDir)
        {
            CleanDirs(historiesDir);
            Console.WriteLine(@"reading from: {0}", eventDir);
            Console.WriteLine(@"writing to:   {0}", historiesDir);
            var usageExtractor = new UsageExtractor();
            var microCommitGenerator = new MicroCommitGenerator(usageExtractor);
            var ioHelper = new IoHelper(eventDir, historiesDir);
            new CompletionEventToMicroCommitRunner(microCommitGenerator, ioHelper).Run();
        }

        private static void RunEventStreamExport(string dirCtxIn, string dirEventsOut)
        {
            CleanDirs(DirEpisodes);
            new EventsExportRunner(dirCtxIn, dirEventsOut).Run();
        }

        private static void CleanDirs(params string[] dirs)
        {
            foreach (var dir in dirs)
            {
                Console.WriteLine(@"cleaning: {0}", dir);
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }
                Directory.CreateDirectory(dir);
            }
        }
    }
}