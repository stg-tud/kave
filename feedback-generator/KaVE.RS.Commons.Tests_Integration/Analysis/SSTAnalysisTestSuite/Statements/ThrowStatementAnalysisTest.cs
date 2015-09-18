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

using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Statements
{
    internal class ThrowStatementAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void LonelyRethrow()
        {
            // Technically incorrect code since rethrows are only allowed in catch blocks.
            CompleteInMethod(@"
                throw;
                $");

            AssertBody(
                new ThrowStatement(),
                Fix.EmptyCompletion);
        }

        [Test]
        public void LonelyRethrow_CompletionBefore()
        {
            CompleteInMethod(@"
                $
                throw;");

            AssertBody(
                Fix.EmptyCompletion,
                new ThrowStatement());
        }

        [Test]
        public void LonelyThrow()
        {
            CompleteInMethod(@"
                throw new Exception(""msg"");
                $");

            AssertBody(
                VarDecl("$0", Fix.Exception),
                VarAssign("$0", InvokeCtor(Fix.Exception_ctor, new ConstantValueExpression())),
                new ThrowStatement {Reference = VarRef("$0")},
                Fix.EmptyCompletion);
        }

        [Test]
        public void LonelyThrow_UnassignedVariable()
        {
            CompleteInMethod(@"
                throw e;
                $");

            AssertBody(
                new ThrowStatement { Reference = VarRef("e") },
                Fix.EmptyCompletion);
        }

        [Test]
        public void TwoThrowsWithCompletionBetween()
        {
            CompleteInMethod(@"
                throw new Exception(""msg"");
                $
                throw new Exception(""msg"");");

            AssertBody(
                VarDecl("$0", Fix.Exception),
                VarAssign("$0", InvokeCtor(Fix.Exception_ctor, new ConstantValueExpression())),
                new ThrowStatement {Reference = VarRef("$0")},
                Fix.EmptyCompletion,
                VarDecl("$1", Fix.Exception),
                VarAssign("$1", InvokeCtor(Fix.Exception_ctor, new ConstantValueExpression())),
                new ThrowStatement {Reference = VarRef("$1")});
        }

        [Test]
        public void ThrowInLoop()
        {
            CompleteInMethod(@"
                while (true)
                {
                    throw;
                }
                $
            ");

            AssertBody(
                new WhileLoop
                {
                    Condition = new ConstantValueExpression(),
                    Body =
                    {
                        new ThrowStatement()
                    }
                },
                Fix.EmptyCompletion);
        }

        [Test]
        public void TriggerInside()
        { 
            CompleteInMethod(@"throw $");

            AssertBody(
                VarDecl("$0", Fix.Exception),
                VarAssign("$0", new CompletionExpression()),
                new ThrowStatement {Reference = VarRef("$0")},
                Fix.EmptyCompletion);
        }

        [Test, Ignore]
        public void TriggerInside_WithToken()
        {
            // Exception => ReferenceExpression:e
            // Affected Node => ReferenceExpression:e
            // Case => Undefined
            CompleteInMethod(@"throw e$");

            AssertBody(
                VarDecl("$0", Fix.Exception),
                VarAssign("$0", new CompletionExpression {Token = "e"}),
                new ThrowStatement { Reference = VarRef("$0") },
                Fix.EmptyCompletion);
        }

        [Test, Ignore]
        public void TriggerInside_WithNewObject()
        {
            // Exception => IObjectCreationExpression 
            // Affected Node => ReferenceName:Ex
            // Case => Undefined
            CompleteInMethod(@"throw new Ex$");

            AssertBody(
                VarDecl("$0", Fix.Exception),
                VarAssign("$0", new CompletionExpression()), // not sure how this completion would look
                new ThrowStatement { Reference = VarRef("$0") },
                Fix.EmptyCompletion);
        }
    }
}