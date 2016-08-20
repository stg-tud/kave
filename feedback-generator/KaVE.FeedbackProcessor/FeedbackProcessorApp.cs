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
using KaVE.FeedbackProcessor.Preprocessing;
using KaVE.FeedbackProcessor.Preprocessing.Filters;
using KaVE.FeedbackProcessor.Preprocessing.Logging;

namespace KaVE.FeedbackProcessor
{
    internal class FeedbackProcessorApp
    {
        private const string TmpDir = Desktop + @"interval-tests\tmp\";

        private const string Desktop = @"C:\Users\seb2\Desktop\";
        private const string InDir = Desktop + @"interval-tests\in\";
        private const string OutDir = Desktop + @"interval-tests\out\";
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

            CleanDirs(TmpDir, OutDir);
            RunPreprocessing(InDir, TmpDir, OutDir);

            //var folder = "C:/Users/Andreas/Desktop/OSS-Events/test";
            //var file = "C:/Users/Andreas/Desktop/OSS-Events/target/be8f9fdb-d75e-4ec1-8b54-7b57bd47706a.zip";
            //var file = "C:/Users/Andreas/Desktop/testrunevents.zip";

            RunWatchdogDebugging();

            //var intervals = new IntervalTransformer(Logger).TransformFolder(cleanedFolder).ToList();
            //Logger.Info(@"Found {0} intervals. Now transforming to Watchdog format ...", intervals.Count);
            //WatchdogExporter.Convert(intervals).WriteToFiles(wdFolder);


            Console.ReadKey();
        }

        private static void RunWatchdogDebugging()
        {
            new WatchdogExportRunner().RunWatchdogDebugging(InDir, WdFolder, SvgFolder);
        }

        private static void RunPreprocessing(string dirIn, string dirTmp, string dirOut)
        {
            var io = new PreprocessingIo(dirIn, dirTmp, dirOut);

            new Runner(
                io,
                new RunnerLogger(new ConsoleLogger()),
                3,
                CreateIdReader,
                new Grouper(),
                CreateZipMerger,
                CreateZipCleaner).Run();
        }

        private static IdReader CreateIdReader(IPreprocessingIo io, IPrepocessingLogger log)
        {
            throw new NotImplementedException();
        }

        private static GroupMerger CreateZipMerger(IPreprocessingIo io, IPrepocessingLogger log)
        {
            throw new NotImplementedException();
        }

        private static Cleaner CreateZipCleaner(IPreprocessingIo io, IPrepocessingLogger log)
        {
            return new Cleaner(io, new CleanerLogger(log))
            {
                Filters =
                {
                    //new VersionFilter(1000),
                    new NoSessionIdFilter(),
                    new NoTimeFilter(),
                    new InvalidCompletionEventFilter()
                }
            };
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