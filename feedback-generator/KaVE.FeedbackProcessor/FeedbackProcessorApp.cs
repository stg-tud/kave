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
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Exceptions;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.FeedbackProcessor.CleanUp2;
using KaVE.FeedbackProcessor.CleanUp2.Filters;
using KaVE.FeedbackProcessor.Intervals;
using KaVE.FeedbackProcessor.Intervals.Exporter;
using KaVE.FeedbackProcessor.SortByUser;

namespace KaVE.FeedbackProcessor
{
    internal class FeedbackProcessorApp
    {
        private static readonly ILogger Logger = new ConsoleLogger();

        private const string Desktop = @"C:\Users\seb2\Desktop\";
        private const string InFolder = Desktop + @"interval-tests\in\";
        private const string OrderedFolder = Desktop + @"interval-tests\ordered\";
        private const string CleanedFolder = Desktop + @"interval-tests\cleaned\";
        private const string WdFolder = Desktop + @"interval-tests\watchdog\";
        private const string SvgFolder = Desktop + @"interval-tests\svg\";
        private const string EventsFolder = Desktop + @"interval-tests\events\";

        public static void Main()
        {
            //new SanityCheckApp().Run();

            //new TimeBudgetEvaluationApp(Logger).Run();
            //new SSTSequenceExtractor(Logger).Run();

            //var events = new EventStreamFilter(
            //    e =>
            //    {
            //        var se = e as SolutionEvent;
            //        return se != null && se.Action == SolutionEvent.SolutionAction.OpenSolution &&
            //               string.IsNullOrWhiteSpace(se.Target.Identifier);
            //    })
            //    .Filter("C:/Users/Andreas/Desktop/OSS-Events/target/be8f9fdb-d75e-4ec1-8b54-7b57bd47706a.zip").ToList();


            RunSortByUser(InFolder, OrderedFolder);
            RunCleanUp(OrderedFolder, CleanedFolder);

            //var folder = "C:/Users/Andreas/Desktop/OSS-Events/test";
            //var file = "C:/Users/Andreas/Desktop/OSS-Events/target/be8f9fdb-d75e-4ec1-8b54-7b57bd47706a.zip";
            //var file = "C:/Users/Andreas/Desktop/testrunevents.zip";

            RunWatchdogDebugging();

            //var intervals = new IntervalTransformer(Logger).TransformFolder(cleanedFolder).ToList();
            //Logger.Info(@"Found {0} intervals. Now transforming to Watchdog format ...", intervals.Count);
            //WatchdogExporter.Convert(intervals).WriteToFiles(wdFolder);


            Logger.Info("Done!");

            Console.ReadKey();
        }

        private static void RunWatchdogDebugging()
        {
            var svge = new SvgExport();

            CleanDirs(WdFolder, SvgFolder, EventsFolder);
            foreach (var zip in FindZips(CleanedFolder))
            {
                Logger.Info(@"Opening {0} ...", zip);
                var intervals = new IntervalTransformer(Logger).TransformFile(zip).ToList();
                Logger.Info(@"Found {0} intervals.", intervals.Count);


                var userId = intervals[0].UserId;
                var svgFile = SvgFolder + userId + ".svg";
                var eventsFile = EventsFolder + userId + ".txt";
                var wdFilesFolder = WdFolder + userId;

                Logger.Info(@"Dumping Event Stream ... ({0})", eventsFile);
                WriteEventStream(zip, eventsFile);

                Logger.Info(@"Converting to WD format ... ({0})", wdFilesFolder);
                WatchdogExporter.Convert(intervals).WriteToFiles(wdFilesFolder);

                Logger.Info(@"Now exporting .svg files for debugging ...");
                svge.Run(Lists.NewListFrom(intervals), svgFile);
            }
        }

        private static string[] FindZips(string dir)
        {
            var zips = Directory.GetFiles(dir, "*.zip", SearchOption.AllDirectories);
            return zips;
        }

        private static void WriteEventStream(string zip, string file)
        {
            var ra = new ReadingArchive(zip);
            var events = ra.GetAll<IDEEvent>();
            EventStreamExport.Write(events, file);
        }


        private static void RunSortByUser(string dirIn, string dirOut)
        {
            CleanDirs(dirOut);
            var log = new SortByUserLogger();
            var io = new IndexCreatingSortByUserIo(dirIn, dirOut, log);
            new SortByUserRunner(io, log).Run();
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
                    //new VersionFilter(1000),
                    new NoSessionIdFilter(),
                    new NoTimeFilter(),
                    new InvalidCompletionEventFilter()
                }
            };
            runner.Run();
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