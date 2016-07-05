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

using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.Statements;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite
{
    internal class ResolvingFullyResolvedTypesTest : BaseSSTAnalysisTest
    {
        public readonly ITypeName TypeNr = TypeName.Get("N.R, TestProject");

        [Test]
        public void FullyQualifiedFieldAccess_BuiltInType()
        {
            CompleteInMethod(@"
                var s = string.Empty;
                var f = float.MinValue;
                var d = double.MinValue;
                var i = int.MinValue;
                var b = byte.MinValue;
                var b2 = bool.FalseString;
                $ 
            ");

            AssertBody(
                VarDecl("s", Fix.String),
                Assign(
                    "s",
                    RefExpr(
                        new FieldReference
                        {
                            FieldName = FieldName.Get("static [{0}] [{0}].Empty", Fix.String)
                        })),
                VarDecl("f", Fix.Float),
                Assign(
                    "f",
                    RefExpr(
                        new FieldReference
                        {
                            FieldName = FieldName.Get("static [{0}] [{0}].MinValue", Fix.Float)
                        })),
                VarDecl("d", Fix.Double),
                Assign(
                    "d",
                    RefExpr(
                        new FieldReference
                        {
                            FieldName = FieldName.Get("static [{0}] [{0}].MinValue", Fix.Double)
                        })),
                VarDecl("i", Fix.Int),
                Assign(
                    "i",
                    RefExpr(
                        new FieldReference
                        {
                            FieldName = FieldName.Get("static [{0}] [{0}].MinValue", Fix.Int)
                        })),
                VarDecl("b", Fix.Byte),
                Assign(
                    "b",
                    RefExpr(
                        new FieldReference
                        {
                            FieldName = FieldName.Get("static [{0}] [{0}].MinValue", Fix.Byte)
                        })),
                VarDecl("b2", Fix.String),
                Assign(
                    "b2",
                    RefExpr(
                        new FieldReference
                        {
                            FieldName =
                                FieldName.Get("static [{0}] [{1}].FalseString", Fix.String, Fix.Bool)
                        })),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void FullyQualifiedFieldAccess()
        {
            CompleteInTestEnv(@"
                var x = SomeNamespace.SomeClass.F;
                $
            ");

            AssertBody(
                VarDecl("x", Fix.Int),
                Assign(
                    "x",
                    RefExpr(
                        new FieldReference
                        {
                            FieldName = FieldName.Get("static [{0}] [{1}].F", Fix.Int, SomeClass)
                        })),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void FullyQualifiedEventAccess()
        {
            CompleteInTestEnv(@"
                SomeNamespace.SomeClass.E += () => { };
                $
            ");

            AssertBody(
                new EventSubscriptionStatement
                {
                    Reference = new EventReference
                    {
                        EventName = EventName.Get("static [{0}] [{1}].E", Fix.Action, SomeClass)
                    },
                    Operation = EventSubscriptionOperation.Add,
                    Expression = new LambdaExpression
                    {
                        Name = LambdaName.Get("[{0}] ()", Fix.Void)
                    }
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void RegularFieldAccess()
        {
            CompleteInTestEnv(@"
                var x = new R().F.F;
                $
            ");

            AssertBody(
                VarDecl("x", TypeNr),
                VarDecl("$0", TypeNr),
                Assign("$0", InvokeCtor(MethodName.Get("[{0}] [{1}]..ctor()", Fix.Void, TypeNr))),
                VarDecl("$1", TypeNr),
                Assign(
                    "$1",
                    RefExpr(
                        new FieldReference
                        {
                            Reference = VarRef("$0"),
                            FieldName = FieldName.Get("[{0}] [{0}].F", TypeNr)
                        })),
                Assign(
                    "x",
                    RefExpr(
                        new FieldReference
                        {
                            Reference = VarRef("$1"),
                            FieldName = FieldName.Get("[{0}] [{0}].F", TypeNr)
                        })),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void FullyQualifiedPropertyAccess()
        {
            CompleteInTestEnv(@"
                var x = SomeNamespace.SomeClass.P;
                $
            ");

            AssertBody(
                VarDecl("x", Fix.Int),
                Assign(
                    "x",
                    RefExpr(
                        new PropertyReference
                        {
                            PropertyName =
                                PropertyName.Get("set get static [{0}] [{1}].P()", Fix.Int, SomeClass)
                        })),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void FullyQualifiedMethodInvocation()
        {
            CompleteInTestEnv(@"
                SomeNamespace.SomeClass.M();
                $
            ");

            AssertBody(
                InvokeStaticStmt(MethodName.Get("static [{0}] [{1}].M()", Fix.Void, SomeClass)),
                ExprStmt(new CompletionExpression()));
        }

        public ITypeName SomeClass
        {
            get { return TypeName.Get("SomeNamespace.SomeClass, TestProject"); }
        }

        private void CompleteInTestEnv(string s)
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    internal class C
                    {
                        public void M()
                        {
                            " + s + @"
                        }
                    }

                    internal class R
                    {
                        public R F;
                    }
                }

                namespace SomeNamespace
                {
                    internal class SomeClass
                    {
                        public static event Action E;

                        public static int F = 1;

                        public static int P { get; set; }

                        public static void M()
                        {
                        }
                    }
                }");
        }
    }
}