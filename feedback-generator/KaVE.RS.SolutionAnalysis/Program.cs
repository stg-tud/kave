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
using System.Linq;
using Ionic.Zip;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Exceptions;
using KaVE.Commons.Utils.Json;
using KaVE.Commons.Utils.Logging.Json;
using KaVE.Commons.Utils.ObjectUsageExport;

namespace KaVE.RS.SolutionAnalysis
{
    internal class Program
    {
        private static readonly DirectoryInfo TargetDirectory =
            new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "output"));

        private static readonly UsageExtractor Exporter = new UsageExtractor();

        private static void Main(string[] args)
        {
            Console.WriteLine("{0} start", DateTime.Now);

            const string dirEvents = @"C:\Users\seb\Desktop\Events\";
            const string dirHistories = @"C:\Users\seb\Desktop\Histories\";
            const string dirContexts = @"C:\Users\seb\Desktop\Analysis\test\Contexts\";
            const string dirUsages = @"C:\Users\seb\Desktop\Analysis\all\Usages\";
            const string dirEpisodes = @"C:\Users\seb\Desktop\Episodes\";

            //new AnalysisStatsPrinter(dirContexts).Run();
            //new UsageExportRunner(dirContexts, dirUsages).Run();
            new EditLocationRunner(dirEvents).Run();
            //AnalyzeProjects();
            //var usages = ExtractUsages(dirContexts);
            //WriteUsages(usages);

            //new EventsExportRunner(dirContexts, dirEpisodes).Run();
            //new CompletionEventToUsageHistoryRunner(dirEvents, dirHistories).Run();

            Console.WriteLine("{0} finish", DateTime.Now);
        }

        private static void WriteUsages(IKaVEList<Query> usages)
        {
            var outFile = Path.Combine(TargetDirectory.FullName, "usages.zip");

            using (var outStream = new FileStream(outFile, FileMode.Create))
            {
                using (var zipFile = new ZipFile())
                {
                    zipFile.UseZip64WhenSaving = Zip64Option.AsNecessary;
                    var i = 0;
                    foreach (var u in usages)
                    {
                        var fileName = (i++) + ".json";
                        var json = u.ToFormattedJson();
                        zipFile.AddEntry(fileName, json);
                    }
                    zipFile.Save(outStream);
                }
            }
        }

        private static IKaVEList<Query> ExtractUsages(string root)
        {
            var usages = Lists.NewList<Query>();
            int contextCounter = 0;

            var logs = FindSSTLogs(root);

            foreach (var log in logs)
            {
                Console.WriteLine("##################################################");
                Console.WriteLine("reading {0}...", Path.GetFileName(log));
                var reader = new JsonLogReader<Context>(new FileStream(log, FileMode.Open), new NullLogger());

                var ctxs = reader.ReadAll().ToList();
                Console.WriteLine("\tFound {0} contexts", ctxs.Count);
                contextCounter += ctxs.Count;

                Console.Write("\tExtracting usages... ");
                int usageCounter = 0;
                foreach (var ctx in ctxs)
                {
                    var u2 = Exporter.Export(ctx);
                    //Console.Write("{0}, ", u2.Count);
                    foreach (var u in u2)
                    {
                        usages.Add(u);
                        usageCounter++;
                    }
                }
                Console.WriteLine("done\n\t--> {0} usages with calls", usageCounter);
            }
            Console.WriteLine(
                "=======\nfound {0} contexts, extracted {1} usages that contain calls",
                contextCounter,
                usages.Count);

            return usages;
        }

        private static string[] FindSSTLogs(string root)
        {
            var logs = Directory.GetFiles(root, "*_contexts.zip", SearchOption.TopDirectoryOnly);
            Console.WriteLine("available logs:");
            foreach (var log in logs)
            {
                Console.WriteLine("\t* {0}", log);
            }
            return logs;
        }

        private static void AnalyzeProjects()
        {
            //if (!TargetDirectory.Exists)
            //{
            //    TargetDirectory.Create();
            //}

            //var repo = new Uri("https://github.com/restsharp/RestSharp.git");

            //Console.WriteLine("Cloning {0} to {1} ...", repo, TargetDirectory);

            var repoRoot = TargetDirectory; //CloneRepository(repo, TargetDirectory.FullName);

            //Console.WriteLine("Finished cloning {0}!", repo);

            var slnFiles = FindSolutionFiles(repoRoot.FullName).ToList();

            Console.WriteLine("Found solution files:");
            slnFiles.ForEach(f => Console.WriteLine("- {0}", f.Name));

            foreach (var slnFile in slnFiles)
            {
                if (SolutionIsAlreadyAnalysed(slnFile))
                {
                    Console.WriteLine("Solution {0} is already analysed. Skipping ...", slnFile.Name);
                    continue;
                }

                Console.WriteLine("Now analysing {0} ...", slnFile.Name);
                try
                {
                    var syntaxTrees = GenerateSyntaxTrees(slnFile);

                    File.WriteAllText(
                        Path.Combine(TargetDirectory.FullName, slnFile.Name + "_Analysis.log"),
                        syntaxTrees.AnalysisLog);

                    if (syntaxTrees.SyntaxTrees != null)
                    {
                        File.WriteAllLines(
                            Path.Combine(TargetDirectory.FullName, slnFile.Name + "_SyntaxTrees.log"),
                            syntaxTrees.SyntaxTrees);
                    }

                    if (syntaxTrees.SerializationErrorLog != null)
                    {
                        File.WriteAllText(
                            Path.Combine(TargetDirectory.FullName, slnFile.Name + "_FailedSerializations.log"),
                            syntaxTrees.SerializationErrorLog);
                    }
                    Console.WriteLine("Finished analysing {0}!", slnFile);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Analysis failed: {0}", e);
                }
            }

            Console.WriteLine("All done!");
            Console.ReadKey();
        }

        private static bool SolutionIsAlreadyAnalysed(FileInfo slnFile)
        {
            return File.Exists(Path.Combine(TargetDirectory.FullName, slnFile.Name + "_Analysis.log"));
        }

        private static DirectoryInfo CloneRepository(Uri uri, string location)
        {
            var folderName = uri.Segments.Last();
            var git = new Process
            {
                StartInfo =
                {
                    FileName = "git.exe",
                    Arguments = String.Format("clone {0} --depth=1 {1}", uri, folderName),
                    WorkingDirectory = location,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            //git.Start();
            //git.WaitForExit();

            var slnRoot = new DirectoryInfo(Path.Combine(location, folderName));
            return slnRoot;
        }

        private static IEnumerable<FileInfo> FindSolutionFiles(string searchRoot)
        {
            return
                Directory.GetFiles(searchRoot, "*.sln", SearchOption.AllDirectories)
                         .Select(fileName => new FileInfo(fileName));
        }

        private static DirectoryInfo CreateTemporaryDirectory()
        {
            string tempPath = Path.GetTempPath();
            string tempDir;
            do
            {
                tempDir = Path.Combine(tempPath, Path.GetRandomFileName());
            } while (Directory.Exists(tempDir));
            return Directory.CreateDirectory(tempDir);
        }

        private class SyntaxTreeGeneratorResult
        {
            public IEnumerable<string> SyntaxTrees { get; set; }
            public string AnalysisLog { get; set; }
            public string SerializationErrorLog { get; set; }
        }

        private static SyntaxTreeGeneratorResult GenerateSyntaxTrees(FileInfo solutionFile)
        {
            var solutionAnalysisPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "KaVE.RS.SolutionAnalysis.dll");
            Asserts.That(
                File.Exists(solutionAnalysisPath),
                String.Format("KaVE solution analyser not found in {0}.", solutionAnalysisPath));

            var workingDirectory = CreateTemporaryDirectory();

            var cmd = String.Format(
                "/c inspectcode.exe \"{0}\" /o=tmp.xml /plugin=\"{1}\" > Analysis.log 2>&1",
                solutionFile,
                solutionAnalysisPath);
            var inspectCode = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    Arguments = cmd,
                    WorkingDirectory = workingDirectory.FullName,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            inspectCode.Start();
            inspectCode.WaitForExit();

            var result = new SyntaxTreeGeneratorResult();

            var resultsFile = new FileInfo(Path.Combine(workingDirectory.FullName, "SyntaxTrees.log"));
            var errorsFile = new FileInfo(Path.Combine(workingDirectory.FullName, "SerializationErrors.log"));
            var analysisLogFile = new FileInfo(Path.Combine(workingDirectory.FullName, "Analysis.log"));

            if (resultsFile.Exists)
            {
                result.SyntaxTrees = File.ReadAllLines(resultsFile.FullName);
            }

            if (errorsFile.Exists)
            {
                result.SerializationErrorLog = File.ReadAllText(errorsFile.FullName);
            }

            if (analysisLogFile.Exists)
            {
                result.AnalysisLog = File.ReadAllText(analysisLogFile.FullName);
            }

            workingDirectory.Delete(true);

            return result;
        }
    }
}