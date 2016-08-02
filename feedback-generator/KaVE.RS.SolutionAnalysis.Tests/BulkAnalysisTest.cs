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
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests
{
    internal class BulkAnalysisTest : BaseTestWithExistingSolution
    {
        private const string Root = @"C:\Data\";
        //private const string RepositoryRoot = Root + @"Repositories-Test\";
        //private const string RepositoryRoot = @"C:\Users\seb\versioned_code\AnalysisTestCases\";
        private const string RepositoryRoot = Root + @"Repositories\";
        private const string ContextRoot = Root + @"Contexts\";
        private const string LogRoot = Root + @"Logs\";

        private string _currentSolution;

        protected override FileSystemPath ExistingSolutionFilePath
        {
            get { return FileSystemPath.Parse(RepositoryRoot + _currentSolution); }
        }

        public static IEnumerable<string> FindSolutionFiles()
        {
            var shortened = ReadOrCreateIndex();
            var notAnalyzed = shortened.Where(s => !IsAlreadyAnalyzed(s));
            var notMarked = notAnalyzed.Where(s => !IsAlreadyStarted(s));
            var notCrashed = notMarked.Where(s => !IsCrashed(s));
            return notCrashed;
        }

        private static IEnumerable<string> ReadOrCreateIndex()
        {
            try
            {
                var indexFile = Path.Combine(RepositoryRoot, "index.json");
                if (File.Exists(indexFile))
                {
                    Console.WriteLine("Reading index... {0}", DateTime.Now);
                    var json = File.ReadAllText(indexFile);
                    return json.ParseJsonTo<IEnumerable<string>>();
                }

                Console.WriteLine("Finding solutions... {0}", DateTime.Now);
                var all = Directory.GetFiles(RepositoryRoot, "*.sln", SearchOption.AllDirectories);
                var filtered = all.Where(sln => !sln.Contains(@"\test\data\"));
                var shortened = filtered.Select(sln => sln.Substring(RepositoryRoot.Length));

                Console.WriteLine("Creating index... {0}", DateTime.Now);
                File.WriteAllText(indexFile, shortened.ToCompactJson());
                return shortened;
            }
            catch (IOException e)
            {
                return new string[0];
            }
        }

        private static bool IsCrashed(string shortenedSolution)
        {
            var marker = GetCrashMarkerName(shortenedSolution);
            var isCrashed = File.Exists(marker);
            return isCrashed;
        }

        // TODO ad hoc fix, write test!
        private static bool IsAlreadyAnalyzed(string shortenedSolution)
        {
            var ctxZipName = GetZipName(shortenedSolution);
            var isAlreadyAnalyzed = File.Exists(ctxZipName);
            return isAlreadyAnalyzed;
        }

        // TODO ad hoc fix, write test!
        private static bool IsAlreadyStarted(string shortenedSolution)
        {
            var ctxZipName = GetMarkerName(shortenedSolution, StartedMarker);
            var isAlreadyMarked = File.Exists(ctxZipName);
            return isAlreadyMarked;
        }

        private string _logName;
        private string _zipName;
        private TestRunnerLogger _logger;
        private const string CrashMarker = ".crashed";
        private const string StartedMarker = ".started";
        private const string EndMarker = ".ended";

        //[TestCaseSource("FindSolutionFiles")]
        public void AnalyzeSolution(string shortenedSolution)
        {
            _currentSolution = shortenedSolution;
            _logName = GetLogName(_currentSolution);
            _zipName = GetZipName(_currentSolution);

            CreateMarker(shortenedSolution, StartedMarker);
            File.Create(GetCrashMarkerName(shortenedSolution)).Close();

            Console.WriteLine("Opening solution: {0} ({1})\n", ExistingSolutionFilePath, DateTime.Now);
            Console.WriteLine("Log: {0}", _logName);
            Console.WriteLine("Contexts: {0}\n", _zipName);

            _logger = new TestRunnerLogger(_logName);

            DoTestSolution(ExistingSolutionFilePath, RunAnalysis);

            CreateMarker(shortenedSolution, EndMarker);
            File.Delete(GetCrashMarkerName(shortenedSolution));
            _logger.AssertNoError();
        }

        private static void CreateMarker(string shortenedSolution, string marker)
        {
            var name = GetMarkerName(shortenedSolution, marker);
            File.Create(name).Close();
        }

        private void RunAnalysis(Lifetime lifetime, ISolution solution)
        {
            Console.WriteLine("Starting analysis... ({0})", DateTime.Now);

            IWritingArchive wa = null;
            try
            {
                wa = new WritingArchive(_zipName);
                var countWithMethods = 0;
                Action<Context> cbContext = ctx =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    wa.Add(ctx);
                    if (ctx.SST.Methods.Count > 0)
                    {
                        countWithMethods++;
                    }
                };

                new SolutionAnalysis(solution, _logger, cbContext).AnalyzeAllProjects();

                _logger.EndPossibleErrorBlock();

                Console.WriteLine("Analysis finished! ({0})", DateTime.Now);
                var count = wa.NumItemsAdded;
                Console.WriteLine(
                    "found {0} context(s), {1} contain(s) method declarations",
                    count,
                    countWithMethods);
            }
            finally
            {
                if (wa != null)
                {
                    wa.Dispose();
                }
            }
        }

        private static string GetZipName(string relativeSolutionPath)
        {
            var fileName = ContextRoot + relativeSolutionPath + "-contexts.zip";
            EnsureFolderExists(fileName);
            return fileName;
        }

        private static string GetMarkerName(string relativeSolutionPath, string marker)
        {
            var fileName = ContextRoot + relativeSolutionPath + marker;
            EnsureFolderExists(fileName);
            return fileName;
        }

        private static string GetCrashMarkerName(string relativeSolutionPath)
        {
            var fileName = RepositoryRoot + relativeSolutionPath + CrashMarker;
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