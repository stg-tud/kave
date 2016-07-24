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
using System.IO;
using System.Linq;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v1.Parser
{
    class TestCaseProvider
    {
        private const string TestSourceRootFolder = @"..\..\Model\Names\CSharp\Parser\Data";
        private const string ValidTypeFile = "\\valid-typenames.tsv";
        private const string InvalidTypeFile = "\\invalid-typenames.tsv";
        private const string ValidMethodFile = "\\valid-methodnames.tsv";
        private const string InvalidMethodFile = "\\invalid-methodnames.tsv";
        private const string ValidDelegatesFile = "\\valid-delegates.tsv";
        private const string InvalidDelegatesFile = "\\invalid-delegates.tsv";
        private const string ValidArrayTypesFile = "\\valid-arraytypes.tsv";
        private const string InvalidArrayTypesFile = "\\invalid-arraytypes.tsv";
        private const string ValidTypeParameterFile = "\\valid-typeparameternames.tsv";
        private const string InvalidTypeParameterFile = "\\invalid-typeparameternames.tsv";

        public static string[] LoadTestFile(string file)
        {
            return
                File.ReadAllLines(file)
                    .Where(s => !s.Equals("") && !s.StartsWith("#") && !s.StartsWith("Identifier"))
                    .ToArray();
        }

        public static IKaVEList<T> LoadTestCase<T>(string path) where T : ITestCase
        {
            var testcases = new KaVEList<T>();
            var lines = LoadTestFile(path);
            foreach (var line in lines)
            {
                var fields = line.Split('\t');
                var t = (T) CreateTestCase(fields, typeof(T));
                testcases.Add(t);
            }
            return testcases;
        }

        private static ITestCase CreateTestCase(string[] fields, Type type)
        {
            if (type == typeof(MethodNameTestCase))
            {
                return new MethodNameTestCase
                {
                    Identifier = fields[0],
                    DeclaringType = fields[1],
                    ReturnType = fields[2],
                    SimpleName = fields[3],
                    Parameters = GetList(fields[4]),
                    TypeParameters = GetList(fields[5]),
                    IsStatic = GetBoolean(fields[6]),
                    HasTypeParameters = GetBoolean(fields[7])
                };
            }
            if (type == typeof(TypeNameTestCase))
            {
                return new TypeNameTestCase
                {
                    Identifier = fields[0],
                    Namespace = fields[1],
                    Assembly = fields[2],
                    FullName = fields[3],
                    Name = fields[4],
                    TypeParameters = GetList(fields[5]),
                    IsReferenceType = GetBoolean(fields[6]),
                    IsClassType = GetBoolean(fields[7]),
                    IsInterfaceType = GetBoolean(fields[8]),
                    IsEnumType = GetBoolean(fields[9]),
                    IsStructType = GetBoolean(fields[10]),
                    IsNestedType = GetBoolean(fields[11])
                };
            }
            if (type == typeof(DelegateTypeNameTestCase))
            {
                return new DelegateTypeNameTestCase
                {
                    Identifier = fields[0],
                    Namespace = fields[1],
                    Assembly = fields[2],
                    FullName = fields[3],
                    Name = fields[4],
                    DeclaringType = fields[5],
                    ReturnType = fields[6],
                    Signature = fields[7],
                    Parameters = GetList(fields[8]),
                    TypeParameters = GetList(fields[9])
                };
            }
            if (type == typeof(ArrayTypeNameTestCase))
            {
                return new ArrayTypeNameTestCase
                {
                    Identifier = fields[0],
                    Namespace = fields[1],
                    Assembly = fields[2],
                    FullName = fields[3],
                    Name = fields[4],
                    ArrayBaseType = fields[5],
                    TypeParameters = GetList(fields[6]),
                    IsInterfaceType = GetBoolean(fields[7]),
                    IsEnumType = GetBoolean(fields[8]),
                    IsStructType = GetBoolean(fields[9]),
                    IsNestedType = GetBoolean(fields[10]),
                    IsDelgateType = GetBoolean(fields[11]),
                    HasTypeParameters = GetBoolean(fields[12]),
                    Rank = GetInt(fields[13])
                };
            }
            if (type == typeof(TypeParameterNameTestCase))
            {
                return new TypeParameterNameTestCase
                {
                    Identifier = fields[0],
                    Namespace = fields[1],
                    Assembly = fields[2],
                    FullName = fields[3],
                    Name = fields[4],
                    TypeParameterShortName = fields[5],
                    TypeParameterType = fields[6]
                };
            }
            return null;
        }

        private static int GetInt(string s)
        {
            return Int32.Parse(s);
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
            return LoadTestCase<TypeNameTestCase>(TestSourceRootFolder + ValidTypeFile);
        }

        public static IKaVEList<string> InvalidTypeNames()
        {
            return Lists.NewList(LoadTestFile(TestSourceRootFolder + InvalidTypeFile));
        }

        public static IKaVEList<DelegateTypeNameTestCase> ValidDelegates()
        {
            return LoadTestCase<DelegateTypeNameTestCase>(TestSourceRootFolder + ValidDelegatesFile);
        }

        public static IKaVEList<string> InvalidDelegates()
        {
            return Lists.NewList(LoadTestFile(TestSourceRootFolder + InvalidDelegatesFile));
        }

        public static IKaVEList<ArrayTypeNameTestCase> ValidArrayTypes()
        {
            return LoadTestCase<ArrayTypeNameTestCase>(TestSourceRootFolder + ValidArrayTypesFile);
        }

        public static IKaVEList<string> InvalidArrayTypes()
        {
            return Lists.NewList(LoadTestFile(TestSourceRootFolder + InvalidArrayTypesFile));
        }

        public static IKaVEList<MethodNameTestCase> ValidMethodNames()
        {
            return LoadTestCase<MethodNameTestCase>(TestSourceRootFolder + ValidMethodFile);
        }

        public static IKaVEList<string> InvalidMethodNames()
        {
            return Lists.NewList(LoadTestFile(TestSourceRootFolder + InvalidMethodFile));
        }

        public static IKaVEList<TypeParameterNameTestCase> ValidTypeParameterNames()
        {
            return LoadTestCase<TypeParameterNameTestCase>(TestSourceRootFolder + ValidTypeParameterFile);
        }

        public static IKaVEList<string> InvalidTypeParameterNames()
        {
            return Lists.NewList(LoadTestFile(TestSourceRootFolder + InvalidTypeParameterFile));
        }
    }

    internal interface ITestCase {}
}