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
using System.Diagnostics;
using System.IO;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.TestUtils;
using KaVE.Commons.TestUtils.Utils;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.IO.Archives;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests
{
    internal class BulkAnalysisTest : BaseTestWithExistingSolution
    {
        private const string Root = @"F:\";
        private const string RepositoryRoot = Root + @"R\";
        private const string ContextRoot = Root + @"Contexts\";
        private const string LogRoot = Root + @"Logs\";

        private static SolutionFinder SlnFinder;

        private string _currentSolution;
        private string _currentSolutionPath;

        private string _logName;
        private string _zipName;
        private TestRunnerLogger _logger;

        protected override FileSystemPath ExistingSolutionFilePath
        {
            get { return FileSystemPath.Parse(_currentSolutionPath); }
        }

        public static IEnumerable<string[]> FindSolutionFiles()
        {
            // set field here to prevent initialization error when path is not found
            if (SlnFinder == null)
            {
                SlnFinder = new SolutionFinder(RepositoryRoot);
            }
            return SlnFinder.GetTestData();
        }

        //[TestCaseSource("FindSolutionFiles")]
        public void AnalyzeSolution(string testCaseLabel, string sln)
        {
            PrintFreeMemoryAndCheckConsumption();

            if (SlnFinder.ShouldIgnore(sln))
            {
                Assert.Ignore();
            }

            _currentSolution = sln;
            _currentSolutionPath = SlnFinder.GetFullPath(_currentSolution);
            SlnFinder.Start(sln);

            Console.WriteLine("Opening solution: {0} ({1})\n", ExistingSolutionFilePath, DateTime.Now);
            Console.WriteLine("Log: {0}", _logName = GetLogName(sln));
            Console.WriteLine("Contexts: {0}\n", _zipName = GetZipName(sln));
            Console.WriteLine("copy&paste: {0}\n", sln.Replace(@"\", "/"));

            _logger = new TestRunnerLogger(_logName);

            DoTestSolution(ExistingSolutionFilePath, RunAnalysis);

            if (_logger.HasError)
            {
                SlnFinder.Crash(sln);
                Assert.Fail("execution produced at least one error, see error log for details\n");
            }
            else
            {
                SlnFinder.End(sln);
            }
        }

        private static void PrintFreeMemoryAndCheckConsumption()
        {
            using (var proc = Process.GetCurrentProcess())
            {
                var sizeInByte = proc.VirtualMemorySize64;
                var sizeInMB = sizeInByte/(1024.0*1024.0);

                Console.WriteLine("Current memory consumption: {0:#,0.00}MB (VirtualMemorySize64)", sizeInMB);
                if (sizeInMB > 1700)
                {
                    Assert.Fail(
                        "analysis aborted, available memory is too low (VirtualMemorySize64 is at {0}MB)",
                        sizeInMB);
                }
            }
        }

        private void RunAnalysis(Lifetime lifetime, ISolution solution)
        {
            Console.WriteLine("Starting analysis... ({0})", DateTime.Now);

            using (var wa = new WritingArchive(_zipName))
            {
                var countWithMethods = 0;
                Action<Context> cbContext = ctx =>
                {
                    _logger.Info("\t> {0}".FormatEx(ctx.SST.EnclosingType));
                    // ReSharper disable once AccessToDisposedClosure
                    wa.Add(ctx);
                    if (ctx.SST.Methods.Count > 0)
                    {
                        countWithMethods++;
                    }
                };

                new ContextSolutionAnalysis(solution, _logger, cbContext).AnalyzeAllProjects();

                _logger.EndPossibleErrorBlock();

                Console.WriteLine("Analysis finished! ({0})", DateTime.Now);
                Console.WriteLine(
                    "found {0} context(s), {1} contain(s) method declarations",
                    wa.NumItemsAdded,
                    countWithMethods);
            }
        }

        private string GetLogName(string relativeSolutionPath)
        {
            var fileName = LogRoot + relativeSolutionPath + "-log.txt";
            EnsureFolderExists(fileName);
            return fileName;
        }

        private static string GetZipName(string relativeSolutionPath)
        {
            var fileName = ContextRoot + relativeSolutionPath + "-contexts.zip";
            EnsureFolderExists(fileName);
            return fileName;
        }

        private static void EnsureFolderExists(string logName)
        {
            var dir = Path.GetDirectoryName(logName);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}