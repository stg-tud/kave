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

using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Statements
{
    internal class AssignmentAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void VariableInit_CompletionAfter()
        {
            CompleteInMethod(@"
                int i = 3;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void VariableInit_CompletionBefore()
        {
            CompleteInMethod(@"
                $
                int i = 3;
            ");

            AssertBody(
                Fix.EmptyCompletion,
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()));
        }

        [Test]
        public void VariableInit_CompletionIn()
        {
            CompleteInMethod(@"
                int i = $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new CompletionExpression()));
        }

        [Test]
        public void FieldAssignment_CompletionAfter()
        {
            CompleteInClass(@"
                private int _f = 3;
                public void M(){
                    _f = 3;
                    $
                }
            ");

            AssertBody(
                Assign(FieldRef("_f", Fix.Int), new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void FieldAssignment_CompletionBefore()
        {
            CompleteInClass(@"
                private int _f = 3;
                public void M(){
                    $
                    _f = 3;
                }
            ");

            AssertBody(
                Fix.EmptyCompletion,
                Assign(FieldRef("_f", Fix.Int), new ConstantValueExpression()));
        }

        [Test]
        public void FieldAssignment_CompletionInEmptyAssignment()
        {
            CompleteInClass(@"
                private int _f = 3;
                public void M(){
                    _f = $
                }
            ");

            AssertBody(
                Assign(FieldRef("_f", Fix.Int), new CompletionExpression()));
        }

        [Test]
        public void VarAssignment_CompletionAfter()
        {
            CompleteInMethod(@"
                int i;
                i = 3;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void VarAssignment_CompletionInEmptyAssignment()
        {
            CompleteInMethod(@"
                int i;
                i = $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new CompletionExpression()));
        }

        [Test]
        public void PropertyAssignment_CompletionAfter()
        {
            CompleteInClass(@"
                private int P {get; set;};
                public void M(){
                    P = 3;
                    $
                }
            ");

            AssertBody(
                Assign(PropRef("P", Fix.Int), new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void PropertyAssignment_CompletionBefore()
        {
            CompleteInClass(@"
                private int P {get; set;};
                public void M(){
                    $
                    P = 3;
                }
            ");

            AssertBody(
                Fix.EmptyCompletion,
                Assign(PropRef("P", Fix.Int), new ConstantValueExpression()));
        }

        [Test]
        public void PropertyAssignment_CompletionInEmptyAssignment()
        {
            CompleteInClass(@"
                private int P {get; set;};
                public void M(){
                    P = $
                }
            ");

            AssertBody(
                Assign(PropRef("P", Fix.Int), new CompletionExpression()));
        }

        [Test]
        public void Assigning_Literals()
        {
            CompleteInMethod(@"
                int i;
                i = 3;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Assigning_Literals_This()
        {
            CompleteInMethod(@"
                object o;
                o = this;
                $
            ");

            AssertBody(
                VarDecl("o", Fix.Object),
                Assign("o", RefExpr("this")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Assigning_Literals_Null()
        {
            CompleteInMethod(@"
                object o;
                o = null;
                $
            ");

            AssertBody(
                VarDecl("o", Fix.Object),
                Assign("o", Const("null")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Assigning_Variables()
        {
            CompleteInMethod(@"
                int i;
                int j = 0;
                i = j;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                VarDecl("j", Fix.Int),
                Assign("j", Const("0")),
                Assign("i", RefExpr("j")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Assigning_Events()
        {
            CompleteInClass(@"
                private event Action E;
                public void M(){
                    var e = E;
                    $
                }
            ");

            AssertBody(
                VarDecl("e", Fix.Action),
                Assign(VarRef("e"), RefExpr(EventRef("E", Fix.Action))),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Assigning_Methods()
        {
            CompleteInClass(@"
                private void H() {}
                public void M(){
                    Action a = H;
                    $
                }
            ");

            AssertBody(
                "M",
                VarDecl("a", Fix.Action),
                Assign(VarRef("a"), RefExpr(MethodRef("H", Fix.Void, Fix.TestClass))),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Assigning_Properties()
        {
            CompleteInClass(@"
                private int P { get; set; }
                public void M(){
                    var p = P;
                    $
                }
            ");

            AssertBody(
                VarDecl("p", Fix.Int),
                Assign(VarRef("p"), RefExpr(PropRef("P", Fix.Int))),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Assigning_Fields()
        {
            CompleteInClass(@"
                private int _f;
                public void M(){
                    var f = _f;
                    $
                }
            ");

            AssertBody(
                VarDecl("f", Fix.Int),
                Assign(VarRef("f"), RefExpr(FieldRef("_f", Fix.Int))),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Assigning_InvocationExpressions()
        {
            CompleteInMethod(@"
                var o = this.ToString();
                $
            ");

            AssertBody(
                VarDecl("o", Fix.String),
                Assign("o", Invoke("this", Fix.ToString(Fix.Object))),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Assigning_SelfAssignIsIgnored()
        {
            CompleteInMethod(@"
                int i = 0;
                i = i;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", Const("0")),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Assigning_SelfAssignInInitIsIgnored()
        {
            CompleteInMethod(@"
                int i = i;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Fancy1()
        {
            CompleteInMethod(@"
                int i = 1, j = 2;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", Const("1")),
                VarDecl("j", Fix.Int),
                Assign("j", Const("2")),
                Fix.EmptyCompletion);
        }

        [Test, Ignore]
        public void Fancy2()
        {
            CompleteInMethod(@"
                int i;
                var j = i = 2;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", Const("2")),
                VarDecl("j", Fix.Int),
                Assign("j", RefExpr("i")),
                Fix.EmptyCompletion);
        }

        [Test]
        public void SyntacticSugar_Add()
        {
            CompleteInMethod(@"
                int i = 0;
                i += 1;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", Const("0")),
                new Assignment
                {
                    Reference = VarRef("i"),
                    Expression = ComposedExpr() // TODO: extend
                },
                Fix.EmptyCompletion);
        }

        [Test]
        public void SyntacticSugar_Remove()
        {
            CompleteInMethod(@"
                int i = 0;
                i -= 1;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", Const("0")),
                new Assignment
                {
                    Reference = VarRef("i"),
                    Expression = ComposedExpr() // TODO: extend
                },
                Fix.EmptyCompletion);
        }

        // TODO extend syntactic sugar examples and move them to separate file
    }
}