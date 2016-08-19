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
using KaVE.Commons.Tests.Model.Naming;
using KaVE.Commons.Utils.IO;
using KaVE.Commons.Utils.ObjectUsageExport;
using KaVE.RS.Commons;
using KaVE.RS.SolutionAnalysis.CleanUp;
using KaVE.RS.SolutionAnalysis.CleanUp.Filters;
using KaVE.RS.SolutionAnalysis.CompletionEventStatistics;
using KaVE.RS.SolutionAnalysis.CompletionEventToMicroCommits;
using KaVE.RS.SolutionAnalysis.CSharpVsFSharp;
using KaVE.RS.SolutionAnalysis.SortByUser;
using KaVE.RS.SolutionAnalysis.StatisticsForPapers;
using KaVE.RS.SolutionAnalysis.UserProfileExports;
using KaVE.RS.SolutionAnalysis.UserStatistics;

// ReSharper disable InconsistentNaming

namespace KaVE.RS.SolutionAnalysis
{
    internal class Program
    {
        //private const string DirRoot = @"C:\Users\Jonas\Desktop\SST-2016-02-19\Github";
        private const string DirRoot = @"E:\";
        private const string DirEventsAll = DirRoot + @"Events\All\";

        private const string DirEventsAll_SortedByUser = DirRoot + @"Events\All-SortedByUser\";

        private const string DirEventsAll_Clean = DirRoot + @"Events\All-Clean\";
        private const string DirEventsCompletion_KeepNoTrigger = DirRoot + @"Events\OnlyCompletion\";
        private const string DirEventsCompletion_KeepNoTriggerInlined = DirRoot + @"Events\OnlyCompletion-inlined\";
        private const string DirEventsCompletion_RemoveNoTrigger = DirRoot + @"Events\OnlyCompletionWithTriggerPoint\";
        private const string DirMicroCommits = DirRoot + @"MicroCommits\";
        private const string DirMicroCommits_Inlined = DirRoot + @"MicroCommits-inlined\";
        private const string DirContexts = DirRoot + @"Contexts\";
        private const string DirContexts_Github = DirContexts + @"Github\";
        private const string DirContexts_Inlined = DirRoot + @"Contexts-inlined\";
        private const string DirUsages = DirRoot + @"Usages\";
        private const string DirUsages_Inlined = DirRoot + @"Usages-inlined\";
        private const string DirEventStream = DirRoot + @"EventStreamForEpisodeMining\";

        private static void Main(string[] args)
        {
            //Console.WriteLine(@"{0} start", DateTime.Now);


            // new JustReadRunner(DirEventsAll).Run();
            //new JavaNamingTestGenerator().Run();
            new MultiRunner().Run();

            return;

            /* data preparation */
            RunSortByUser(DirEventsAll, DirEventsAll_SortedByUser);
            RunCleanUp(DirEventsAll_SortedByUser, DirEventsAll_Clean);
            //RunFailingRepoFinder();
            //RunApiStatisticsRunner();
            //RunCompletionEventStatistics();
            //RunUsageExport(DirContexts, DirUsages);
            //RunUsageExport(DirContexts_Inlined, DirUsages_Inlined);
            RunCompletionEventFilter(CompletionEventFilter.NoTriggerPointOption.Keep);
            //RunCompletionEventFilter(CompletionEventFilter.NoTriggerPointOption.Remove);
            //RunCompletionEventToMicroCommit(DirEventsCompletion_KeepNoTrigger, DirMicroCommits);
            //RunCompletionEventToMicroCommit(DirEventsCompletion_KeepNoTriggerInlined, DirMicroCommits_Inlined);
            //RunEventStreamExport(DirContexts, DirEventStream);
            //RunQuickSanityCheck();
            /* evaluations */
            //new EditLocationRunner(DirEventsCompletion_KeepNoTrigger).Run();
            //new EditLocationRunner(DirEventsCompletion_KeepNoTriggerInlined).Run();
            //RunUserProfileExport();
            //RunStatisticsForPaperCreation();
            //RunCShaspVsFSharpStats();
            RunUserStats(); // used to export positions used in the Demographic generator on Java

            RunNameGrabber();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(@"{0} finish", DateTime.Now);
        }

