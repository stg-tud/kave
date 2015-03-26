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

using System.Linq;
using JetBrains.Application;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs.Impl.Declarations;
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

            var contextForTypeWithDependency = results.AnalyzedContexts.First(
                context => context.TypeShape.TypeHierarchy.Element.Name.Equals("ClassWithCoreLibDependency"));
            var expectedDeclaration = new FieldDeclaration
            {
                Name = FieldName.Get(
                    "[i:System.Collections.Generic.IList`1[[T -> System.String, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0] [Project1.ClassWithCoreLibDependency, Project1].MyList")
            };
            CollectionAssert.Contains(contextForTypeWithDependency.SST.Fields, expectedDeclaration);
        }

        private SolutionAnalysis.AnalysesResults RunAnalysis()
        {
            SolutionAnalysis.AnalysesResults results = null;
            DoTestSolution(
                (lifetime, solution) =>
                    results =
                        new SolutionAnalysis(solution, Shell.Instance.GetComponent<ILogger>()).AnalyzeAllProjects());
            return results;
        }
    }
}