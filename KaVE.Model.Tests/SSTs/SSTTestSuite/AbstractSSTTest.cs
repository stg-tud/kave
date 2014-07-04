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

using KaVE.Model.SSTs;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.SSTTestSuite
{
    // ReSharper disable once InconsistentNaming
    internal class AbstractSSTTest
    {
        internal SST AnalysisResult;

        internal void Analyze(string input)
        {
            WhenTriggeredIn("$ " + input);
        }

        internal void AnalyzeMulti(string s)
        {
            throw new System.NotImplementedException();
        }

        internal void WhenTriggeredIn(string input)
        {
            throw new System.NotImplementedException();
        }

        internal MethodDeclaration NewMethodDeclaration(string empty)
        {
            throw new System.NotImplementedException();
        }

        internal void AssertResult(SST expected)
        {
            Assert.AreEqual(expected, AnalysisResult);
        }

        internal void AssertEntryPoints(params MethodDeclaration[] expectedDecl)
        {
            throw new System.NotImplementedException();
        }

        internal void AssertMethodDeclaration(MethodDeclaration expectedDecl)
        {
            //string methodIdentifier = "Get from expectedDecl";
            throw new System.NotImplementedException();
        }

        internal void AssertMethodDeclarations(params MethodDeclaration[] expectedDecl)
        {
            throw new System.NotImplementedException();
        }
    }
}