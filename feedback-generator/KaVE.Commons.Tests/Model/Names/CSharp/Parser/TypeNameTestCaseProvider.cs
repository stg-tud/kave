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
using System.Text;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Tests.Model.Names.CSharp.Parser
{
    class TypeNameTestCaseProvider
    {
        private const string TestSourceRootFolder = @"..\..\Model\Names\CSharp\Parser\Data";
        private const string ValidTypeFile = "\\valid-typenames.tsv";
        private const string InvalidTypeFile = "\\invalid-typenames.tsv";
        private const string ValidMethodFile = "\\valid-methodnames.tsv";
        private const string InvalidMethodFile = "\\invalid-typenames.tsv";

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
                var t = new TypeNameTestCase()
                {
                    Identifier = fields[0],
                    Namespace = fields[1],
                    Assembly = fields[2]
                };
                testcases.Add(t);
            }
            return testcases;
        }

        public static IKaVEList<MethodNameTestCase> LoadMethodNameTestCases(string path)
        {
            var testcases = new KaVEList<MethodNameTestCase>();
            var lines = LoadTestFile(path);
            foreach (var line in lines)
            {
                var fields = line.Split('\t');
                var t = new MethodNameTestCase()
                {
                    Identifier = fields[0],
                    DeclaringType = fields[1],
                    ReturnType = fields[2],
                    SimpleName = fields[3],
                    IsStatic = GetBoolean(fields[4]),
                    IsGeneric = GetBoolean(fields[5]),
                    Parameters = GetList(fields[7]),
                    TypeParameters = GetList(fields[8])

                };
                testcases.Add(t);
            }
            return testcases;
        }

        private static Boolean GetBoolean(string s)
        {
            return s.Equals("t");
        }

        private static IKaVEList<string> GetList(string s)
        {
            return s.Equals("") ? Lists.NewList<string>() : Lists.NewList(s.Split(';'));
        }


        public static IKaVEList<TypeNameTestCase> ValidTypeNames()
        {
            return LoadTypeNameTestCase(TestSourceRootFolder + ValidTypeFile);
        }

        internal static IKaVEList<string> InvalidTypeNames()
        {
            return KaVE.Commons.Utils.Collections.Lists.NewList(LoadTestFile(TestSourceRootFolder + InvalidTypeFile));
        }

        public static IKaVEList<MethodNameTestCase> ValidMethodNames()
        {
            return LoadMethodNameTestCases(TestSourceRootFolder + ValidMethodFile);
        }

        public static IKaVEList<string> InvalidMethodNames()
        {
            return KaVE.Commons.Utils.Collections.Lists.NewList(LoadTestFile(TestSourceRootFolder + InvalidMethodFile));
        }
    }
}
