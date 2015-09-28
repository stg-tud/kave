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

using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Expressions
{
    internal class LoopHeaderExpressionAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void ConstantValue()
        {
            CompleteInMethod(@"
                while(true) {}
                $
            ");

            AssertBody(
                new WhileLoop
                {
                    Condition = new ConstantValueExpression()
                },
                Fix.EmptyCompletion);
        }

        [Test]
        public void SimpleExpression()
        {
            CompleteInMethod(@"
                var isX = true;
                while(isX) {}
                $
            ");

            AssertBody(
                VarDecl("isX", Fix.Bool),
                Assign("isX", new ConstantValueExpression()),
                new WhileLoop
                {
                    Condition = RefExpr("isX")
                },
                Fix.EmptyCompletion);
        }


        [Test]
        public void MethodInvocation()
        {
            CompleteInClass(@"
                public void M() {
                    while(IsX()) {}
                    $
                }
                public bool IsX() {
                    return true;
                }
            ");

            AssertBody(
                "M",
                new WhileLoop
                {
                    Condition = new LoopHeaderBlockExpression
                    {
                        Body =
                        {
                            VarDecl("$0", Fix.Bool),
                            Assign("$0", Invoke("this", Method("[{0}] [{1}].IsX()", Fix.Bool, Fix.TestClass))),
                            new ReturnStatement{Expression = RefExpr("$0"), IsVoid = false}
                        }
                    }
                },
                Fix.EmptyCompletion);
        }
    }
}