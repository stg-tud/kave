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
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.TestUtils;
using KaVE.Commons.TestUtils.Utils;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.Commons.Utils.TypeShapeIndex;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests
{
    internal class BulkTypeShapeAnalysisTest : BaseTestWithExistingSolution
    {
        private const string Root = @"D:\Analysis\";
        private const string RepositoryRoot = Root + @"Repositories\";
        private const string TypeShapeRoot = Root + @"TypeShapes\";
        private const string LogRoot = Root + @"Logs\";

        private static SolutionFinder _slnFinder;

        private string _currentSolution;
        private string _currentSolutionPath;

        private Dictionary<IAssemblyName, WritingArchive> _writingArchives;

        private string _logName;
        private TestRunnerLogger _logger;

        protected override FileSystemPath ExistingSolutionFilePath
        {
            get { return FileSystemPath.Parse(_currentSolutionPath); }
        }

        public static IEnumerable<string[]> FindSolutionFiles()
        {
            // set field here to prevent initialization error when path is not found
            if (_slnFinder == null)
            {
                _slnFinder = new SolutionFinder(RepositoryRoot);
            }
            return _slnFinder.GetTestData();
        }

        //[TestCaseSource("FindSolutionFiles")]
        public void AnalyzeSolution(string testCaseLabel, string sln)
        {
            if (_slnFinder.ShouldIgnore(sln))
            {
                Assert.Ignore();
            }

            _currentSolution = sln;
            _currentSolutionPath = _slnFinder.GetFullPath(_currentSolution);
            _slnFinder.Start(sln);

            Console.WriteLine("Opening solution: {0} ({1})\n", ExistingSolutionFilePath, DateTime.Now);
            Console.WriteLine("Log: {0}", _logName = GetLogName(sln));
            Console.WriteLine("copy&paste: {0}\n", sln.Replace(@"\", "/"));

            _logger = new TestRunnerLogger(_logName);

            _writingArchives = new Dictionary<IAssemblyName, WritingArchive>();

            DoTestSolution(ExistingSolutionFilePath, RunAnalysis);

            if (_logger.HasError)
            {
                _slnFinder.Crash(sln);
                Assert.Fail("execution produced at least one error, see error log for details\n");
            }
            else
            {
                _slnFinder.End(sln);
            }
        }

        private void RunAnalysis(Lifetime lifetime, ISolution solution)
        {
            Console.WriteLine("Starting analysis... ({0})", DateTime.Now);

            Func<TypeShape, bool> cbTypeShape = tS =>
            {
                var assemblyName = tS.TypeHierarchy.Element.Assembly;
                if (AssemblyAlreadyExists(assemblyName))
                {
                    return true;
                }
                _logger.Info("\t> {0}".FormatEx(tS.TypeHierarchy.Element));
                if (_writingArchives.ContainsKey(assemblyName))
                {
                    _writingArchives[assemblyName].Add(tS);
                }
                else
                {
                    _writingArchives.Add(assemblyName, new WritingArchive(GetZipName(assemblyName)));
                }
                return false;
            };

            new TypeShapeSolutionAnalysis(lifetime, solution, _logger, cbTypeShape).AnalyzeAllProjects();

            foreach (var writingArchive in _writingArchives)
            {
                writingArchive.Value.Dispose();
            }

            _logger.EndPossibleErrorBlock();

            Console.WriteLine("Analysis finished! ({0})", DateTime.Now);
            Console.WriteLine(
                "Generated {0} Assembly Zips",
                _writingArchives.Count);
        }

        private bool AssemblyAlreadyExists(IAssemblyName assemblyName)
        {
            var zipName = GetZipName(assemblyName);
            return File.Exists(zipName) && !_writingArchives.ContainsKey(assemblyName);
        }

        private string GetLogName(string relativeSolutionPath)
        {
            var fileName = LogRoot + relativeSolutionPath + "-log.txt";
            EnsureFolderExists(fileName);
            return fileName;
        }

        private static string GetZipName(IAssemblyName assemblyName)
        {
            var assemblyFileName = TypeShapeIndexUtil.GetAssemblyFileName(assemblyName);
            var fileName = TypeShapeRoot + "\\" + assemblyFileName + ".zip";
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