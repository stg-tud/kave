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
using System.Globalization;
using System.IO;
using System.Linq;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Intervals;
using KaVE.FeedbackProcessor.Intervals.Exporter;
using KaVE.FeedbackProcessor.Names;
using KaVE.VS.FeedbackGenerator.SessionManager.Anonymize;

namespace KaVE.FeedbackProcessor
{
    internal class FeedbackProcessorApp
    {
        private static readonly ILogger Logger = new ConsoleLogger();

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

            var file = "C:/Users/Andreas/Desktop/OSS-Events/target/be8f9fdb-d75e-4ec1-8b54-7b57bd47706a.zip";
            //var file = "C:/Users/Andreas/Desktop/testrunevents.zip";
            var intervals = new IntervalTransformer(Logger).TransformFile(file).ToList();

            Logger.Info(@"Got {0} intervals. Now transforming to Watchdog format ...", intervals.Count);

            WatchdogExporter.Convert(intervals).WriteToFiles("C:/Users/Andreas/Desktop/wd-test");

            Logger.Info("Done!");

            Console.ReadKey();
        }
    }
}