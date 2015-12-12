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
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Expressions
{
    internal class ObjectCreationExpressionAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void UnresolvedMethod()
        {
            CompleteWithInitializer(@"
                var t = new XYZ();
                $
            ");

            AssertBody(
                VarDecl("t", TypeName.UnknownName),
                Assign("t", new InvocationExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void InAssign_RegularConstructor()
        {
            CompleteWithInitializer(@"
                var t = new T();
                $
            ");

            AssertBody(
                VarDecl("t", TypeName.Get("N.T, TestProject")),
                Assign("t", InvokeCtor(MethodName.Get("[{0}] [N.T, TestProject]..ctor()", Fix.Void))),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Nested_RegularConstructor()
        {
            CompleteWithInitializer(@"
                Equals(new T());
                $
            ");

            AssertBody(
                VarDecl("$0", TypeName.Get("N.T, TestProject")),
                Assign("$0", InvokeCtor(MethodName.Get("[{0}] [N.T, TestProject]..ctor()", Fix.Void))),
                ExprStmt(Invoke("this", Fix.Object_Equals, RefExpr("$0"))),
                Fix.EmptyCompletion);
        }

        [Test]
        public void InAssign_ObjectInitializer()
        {
            CompleteWithInitializer(@"
                var t = new T
                {
                    P = 1
                };
                $
            ");

            AssertBody(
                VarDecl("t", TypeName.Get("N.T, TestProject")),
                Assign("t", InvokeCtor(MethodName.Get("[{0}] [N.T, TestProject]..ctor()", Fix.Void))),
                AssignP("t", Const("1")),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Nested_ObjectInitializer()
        {
            CompleteWithInitializer(@"
                Equals(new T
                {
                    P = 1
                });
                $
            ");
            AssertBody(
                VarDecl("$0", TypeName.Get("N.T, TestProject")),
                Assign("$0", InvokeCtor(MethodName.Get("[{0}] [N.T, TestProject]..ctor()", Fix.Void))),
                AssignP("$0", Const("1")),
                ExprStmt(Invoke("this", Fix.Object_Equals, RefExpr("$0"))),
                Fix.EmptyCompletion);
        }

        [Test]
        public void InAssign_CollectionInitializer()
        {
            CompleteWithInitializer(@"
                var t = new List<int> {1};
                $
            ");

            AssertBody(
                VarDecl("t", Fix.ListOfInt),
                Assign("t", InvokeCtor(Fix.ListOfInt_Init)),
                new ExpressionStatement {Expression = Invoke("t", Fix.ListOfInt_Add, Const("1"))},
                Fix.EmptyCompletion);
        }

        [Test]
        public void InAssign_NestedInitializers()
        {
            CompleteWithInitializer(@"
                var t = new T
                {
                    P = new T
                    {
                        P = 2
                    }
                };
                $
            ");

            AssertBody(
                VarDecl("t", TypeName.Get("N.T, TestProject")),
                Assign("t", InvokeCtor(MethodName.Get("[{0}] [N.T, TestProject]..ctor()", Fix.Void))),
                VarDecl("$0", TypeName.Get("N.T, TestProject")),
                Assign("$0", InvokeCtor(MethodName.Get("[{0}] [N.T, TestProject]..ctor()", Fix.Void))),
                AssignP("$0", Const("1")),
                AssignP("t", RefExpr("$0")),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Nested_NestedInitializers()
        {
            CompleteWithInitializer(@"
                Equals(new T
                {
                    P = new T
                    {
                        P = 1
                    }
                });
                $
            ");

            AssertBody(
                VarDecl("$0", TypeName.Get("N.T, TestProject")),
                Assign("$0", InvokeCtor(MethodName.Get("[{0}] [N.T, TestProject]..ctor()", Fix.Void))),
                VarDecl("$1", TypeName.Get("N.T, TestProject")),
                Assign("$1", InvokeCtor(MethodName.Get("[{0}] [N.T, TestProject]..ctor()", Fix.Void))),
                AssignP("$1", Const("1")),
                AssignP("$0", RefExpr("$1")),
                ExprStmt(Invoke("this", Fix.Object_Equals, RefExpr("$0"))),
                Fix.EmptyCompletion);
        }

        [Test]
        public void InAssign_NestedCollectionInit()
        {
            CompleteWithInitializer(@"
                var t = new T
                {
                    CP = {1}
                };
                $
            ");

            AssertBody(
                VarDecl("$0", TypeName.Get("N.T, TestProject")),
                Assign("$0", InvokeCtor(MethodName.Get("[{0}] [N.T, TestProject]..ctor()", Fix.Void))),
                VarDecl("$1", TypeName.Get("N.T, TestProject")),
                Assign("$1", InvokeCtor(MethodName.Get("[{0}] [N.T, TestProject]..ctor()", Fix.Void))),
                AssignP("$1", Const("0")),
                AssignP("$0", RefExpr("$1")),
                ExprStmt(Invoke("this", Fix.Object_Equals, RefExpr("$0"))),
                Fix.EmptyCompletion);
        }

        [Test]
        public void InAssign_Complex()
        {
            CompleteWithInitializer(@"
                var t = new T
                {
                    P = new T(),
                    CP = {1, 2, 3}
                };
                $
            ");

            AssertBody(
                VarDecl("t", TypeName.Get("N.T, TestProject")),
                Assign("t", InvokeCtor(MethodName.Get("[{0}] [N.T, TestProject]..ctor()", Fix.Void))),
                Fix.EmptyCompletion,
                Fix.EmptyCompletion);
        }

        [Test]
        public void Nested_CollectionInitializer()
        {
            CompleteWithInitializer(@"
                Equals(new List<int> {1});
                $
            ");

            AssertBody(
                VarDecl("$0", Fix.ListOfInt),
                Assign("$0", InvokeCtor(Fix.ListOfInt_Init)),
                new ExpressionStatement {Expression = Invoke("$0", Fix.ListOfInt_Add, Const("1"))},
                ExprStmt(Invoke("this", Fix.Object_Equals, RefExpr("$0"))),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Nested_NestedCollectionInit()
        {
            CompleteWithInitializer(@"
                Equals(new T
                {
                    CP = {1}
                });
                $
            ");

            AssertBody(
                VarDecl("$0", TypeName.Get("N.T, TestProject")),
                Assign("$0", InvokeCtor(MethodName.Get("[{0}] [N.T, TestProject]..ctor()", Fix.Void))),
                VarDecl("$1", Fix.ListOfObject),
                Assign(
                    "$1",
                    RefExpr(
                        new PropertyReference
                        {
                            Reference = VarRef("$0"),
                            PropertyName = PropertyName.Get("set get [{0}] [N.T, TestProject].CP()", Fix.ListOfObject)
                        })),
                InvokeStmt("$1", Fix.ListOfObject_Add, Const("1")),
                ExprStmt(Invoke("this", Fix.Object_Equals, RefExpr("$0"))),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Nested_Complex()
        {
            CompleteWithInitializer(@"
                Equals(new T
                {
                    P = new T {
                        P = 0
                    },
                    CP = {1}
                });
                $
            ");

            AssertBody(
                VarDecl("$0", TypeName.Get("N.T, TestProject")),
                Assign("$0", InvokeCtor(MethodName.Get("[{0}] [N.T, TestProject]..ctor()", Fix.Void))),
                VarDecl("$1", TypeName.Get("N.T, TestProject")),
                Assign("$1", InvokeCtor(MethodName.Get("[{0}] [N.T, TestProject]..ctor()", Fix.Void))),
                AssignP("$1", Const("0")),
                AssignP("$0", RefExpr("$1")),
                VarDecl("$2", Fix.ListOfObject),
                Assign(
                    "$2",
                    new ReferenceExpression
                    {
                        Reference = new PropertyReference
                        {
                            Reference = VarRef("$0"),
                            PropertyName = PropertyName.Get("set get [{0}] [N.T, TestProject].CP()", Fix.ListOfObject)
                        }
                    }),
                InvokeStmt("$2", Fix.ListOfObject_Add, Const("1")),
                ExprStmt(Invoke("this", Fix.Object_Equals, RefExpr("$0"))),
                Fix.EmptyCompletion);
        }

        private static Assignment AssignP(string varId, ISimpleExpression expr)
        {
            return new Assignment
            {
                Reference = new PropertyReference
                {
                    Reference = VarRef(varId),
                    PropertyName = PropertyName.Get("set get [{0}] [N.T, TestProject].P()", Fix.Object)
                },
                Expression = expr
            };
        }

        private void CompleteWithInitializer(string body)
        {
            CompleteInCSharpFile(@"
                namespace N {
                    class C
                    {
                        public void M()
                        {
                            " + body + @"
                        }
                    }

                    class T
                    {
                        public object P { get; set; }
                        public List<object> CP { get; set; }
                    }
                }
            ");
        }
    }
}