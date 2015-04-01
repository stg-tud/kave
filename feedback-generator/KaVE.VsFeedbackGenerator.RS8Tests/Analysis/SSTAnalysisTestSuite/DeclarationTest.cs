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

using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    internal class DeclarationTest : BaseSSTAnalysisTest
    {
        [Test, Ignore]
        public void UntypedVariable()
        {
            CompleteInClass(@"
                public void A()
                {
                    var i;
                    $
                }
            ");

            var mA = NewMethodDeclaration(SSTAnalysisFixture.Void, "A");
            mA.Body.Add(SSTUtil.Declare("i", Fix.Unknown));

            AssertAllMethods(mA);
        }

        [Test, Ignore]
        public void ExplicitlyTypedVariable()
        {
            CompleteInClass(@"
                public void A()
                {
                    int i;
                    $
                }
            ");

            var mA = NewMethodDeclaration(SSTAnalysisFixture.Void, "A");
            mA.Body.Add(SSTUtil.Declare("i", Fix.Int));

            AssertAllMethods(mA);
        }

        [Test, Ignore]
        public void ImplicitlyTypedVariable()
        {
            CompleteInClass(@"
                public void A()
                {
                    var i = 3;
                    $
                }
            ");

            var mA = NewMethodDeclaration(SSTAnalysisFixture.Void, "A");
            mA.Body.Add(SSTUtil.Declare("i", Fix.Int));
            mA.Body.Add(SSTUtil.AssignmentToLocal("i", new ConstantValueExpression()));

            AssertAllMethods(mA);
        }

        [Test, Ignore]
        public void LambdaDeclaration()
        {
            CompleteInClass(@"
                public class C {
                    public void M() {}
                }
                public void A(C c) {
                    Action<C> inc = (C c) => c.M();
                    inc(c)
                    $
                }
            ");

            var mA = NewMethodDeclaration(SSTAnalysisFixture.Void, "A");
            // mA.Body.Add(new LambdaDeclaration("inc", TypeName.Get("Action<C> ..?")));
            // mA.Body.Add(new LambdaInvocation("inc", "c"));

            // TODO assert that c.M does not show up

            AssertAllMethods(mA);
        }
    }
}