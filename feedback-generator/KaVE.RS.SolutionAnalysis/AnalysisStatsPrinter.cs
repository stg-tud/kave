using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Json;
using KaVE.Commons.Utils.ObjectUsageExport;

namespace KaVE.RS.SolutionAnalysis
{
    internal class AnalysisStatsPrinter
    {
        private readonly string _root;

        public AnalysisStatsPrinter(string root)
        {
            _root = root;
        }

        public void Run()
        {
            var zips = FindContextZipFiles();
            Console.WriteLine(@"found {0} zips:", zips.Count);
            foreach (var zip in zips)
            {
                Console.WriteLine(@"- {0}", zip);
            }
            Console.WriteLine();

            var allCtxs = new List<Context>();
            var totalCtxs = 0;
            var totalCtxsWithMethods = 0;

            foreach (var zip in zips)
            {
                Console.WriteLine(@"# {0}", zip);
                var ctxs = ReadContexts(zip);
                var numCtxs = ctxs.Count;
                Console.WriteLine("\t-found {0} contexts", numCtxs);
                var numCtxsWithMethods = ctxs.Where(c => c.SST.Methods.Count > 0).ToList().Count;
                Console.WriteLine("\t-{0} have method declarationsc", numCtxsWithMethods);

                allCtxs.AddRange(ctxs);
                totalCtxs += numCtxs;
                totalCtxsWithMethods += numCtxsWithMethods;
            }

            Console.WriteLine(@"# total: {0} contexts, {1} with method declarations", totalCtxs, totalCtxsWithMethods);

            CountAndReportUsages(allCtxs);
        }

        private int _totalNumUsages;
        private int _totalNumUsagesWith;
        private readonly UsageExtractor _extractor = new UsageExtractor();
        private readonly Dictionary<CoReTypeName, int> _typeCounts = new Dictionary<CoReTypeName, int>();
        private readonly Dictionary<CoReTypeName, int> _typeCountsWith = new Dictionary<CoReTypeName, int>();

        private void CountAndReportUsages(List<Context> ctxs)
        {
            foreach (var ctx in ctxs)
            {
                var usages = _extractor.Export(ctx);

                foreach (var u in usages)
                {
                    _totalNumUsages++;
                    Count(u.type, _typeCounts);
                    if (u.HasReceiverCallSites)
                    {
                        _totalNumUsagesWith++;
                        Count(u.type, _typeCountsWith);
                    }
                }
            }

            Console.WriteLine(
                "extracted {0} usages ({1} have receiver call sites)",
                _totalNumUsages,
                _totalNumUsagesWith);

            Console.WriteLine("\ntop types in 'counts':");
            PrintTopEntries(_typeCounts, 40);
            Console.WriteLine("\ntop types in 'countsWith':");
            PrintTopEntries(_typeCountsWith, 40);
        }

        private static void PrintTopEntries(Dictionary<CoReTypeName, int> counts, int numTake)
        {
            var myList = counts.ToList();
            myList.Sort(CompareEntry);
            foreach (var entry in myList.Take(numTake))
            {
                Console.WriteLine("{0}: {1}", entry.Value, entry.Key);
            }
        }

        private static int CompareEntry(KeyValuePair<CoReTypeName, int> x, KeyValuePair<CoReTypeName, int> y)
        {
            return y.Value.CompareTo(x.Value);
        }

        private void Count(CoReTypeName type, Dictionary<CoReTypeName, int> dictionary)
        {
            if (dictionary.ContainsKey(type))
            {
                dictionary[type] += 1;
            }
            else
            {
                dictionary[type] = 0;
            }
        }

        private static IList<Context> ReadContexts(string zip)
        {
            var ctxs = new List<Context>();
            var tmp = GetTemporaryDirectory();
            try
            {
                try
                {
                    using (var zipFile = ZipFile.Read(zip))
                    {
                        zipFile.ExtractAll(tmp);

                        foreach (var f in Directory.EnumerateFileSystemEntries(tmp, "*.json"))
                        {
                            var json = File.ReadAllText(f);
                            ctxs.Add(json.ParseJsonTo<Context>());
                        }
                    }
                }
                catch (ZipException e)
                {
                    Console.WriteLine(@"[ERROR] {0}", e);
                }
            }
            finally
            {
                Directory.Delete(tmp, true);
            }
            return ctxs;
        }


        public IList<string> FindContextZipFiles()
        {
            return Directory.GetFiles(_root, "*-contexts.zip", SearchOption.AllDirectories);
        }

        public static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}