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
using KaVE.Commons.Model.SSTs.Statements;
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
        public void SimpleVariableToVariableAssignment()
        {
            CompleteInMethod(@"
                int i, j;
                i = 1;
                j = i;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                VarDecl("j", Fix.Int),
                Assign("i", new ConstantValueExpression()),
                Assign("j", new UnknownExpression()),
                // TODO fix this (ComposedExpression!)
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Assigning_Literals()
        {
            Assert.Fail();
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
                Assign("o", new UnknownExpression()),
                // TODO fix this (ComposedExpression!)
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Assigning_Variables()
        {
            Assert.Fail();
        }

        [Test]
        public void Assigning_Events()
        {
            Assert.Fail();
        }

        [Test]
        public void Assigning_Properties()
        {
            Assert.Fail();
        }

        [Test]
        public void Assigning_Fields()
        {
            Assert.Fail();
        }

        [Test]
        public void Assigning_InvocationExpressions()
        {
            Assert.Fail();
        }

        [Test]
        public void Assigning_Composed()
        {
            // TODO create separate test suite for this kind of expressions
            Assert.Fail();
        }

        [Test]
        public void Fancy1()
        {
            CompleteInMethod(@"
                int i = 1, j = 2;
                $
            ");

            Assert.Fail();
        }

        [Test]
        public void Fancy2()
        {
            CompleteInMethod(@"
                int i = 1;
                var j = (i = 2);
                $
            ");

            Assert.Fail();
        }

        [Test]
        public void Event_AddingListener_Lambda()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E += i => { };
                    $
                }
            ");

            AssertBody(
                new Assignment
                {
                    Reference = EventRef("E", Fix.ActionOfInt),
                    Expression = new UnknownExpression(), // TODO: LambdaExpression
                    Kind = AssignmentType.Add
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Event_AddingListener_DefaultDelegate()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E += delegate { };
                    $
                }
            ");

            AssertBody(
                new Assignment
                {
                    Reference = EventRef("E", Fix.ActionOfInt),
                    Expression = new UnknownExpression(), // TODO: LambdaExpression
                    Kind = AssignmentType.Add
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Event_AddingListener_Method()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E += Listener;
                    $
                }
                private void Listener(int i) {}
            ");

            AssertBody(
                new Assignment
                {
                    Reference = EventRef("E", Fix.ActionOfInt),
                    Expression = new UnknownExpression(), // TODO: RefExpr(MethodReference)
                    Kind = AssignmentType.Add
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Event_RemovingListener_Lambda()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E -= i => { };
                    $
                }
            ");

            AssertBody(
                new Assignment
                {
                    Reference = EventRef("E", Fix.ActionOfInt),
                    Expression = new UnknownExpression(), // TODO: LambdaExpression
                    Kind = AssignmentType.Add
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Event_RemovingListener_DefaultDelegate()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E -= delegate { };
                    $
                }
            ");

            AssertBody(
                new Assignment
                {
                    Reference = EventRef("E", Fix.ActionOfInt),
                    Expression = new UnknownExpression(), // TODO: LambdaExpression
                    Kind = AssignmentType.Add
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Event_RemovingListener_Method()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E -= Listener;
                    $
                }
                private void Listener(int i) {}
            ");

            AssertBody(
                new Assignment
                {
                    Reference = EventRef("E", Fix.ActionOfInt),
                    Expression = new UnknownExpression(), // TODO: RefExpr(MethodReference)
                    Kind = AssignmentType.Add
                },
                ExprStmt(new CompletionExpression()));
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
                Assign("i", new ConstantValueExpression()),
                new Assignment
                {
                    Reference = VarRef("i"),
                    Kind = AssignmentType.Add,
                    Expression = new ConstantValueExpression()
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
                Assign("i", new ConstantValueExpression()),
                new Assignment
                {
                    Reference = VarRef("i"),
                    Kind = AssignmentType.Remove,
                    Expression = new ConstantValueExpression()
                },
                Fix.EmptyCompletion);
        }
    }
}