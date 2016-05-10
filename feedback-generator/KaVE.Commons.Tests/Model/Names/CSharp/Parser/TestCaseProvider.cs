using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Tests.Model.Names.CSharp.Parser
{
    class TestCaseProvider
    {
        private const string TestSourceRootFolder = @"..\..\Model\Names\CSharp\Parser\Data";
        private const string ValidTypeFile = "\\valid-typenames.tsv";
        private const string InvalidTypeFile = "\\invalid-typenames.tsv";
       
        public static string[] LoadTestFile(string file)
        {
            return File.ReadAllLines(file).Where(s => !s.Equals("") && !s.StartsWith("#") && !s.StartsWith("identifier")).ToArray();
        }

        public static IKaVEList<TypeNameTestCase> LoadTypeNameTestCase(string path)
        {
            var testcases = new KaVEList<TypeNameTestCase>();
            var lines = LoadTestFile(path);
            foreach (var line in lines)
            {
                var fields = line.Split('\t');
                var t = new TypeNameTestCase(fields[0], fields[1], fields[2]);
                testcases.Add(t);
            }
            return testcases;
        }

        public static IKaVEList<TypeNameTestCase> ValidTypeNames()
        {
            return LoadTypeNameTestCase(TestSourceRootFolder + ValidTypeFile);
        }

        internal static IKaVEList<string> InvalidTypeNames()
        {
            return KaVE.Commons.Utils.Collections.Lists.NewList(LoadTestFile(TestSourceRootFolder + InvalidTypeFile));
        }
    }
}
