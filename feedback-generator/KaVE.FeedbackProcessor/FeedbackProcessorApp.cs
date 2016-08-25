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
using System.Threading;
using KaVE.FeedbackProcessor.Naming;
using KaVE.FeedbackProcessor.Preprocessing;

namespace KaVE.FeedbackProcessor
{
    internal class FeedbackProcessorApp
    {
        private const int NumWorkers = 2;

        //private const string Root = @"C:\Users\Sebastian\Desktop\Test\";
        private const string Root = @"C:\Users\seb2\Desktop\interval-tests\";

        private const string DirTmp = Root + @"Tmp\";

        private const string DirEventsIn = Root + @"Events\";
        private const string DirEventsOut = Root + @"Events-Out\";

        private const string WdFolder = Root + @"watchdog\";
        private const string SvgFolder = Root + @"svg\";

        private const string DirContexts = @"E:\Contexts\";

        public static void Main()
        {
            var startedAt = DateTime.Now;
            Console.WriteLine(@"started at: {0}", startedAt);

            CleanDirs(DirTmp, DirEventsOut, WdFolder, SvgFolder);

            //new SanityCheckApp().Run();
            //new TimeBudgetEvaluationApp(Logger).Run();
            //new SSTSequenceExtractor(Logger).Run();
            //RunExhaustiveNamesFixTests();
            RunPreprocessing();
            RunWatchdogDebugging();

            var endedAt = DateTime.Now;
            Console.WriteLine(@"ended at {0}, took {1}", endedAt, (endedAt - startedAt));
            Console.ReadKey();
        }

        private static void RunExhaustiveNamesFixTests()
        {
            new NameFixesIntegrationTest(NumWorkers, DirContexts).TryToNameFixSomeNamesFromContexts();
        }

        private static void RunPreprocessing()
        {
            new PreprocessingRunner(DirEventsIn, DirTmp, DirEventsOut, NumWorkers).Run();
        }

        private static void RunWatchdogDebugging()
        {
            new WatchdogExportRunner().RunWatchdogDebugging(DirEventsOut, WdFolder, SvgFolder);
        }

        private static void CleanDirs(params string[] dirs)
        {
            foreach (var dir in dirs)
            {
                Console.WriteLine(@"cleaning: {0}", dir);
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                    while (Directory.Exists(dir))
                    {
                        Thread.Sleep(250);
                    }
                }
                Directory.CreateDirectory(dir);
            }
        }
    }
}