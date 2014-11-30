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
 *    - Sebastian Proksch
 */

using System.Collections.Generic;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Declarations;
using KaVE.VsFeedbackGenerator.Generators;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    internal abstract class BaseSSTAnalysisTest : BaseTest
    {
        internal readonly IList<string> Log = new List<string>();

        [SetUp]
        public void RegisterLogger()
        {
            var loggerMock = new Mock<ILogger>();
            loggerMock.Setup(l => l.Info(It.IsAny<string>())).Callback<string>(info => Log.Add(info));
            Registry.RegisterComponent(loggerMock.Object);
        }

        internal SST NewSST()
        {
            return new SST
            {
                EnclosingType = TypeName.Get("N.C, TestProject")
            };
        }

        internal MethodDeclaration NewMethodDeclaration(ITypeName returnType, string simpleName)
        {
            return NewMethodDeclaration(returnType, simpleName, new string[0]);
        }

        internal MethodDeclaration NewMethodDeclaration(ITypeName returnType, string simpleName, params string[] args)
        {
            const string package = "N.C, TestProject";
            var identifier = string.Format(
                "[{0}] [{1}].{2}({3})",
                returnType,
                package,
                simpleName,
                string.Join(", ", args));
            return new MethodDeclaration
            {
                Name = MethodName.Get(identifier),
                IsEntryPoint = true
            };
        }

        internal void AssertResult(SST expected)
        {
            Assert.AreEqual(expected, ResultSST);
        }

        internal void AssertMethod(MethodDeclaration expected)
        {
            Assert.IsTrue(ResultSST.Methods.Contains(expected));
        }

        internal void AssertAllMethods(params MethodDeclaration[] expectedDecls)
        {
            var ms = ResultSST.Methods;
            Assert.AreEqual(expectedDecls.Length, ms.Count);

            foreach (var expectedDecl in expectedDecls)
            {
                Assert.IsTrue(ms.Contains(expectedDecl));
            }
        }

        [TearDown]
        public void ClearRegistry()
        {
            Registry.Clear();
            var logCopy = new List<string>(Log);
            Log.Clear();
            Assert.IsEmpty(logCopy);
        }
    }
}