using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.Exceptions;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests
{
    internal class BulkAnalysisTest : BaseTestWithExistingSolution
    {
        private const string Qualifier = @"test\";
        private const string Root = @"C:\Users\seb\Desktop\Analysis\" + Qualifier;
        //private const string RepositoryRoot = Root + @"Repositories-Test\";
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

            ReportAnalysisStart(_logName, _zipName);

            _logger = TestRunnerLogger.Create(_logName);

            DoTestSolution(ExistingSolutionFilePath, RunAnalysis);

            _logger.AssertNoError();
        }

        private void RunAnalysis(Lifetime lifetime, ISolution solution)
        {
            Console.WriteLine("Starting analysis...");
            var solutionAnalysis = new SolutionAnalysis(solution, _logger);

            var ctxs = solutionAnalysis.AnalyzeAllProjects();

            _logger.EndPossibleErrorBlock();

            Write(ctxs, _zipName);
            ReportAnalysisFinished(ctxs);
        }

        private void ReportAnalysisStart(string logName, string zipName)
        {
            Console.WriteLine("Opening solution: {0}\n", ExistingSolutionFilePath);
            Console.WriteLine("Log: {0}", logName);
            Console.WriteLine("Contexts: {0}\n", zipName);
        }

        private void Write(IList<Context> ctxs, string outFile)
        {
            if (ctxs.Count == 0)
            {
                Console.WriteLine("no contexts, skipping zip creation");
                return;
            }

            using (var outStream = new FileStream(outFile, FileMode.Create))
            {
                using (var zipFile = new ZipFile())
                {
                    zipFile.UseZip64WhenSaving = Zip64Option.AsNecessary;
                    var i = 0;
                    foreach (var u in ctxs)
                    {
                        var fileName = (i++) + ".json";
                        var json = u.ToFormattedJson();
                        zipFile.AddEntry(fileName, json);
                    }
                    zipFile.Save(outStream);
                }
            }
        }

        private static void ReportAnalysisFinished(IList<Context> ctxs)
        {
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

    internal class TestRunnerLogger : FileLogger
    {
        private readonly string _logName;
        private bool _hasError;

        private TestRunnerLogger(string logName) : base(logName)
        {
            _logName = logName;
        }

        public override void Error(Exception exception, string content, params object[] args)
        {
            _hasError = true;
            WriteSeparator();
            Console.WriteLine(content, args);
            Console.WriteLine(exception);
            base.Error(exception, content, args);
        }

        private static void WriteSeparator()
        {
            Console.WriteLine(@"==========================================================================");
        }

        public override void Error(Exception exception)
        {
            _hasError = true;
            WriteSeparator();
            Console.WriteLine(exception);
            base.Error(exception);
        }

        public override void Error(string content, params object[] args)
        {
            _hasError = true;
            WriteSeparator();
            Console.WriteLine(content, args);
            base.Error(content, args);
        }

        public static TestRunnerLogger Create(string logName)
        {
            File.Delete(logName);
            return new TestRunnerLogger(logName);
        }

        public void EndPossibleErrorBlock()
        {
            if (_hasError)
            {
                WriteSeparator();
            }
        }

        public void AssertNoError()
        {
            if (_hasError)
            {
                Assert.Fail("execution produced at least one error, see log for more details: {0}\n", _logName);
            }
        }
    }
}