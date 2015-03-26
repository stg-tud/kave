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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Application;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;
using ILogger = KaVE.Utils.Exceptions.ILogger;

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

            CollectionAssert.AreEquivalent(new[] {"Project1", "Project2"}, results.AnalyzedProjectNames);
        }

        [Test]
        public void AnalyzesGlobalClass()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFileNames, "GlobalClass.cs");
            CollectionAssert.Contains(results.AnalyzedTypes, TypeName.Get("GlobalClass, Project1"));
        }

        [Test]
        public void AnalyzesClassInNamespace()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFileNames, "ClassInNamespace.cs");
            CollectionAssert.Contains(results.AnalyzedTypes, TypeName.Get("Project1.ClassInNamespace, Project1"));
        }

        [Test]
        public void AnalyzesMultipleClassesInOneFileInSameNamespace()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFileNames, "MultipleClassesFile.cs");
            CollectionAssert.Contains(results.AnalyzedTypes, TypeName.Get("Project1.SiblingClass1, Project1"));
            CollectionAssert.Contains(results.AnalyzedTypes, TypeName.Get("Project1.SiblingClass2, Project1"));
        }

        [Test]
        public void AnalyzesClassInSubfolders()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFileNames, "ClassInFileInFolder.cs");
            CollectionAssert.Contains(results.AnalyzedTypes, TypeName.Get("Project1.A.B.ClassInFileInFolder, Project1"));
        }

        [Test]
        public void AnalyzesClassInNestedNamespace()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFileNames, "ClassInNestedNamespace.cs");
            CollectionAssert.Contains(results.AnalyzedTypes, TypeName.Get("Project1.A.B.C.ClassInNestedNamespace, Project1"));
        }

        [Test]
        public void AnalyzesNestedClass()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFileNames, "NestedClasses.cs");
            CollectionAssert.Contains(results.AnalyzedTypes, TypeName.Get("Project1.OuterClass+InnerClass, Project1"));
        }

        [Test]
        public void DoesNotAnalyzeInterface()
        {
            var results = RunAnalysis();

            CollectionAssert.Contains(results.AnalyzedFileNames, "IInterface.cs");
            CollectionAssert.DoesNotContain(results.AnalyzedTypes, TypeName.Get("IInterface, Project1"));
            // this happened initially, because analysis failed to resolve the interface's type shape
            CollectionAssert.DoesNotContain(results.AnalyzedTypes, TypeName.UnknownName);
        }

        private TestAnalysesResults RunAnalysis()
        {
            SolutionAnalysis.AnalysesResults results = null;
            DoTestSolution(
                (lifetime, solution) =>
                    results =
                        new SolutionAnalysis(solution, Shell.Instance.GetComponent<ILogger>()).AnalyzeAllProjects());
            return new TestAnalysesResults(results);
        }

        private class TestAnalysesResults
        {
            private readonly SolutionAnalysis.AnalysesResults _results;

            public TestAnalysesResults(SolutionAnalysis.AnalysesResults results)
            {
                _results = results;
            }

            public IEnumerable<string> AnalyzedProjectNames
            {
                get { return _results.AnalyzedProjects; }
            }

            public IEnumerable<string> AnalyzedFileNames
            {
                get { return _results.AnalyzedFiles.Select(Path.GetFileName); }
            }

            public IEnumerable<ITypeName> AnalyzedTypes
            {
                get { return _results.AnalyzedContexts.Select(context => context.TypeShape.TypeHierarchy.Element); }
            }
        }
    }
}