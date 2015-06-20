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

using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using NUnit.Framework;
using Fix = KaVE.ReSharper.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.ReSharper.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Statements
{
    internal class VariableDeclarationTest : BaseSSTAnalysisTest
    {
        [Test]
        public void UntypedVariable()
        {
            CompleteInMethod(@"
                var i;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Unknown),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void ExplicitlyTypedVariable()
        {
            CompleteInMethod(@"
                int i;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void ImplicitlyTypedVariable()
        {
            CompleteInMethod(@"
                var i = 3;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()),
                ExprStmt(new CompletionExpression()));
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

            Assert.Fail();
        }
    }
}