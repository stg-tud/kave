﻿/*
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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.ReSharper.Commons.Utils;
using NUnit.Framework;

namespace KaVE.SolutionAnalysis.Tests
{
    [TestFixture, TestNetFramework4]
    internal class SolutionAnalysisTest : BaseTestWithExistingSolution
    {
        protected override string RelativeTestDataPath
        {
            get { return @"TestSolution"; }
        }

        protected override FileSystemPath ExistingSolutionFilePath
        {
            get { return GetTestDataFilePath2("TestSolution.sln"); }
        }

        [Test]
        public void AnalyzesAllProjects()
        {
            var results = RunAnalysis();

            CollectionAssert.AreEquivalent(new[] {"Project1", "Project2"}, results.AnalyzedProjectsNames);
        }

        [Test]
        public void AnalyzesGlobalClass()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFilesNames, @"<Project1>\GlobalClass.cs");
            CollectionAssert.Contains(results.AnalyzedTypesNames, "GlobalClass");
        }

        [Test]
        public void AnalyzesClassInNamespace()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFilesNames, @"<Project1>\ClassInNamespace.cs");
            CollectionAssert.Contains(results.AnalyzedTypesNames, "Project1.ClassInNamespace");
        }

        [Test]
        public void AnalyzesMultipleClassesInOneFileInSameNamespace()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFilesNames, @"<Project1>\MultipleClassesFile.cs");
            CollectionAssert.Contains(results.AnalyzedTypesNames, "Project1.SiblingClass1");
            CollectionAssert.Contains(results.AnalyzedTypesNames, "Project1.SiblingClass2");
        }

        [Test]
        public void AnalyzesClassInSubfolders()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFilesNames, @"<Project1>\A\B\ClassInFileInFolder.cs");
            CollectionAssert.Contains(results.AnalyzedTypesNames, "Project1.A.B.ClassInFileInFolder");
        }

        [Test]
        public void AnalyzesClassInNestedNamespace()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFilesNames, @"<Project1>\ClassInNestedNamespace.cs");
            CollectionAssert.Contains(results.AnalyzedTypesNames, "Project1.A.B.C.ClassInNestedNamespace");
        }

        [Test]
        public void AnalyzesNestedClass()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFilesNames, @"<Project1>\NestedClasses.cs");
            CollectionAssert.Contains(results.AnalyzedTypesNames, "Project1.OuterClass+InnerClass");
        }

        [Test]
        public void AnalyzesStruct()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFilesNames, @"<Project1>\Struct.cs");
            CollectionAssert.Contains(results.AnalyzedTypesNames, "Project1.Struct");
        }

        [Test]
        public void DoesNotAnalyzeInterface()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFilesNames, @"<Project1>\IInterface.cs");
            CollectionAssert.DoesNotContain(results.AnalyzedTypesNames, "Project1.IInterface");
        }

        [Test]
        public void DoesNotAnalyzeEnum()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFilesNames, @"<Project1>\Enum.cs");
            CollectionAssert.DoesNotContain(results.AnalyzedTypesNames, "Project1.Enum");
        }

        [Test]
        public void DoesNotAnalyzeDelegate()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFilesNames, @"<Project1>\Delegate.cs");
            CollectionAssert.DoesNotContain(results.AnalyzedTypesNames, "Project1.SomeDelegate");
        }

        [Test]
        public void AnalysisResolvesLocalTypes()
        {
            var results = RunAnalysis();

            var analyzedTypes = results.AnalyzedContexts.Select(context => context.TypeShape.TypeHierarchy.Element);
            CollectionAssert.Contains(analyzedTypes, TypeName.Get("Project1.ClassInNamespace, Project1"));
        }

        [Test]
        public void AnalysisResolvesCoreDependencies()
        {
            var results = RunAnalysis();

            var context = GetContextForType(results, "ClassWithCoreLibDependency");
            var expectedDeclaration = new FieldDeclaration
            {
                Name = FieldName.Get(
                    "[i:System.Collections.Generic.IList`1[[T -> System.String, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0] [Project1.ClassWithCoreLibDependency, Project1].MyList")
            };
            CollectionAssert.Contains(context.SST.Fields, expectedDeclaration);
        }

        [Test, Ignore("This test runs on my machine, but not on the build server. I have no idea why...")]
        public void AnalysisResolvesNuGetDependencies()
        {
            var results = RunAnalysis();

            var context = GetContextForType(results, "ClassWithNuGetDependency");
            var expectedDeclaration = new FieldDeclaration
            {
                Name = FieldName.Get(
                    "[Newtonsoft.Json.JsonConverter, Newtonsoft.Json, 6.0.0.0] [Project1.ClassWithNuGetDependency, Project1].MyConverter")
            };
            CollectionAssert.Contains(context.SST.Fields, expectedDeclaration);
        }

        private Results RunAnalysis()
        {
            IList<string> infos = new List<string>();
            var testLogger = Registry.GetComponent<TestLogger>();
            testLogger.InfoLogged += infos.Add;

            IEnumerable<Context> results = null;
            DoTestSolution(
                (lifetime, solution) =>
                    results =
                        new SolutionAnalysis(solution, testLogger).AnalyzeAllProjects());

            testLogger.InfoLogged -= infos.Add;
            return new Results(results, infos);
        }

        private class Results
        {
            public Results(IEnumerable<Context> contexts, IList<string> loggedInfos)
            {
                AnalyzedContexts = contexts;
                LoggedInfos = loggedInfos;
            }

            private IList<string> LoggedInfos { get; set; }

            public IEnumerable<Context> AnalyzedContexts { get; private set; }

            public IEnumerable<string> AnalyzedProjectsNames
            {
                get { return LoggedInfos.Where(info => info.Contains("Analyzing project")).Select(ParseEntityName); }
            }

            public IEnumerable<string> AnalyzedFilesNames
            {
                get { return LoggedInfos.Where(info => info.Contains("Analyzing file")).Select(ParseEntityName); }
            }

            public IEnumerable<string> AnalyzedTypesNames
            {
                get { return LoggedInfos.Where(info => info.Contains("Analyzing type")).Select(ParseEntityName); }
            }

            private static string ParseEntityName(string info)
            {
                var startOfName = info.IndexOf('\'') + 1;
                var lengthOfName = info.LastIndexOf('\'') - startOfName;
                return info.Substring(startOfName, lengthOfName);
            }
        }

        private static Context GetContextForType(Results results, string classname)
        {
            return results.AnalyzedContexts.First(
                context => context.TypeShape.TypeHierarchy.Element.Name.Equals(classname));
        }
    }
}