        private static void RunUserStats()
        {
            new UserStatsRunner(new UserStatsIo(DirEventsAll_Clean, "")).Run();
        }

        private static void RunCShaspVsFSharpStats()
        {
            new FileSetExtractionRunner(DirEventsAll_Clean).Run();
        }

        private static void RunNameGrabber()
        {
            new NameGrabber(DirRoot, DirRoot, -1, -1, true).Run();
        }

        private static void RunQuickSanityCheck()
        {
            new SanityCheck(DirEventsCompletion_KeepNoTrigger).Run();
        }

        private static void RunCompletionEventStatistics()
        {
            var io = new CompletionEventStatisticsIo(DirEventsAll_Clean);
            var log = new CompletionEventStatisticsLogger();
            new CompletionEventStatisticsRunner(io, log).Run();
        }

        private static void RunApiStatisticsRunner()
        {
            new ApiStatisticsRunner().Run(DirContexts_Github);
            //new LocCounter().Run(DirContexts_Github);
        }

        private static void RunFailingRepoFinder()
        {
            var log = new FailingRepoLogger();
            new FailingRepoFinder(log).Run(DirContexts_Github);
        }

        private static void RunCleanUp(string dirIn, string dirOut)
        {
            CleanDirs(dirOut);
            var log = new CleanUpLogger();
            var io = new CleanUpIo(dirIn, dirOut);
            var runner = new CleanUpRunner(io, log)
            {
                Filters =
                {
                    new VersionFilter(1000),
                    new NoSessionIdFilter(),
                    new NoTimeFilter(),
                    new InvalidCompletionEventFilter()
                }
            };
            runner.Run();
        }

        private static void RunSortByUser(string dirIn, string dirOut)
        {
            CleanDirs(dirOut);
            var log = new SortByUserLogger();
            var io = new IndexCreatingSortByUserIo(dirIn, dirOut, log);
            new SortByUserRunner(io, log).Run();
        }

        private static void RunStatisticsForPaperCreation()
        {
            var printer = new StatisticsPrinter();
            var io = new StatisticsIo(DirEventsCompletion_KeepNoTrigger, DirEventsAll);
            new StatisticsForPaperRunner(io, printer).Run();
        }

        private static void RunUserProfileExport()
        {
            var io = new IoHelper(DirEventsAll, DirMicroCommits);
            var helper = new UserProfileExportHelper();
            new UserProfileExportRunner(io, helper).Export(DirEventsAll);
        }

        private static void RunCompletionEventFilter(CompletionEventFilter.NoTriggerPointOption noTriggerPointOption)
        {
            const string dirIn = DirEventsAll_Clean;
            var dirOut = noTriggerPointOption == CompletionEventFilter.NoTriggerPointOption.Keep
                ? DirEventsCompletion_KeepNoTrigger
                : DirEventsCompletion_RemoveNoTrigger;

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

        private static void RunUsageExport(string dirContextsAll, string dirUsages)
        {
            CleanDirs(dirUsages);
            new UsageExportRunner(dirContextsAll, dirUsages).Run();
        }

        private static void RunCompletionEventToMicroCommit(string eventDir, string outDir)
        {
            CleanDirs(outDir);
            Console.WriteLine(@"reading from: {0}", eventDir);
            Console.WriteLine(@"writing to:   {0}", outDir);
            var usageExtractor = new UsageExtractor();
            var microCommitGenerator = new MicroCommitGenerator(usageExtractor);
            var ioHelper = new IoHelper(eventDir, outDir);
            new CompletionEventToMicroCommitRunner(microCommitGenerator, ioHelper).Run();
        }

        private static void RunEventStreamExport(string dirCtxIn, string dirEventsOut)
        {
            CleanDirs(DirEventStream);
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