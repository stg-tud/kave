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

namespace KaVE.FeedbackProcessor
{
    internal class FeedbackProcessorApp
    {
        private const int NumWorkers = 8;

        private const string Root = @"C:\Users\Sebastian\Desktop\Test\";
        //private const string Root = @"C:\Users\seb2\Desktop\interval-tests\";

        private const string DirTmp = Root + @"Tmp\";

        private const string DirEventsIn = Root + @"Events\";
        private const string DirEventsOut = Root + @"Events-Out\";

        private const string WdFolder = Root + @"watchdog\";
        private const string SvgFolder = Root + @"svg\";

        public static void Main()
        {
            var startedAt = DateTime.Now;
            Console.WriteLine(@"started at: {0}", startedAt);
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

            //new NameFixesIntegrationTest().TryToNameFixSomeNamesFromContexts();

            CleanDirs(DirTmp, DirEventsOut);
            new PreprocessingRunner(DirEventsIn, DirTmp, DirEventsOut, NumWorkers).Run();

            //var folder = "C:/Users/Andreas/Desktop/OSS-Events/test";
            //var file = "C:/Users/Andreas/Desktop/OSS-Events/target/be8f9fdb-d75e-4ec1-8b54-7b57bd47706a.zip";
            //var file = "C:/Users/Andreas/Desktop/testrunevents.zip";

            //RunWatchdogDebugging();

            //var intervals = new IntervalTransformer(Logger).TransformFolder(cleanedFolder).ToList();
            //Logger.Info(@"Found {0} intervals. Now transforming to Watchdog format ...", intervals.Count);
            //WatchdogExporter.Convert(intervals).WriteToFiles(wdFolder);


            var endedAt = DateTime.Now;
            Console.WriteLine(@"ended at: " + endedAt);
            Console.WriteLine(@"took:     " + (endedAt - startedAt));
            Console.ReadKey();
        }

        private static void RunWatchdogDebugging()
        {
            new WatchdogExportRunner().RunWatchdogDebugging(DirEventsIn, WdFolder, SvgFolder);
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