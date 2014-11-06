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

using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Declarations;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    internal abstract class BaseSSTAnalysisTest : BaseTest
    {
        internal SST NewSST()
        {
            return new SST();
        }

        internal MethodDeclaration NewMethodDeclaration(ITypeName returnType, string simpleName)
        {
            const string package = "N.C, TestProject";
            var identifier = string.Format("[{0}] [{1}].{2}()", returnType, package, simpleName);
            return new MethodDeclaration {Name = MethodName.Get(identifier)};
        }

        internal void AssertResult(SST expected)
        {
            Assert.AreEqual(expected, ResultSST);
        }

        internal void AssertEntryPoints(params MethodDeclaration[] expectedDecls)
        {
            var eps = ResultSST.EntryPoints;
            Assert.AreEqual(expectedDecls.Length, eps.Count);

            CollectionAssert.AreEqual(expectedDecls, eps);
            // TODO: @Seb: CollectionAssert works but contains doesn't
            /*foreach (var expectedDecl in expectedDecls)
            {
                Assert.IsTrue(eps.Contains(expectedDecl));
            }*/
        }

        internal void AssertMethodDeclarations(params MethodDeclaration[] expectedDecls)
        {
            var neps = ResultSST.NonEntryPoints;
            Assert.AreEqual(expectedDecls.Length, neps.Count);

            foreach (var expectedDecl in expectedDecls)
            {
                Assert.IsTrue(neps.Contains(expectedDecl));
            }
        }
    }
}