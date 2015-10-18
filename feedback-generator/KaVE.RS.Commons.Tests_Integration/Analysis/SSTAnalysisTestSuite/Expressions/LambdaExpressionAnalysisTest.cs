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

using System;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Expressions
{
    internal class LambdaExpressionAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void StandardAction_EmptyBody()
        {
            CompleteInMethod(@"
                Action<int> a = (int i) => {  };
                $");

            AssertBody(
                VarDecl("a", Fix.ActionOfInt),
                VarAssign("a", new LambdaExpression
                {
                    Name = LambdaName.Get("[System.Void, mscorlib, 4.0.0.0] ([System.Int32, mscorlib, 4.0.0.0] i)"),
                }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void StandardAction()
        {
            CompleteInMethod(@"
                Action<int> a = (int i) => { return; };
                $");

            AssertBody(
                VarDecl("a", Fix.ActionOfInt),
                VarAssign("a", new LambdaExpression
                {
                    Name = LambdaName.Get("[System.Void, mscorlib, 4.0.0.0] ([System.Int32, mscorlib, 4.0.0.0] i)"),
                    Body = { new ReturnStatement {IsVoid = true} }
                }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void StandardFunc()
        {
            CompleteInMethod(@"
                Func<int, int> a = (int i) => { return i + 1; };
                $");

            AssertBody(
                VarDecl("a", Fix.FuncOfIntAndInt),
                VarAssign("a", new LambdaExpression
                {
                    Name = LambdaName.Get("[System.Int32, mscorlib, 4.0.0.0] ([System.Int32, mscorlib, 4.0.0.0] i)"),
                    Body =
                    {
                        VarDecl("$0", Fix.Int),
                        VarAssign("$0", new ComposedExpression {References = {VarRef("i")}}),
                        new ReturnStatement { IsVoid = false, Expression = new ReferenceExpression {Reference = VarRef("$0")}}
                    }
                }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void StandardFunc_ShorthandSyntax()
        {
            CompleteInMethod(@"
                Func<int, int> a = i => i + 1;
                $");

            AssertBody(
                VarDecl("a", Fix.FuncOfIntAndInt),
                VarAssign("a", new LambdaExpression
                {
                    Name = LambdaName.Get("[System.Int32, mscorlib, 4.0.0.0] ([System.Int32, mscorlib, 4.0.0.0] i)"),
                    Body =
                    {
                        VarDecl("$0", Fix.Int),
                        VarAssign("$0", new ComposedExpression {References = {VarRef("i")}}),
                        new ReturnStatement { IsVoid = false, Expression = new ReferenceExpression {Reference = VarRef("$0")}}
                    }
                }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void LambdaWithBodyBlock_CompletionWithin()
        {
            CompleteInMethod(@"Action<int> a = (int i) => { $ };");

            AssertBody(
                VarDecl("a", Fix.ActionOfInt),
                VarAssign("a", new LambdaExpression
                {
                    Name = LambdaName.Get("[System.Void, mscorlib, 4.0.0.0] ([System.Int32, mscorlib, 4.0.0.0] i)"),
                    Body = { Fix.EmptyCompletion }
                }));
        }

        [Test, Ignore]
        public void StandardLambdaWithBodyExpression_CompletionWithin()
        {
            // AffectedNode is the variable declaration (case: in body)
            CompleteInMethod(@"Func<int, int> a = i => $;");

            AssertBody(
                VarDecl("a", Fix.FuncOfIntAndInt),
                VarAssign("a", new LambdaExpression
                {
                    Name = LambdaName.Get("[System.Int32, mscorlib, 4.0.0.0] ([System.Int32, mscorlib, 4.0.0.0] i)"),
                    Body =
                    {
                        Fix.EmptyCompletion
                    }
                }));
        }
    }
}
