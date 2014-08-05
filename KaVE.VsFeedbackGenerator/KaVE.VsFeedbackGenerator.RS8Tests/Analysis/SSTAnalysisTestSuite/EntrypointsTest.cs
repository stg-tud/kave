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

using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    internal class EntrypointsTest : BaseSSTAnalysisTest
    {
        [Test]
        public void PrivateIsInlined()
        {
            CompleteInClass(@"$
                private void PrivateA() {}
                public void PublicA() {}
            ");

            var mA = NewMethodDeclaration(Fix.Void, "PublicA");

            AssertEntryPoints(mA);
        }

        [Test]
        public void EPsAndNonEPsAreDistinguished()
        {
            CompleteInClass(@"$
                public void A() { B();}
                public void B() {}
                public void C() {}
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            var mB = NewMethodDeclaration(Fix.Void, "B"); // ...
            //mB.Body.Add(new Invocation("this", Fix.GetMethodName("A.B")));
            var mC = NewMethodDeclaration(Fix.Void, "C");

            AssertEntryPoints(mA, mC);
            AssertMethodDeclarations(mB);
        }

        // TODO @Seb: adapt remaining tests from EntryPointSelectorTest
    }
}