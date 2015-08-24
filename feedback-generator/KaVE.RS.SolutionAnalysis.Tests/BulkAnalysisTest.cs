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
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.IO.Archives;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests
{
    internal class BulkAnalysisTest : BaseTestWithExistingSolution
    {
        private const string Root = @"C:\Users\seb\Desktop\Data\";
        //private const string RepositoryRoot = Root + @"Repositories-Test\";
        private const string RepositoryRoot = @"C:\Users\seb\versioned_code\AnalysisTestCases\";
        //private const string RepositoryRoot = Root + @"Repositories\";
        private const string ContextRoot = Root + @"Contexts\Github\";
        private const string LogRoot = Root + @"Logs\";

        private string _currentSolution;

        protected override FileSystemPath ExistingSolutionFilePath
        {
            get { return FileSystemPath.Parse(RepositoryRoot + _currentSolution); }
        }

        public static IEnumerable<string> FindSolutionFiles()
        {
            var all = Directory.GetFiles(RepositoryRoot, "*.sln", SearchOption.AllDirectories);
            var filtered = all.Where(sln => !sln.Contains(@"\test\data\"));
            var shortened = filtered.Select(sln => sln.Substring(RepositoryRoot.Length));
            return shortened;
        }

        private string _logName;
        private string _zipName;
        private TestRunnerLogger _logger;

        [TestCaseSource("FindSolutionFiles")]
        public void AnalyzeSolution(string shortenedSolution)
        {
            _currentSolution = shortenedSolution;
            _logName = GetLogName(_currentSolution);
            _zipName = GetZipName(_currentSolution);

            Console.WriteLine("Opening solution: {0}\n", ExistingSolutionFilePath);
            Console.WriteLine("Log: {0}", _logName);
            Console.WriteLine("Contexts: {0}\n", _zipName);

            _logger = new TestRunnerLogger(_logName);

            DoTestSolution(ExistingSolutionFilePath, RunAnalysis);

            _logger.AssertNoError();
        }

        private void RunAnalysis(Lifetime lifetime, ISolution solution)
        {
            Console.WriteLine("Starting analysis...");
            var solutionAnalysis = new SolutionAnalysis(solution, _logger);

            var ctxs = solutionAnalysis.AnalyzeAllProjects();

            _logger.EndPossibleErrorBlock();

            using (var wa = new WritingArchive(_zipName))
            {
                wa.AddAll(ctxs);
            }
            Console.WriteLine("Analysis finished!");
            var count = ctxs.Count;
            var countWithMethods = ctxs.Where(c => c.SST.Methods.Count > 0).ToList().Count;
            Console.WriteLine(
                "found {0} context(s), {1} contain(s) method declarations",
                count,
                countWithMethods);
        }

        private static string GetZipName(string relativeSolutionPath)
        {
            var fileName = ContextRoot + relativeSolutionPath + "-contexts.zip";
            EnsureFolderExists(fileName);
            return fileName;
        }

        private static string GetLogName(string relativeSolutionPath)
        {
            var fileName = LogRoot + relativeSolutionPath + "-log.txt";
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