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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Application;
using JetBrains.CommandLine.InspectCode;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.SolutionAnalysis;
using JetBrains.Threading;
using JetBrains.Util;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.Json;
using KaVE.Commons.Utils.Logging.Json;
using ILogger = KaVE.Commons.Utils.Exceptions.ILogger;

namespace KaVE.SolutionAnalysis
{
    [SolutionComponent]
    public class InspectCodeIntegrator : IInspectCodeConsumerFactory
    {
        private const string LogFileName = "SyntaxTrees.log";
        private const string ErrorsFileName = "SerializationErrors.log";

        private readonly ISolution _solution;
        private readonly ILogger _logger;

        public InspectCodeIntegrator(ISolution solution, ILogger logger)
        {
            _solution = solution;
            _logger = logger;
        }

        public IInspectCodeConsumer CreateConsumer(IEnumerable<IProjectModelElement> inspectScope,
            FileSystemPath outputFile = null)
        {
            var results = RunAnalysis();
            WriteResultsToFile(results);

            return new DummyInspectCodeConsumer();
        }

        private static void WriteResultsToFile(IEnumerable<Context> results)
        {
            using (var writer = new JsonLogWriter<Context>(new FileStream(LogFileName, FileMode.OpenOrCreate)))
            {
                foreach (var result in results)
                {
                    try
                    {
                        writer.Write(result);
                    }
                    catch (Exception e)
                    {
                        using (var file = File.AppendText(ErrorsFileName))
                        {
                            file.WriteLine("{{{0}}}", result.SST.EnclosingType.FullName);
                            file.Write(e.ToString());
                            file.WriteLine();
                            file.WriteLine();
                        }
                        
                    }
                }
            }
        }

        private IEnumerable<Context> RunAnalysis()
        {
            IEnumerable<Context> results = null;
            ReentrancyGuard.Current.Execute(
                "SolutionAnalysis",
                () =>
                    ReadLockCookie.Execute(
                        () =>
                            results = new SolutionAnalysis(_solution, _logger).AnalyzeAllProjects()));
            return results;
        }
    }

    public class DummyInspectCodeConsumer : IInspectCodeConsumer
    {
        public void Dispose() {}

        public void Consume(IIssue issue) {}
    }
}