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

using System.Linq;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Expressions
{
    internal class ReferenceExpressionTest : BaseSSTAnalysisTest
    {
        [Test]
        public void SimpleDeclaration()
        {
            CompleteInMethod(@"
                object o = null;
                $
            ");

            AssertBody(
                VarDecl("o", SSTAnalysisFixture.Object),
                Assign("o", new NullExpression()),
                SSTAnalysisFixture.EmptyCompletion);
        }

        [Test]
        public void Reference()
        {
            CompleteInMethod(@"
                object o = null;
                o$
            ");

            AssertBody(
                VarDecl("o", SSTAnalysisFixture.Object),
                Assign("o", new NullExpression()),
                ExprStmt(
                    new CompletionExpression
                    {
                        Token = "o"
                    }));
        }

        [Test]
        public void Reference_WithDot()
        {
            CompleteInMethod(@"
                object o = null;
                o.$
            ");

            AssertBody(
                VarDecl("o", SSTAnalysisFixture.Object),
                Assign("o", new NullExpression()),
                ExprStmt(
                    new CompletionExpression
                    {
                        VariableReference = VarRef("o"),
                        Token = ""
                    }));
        }

        [Test]
        public void Reference_WithQualifier()
        {
            CompleteInMethod(@"
                object o = null;
                o.f$
            ");

            AssertBody(
                VarDecl("o", SSTAnalysisFixture.Object),
                Assign("o", new NullExpression()),
                ExprStmt(
                    new CompletionExpression
                    {
                        VariableReference = VarRef("o"),
                        Token = "f"
                    }));
        }

        public void SetupReferenceExample(string lineWithReference)
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    public class C
                    {
                        public C c;
                        public static C s;
                        public C f;
                        public C P { get { return this; } }
                        public C GetC()
                        {
                            return this;
                        }
                    }

                    internal class C2
                    {
                        private void M(C c)
                        {
                            " + lineWithReference + @"
                        }
                    }
                }
            ");
        }

        public void AssertReferenceExampleBody(params IStatement[] stmts)
        {
            var m = ResultSST.Methods.First();
            Assert.AreEqual(Lists.NewList(stmts), m.Body);
        }

        [Test]
        public void Reference_OnReference()
        {
            SetupReferenceExample("c.c.f$");

            AssertReferenceExampleBody(
                VarDecl("$0", TypeName.Get("N.C, TestProject")),
                Assign(
                    "$0",
                    RefExpr(
                        new FieldReference
                        {
                            Reference = VarRef("c"),
                            FieldName = FieldName.Get(string.Format("[{0}] [{0}].c", TypeName.UnknownName))
                        })),
                ExprStmt(
                    new CompletionExpression
                    {
                        VariableReference = VarRef("$0"),
                        Token = "f"
                    }));
        }

        [Test]
        public void Reference_OnInvocation()
        {
            SetupReferenceExample("c.GetC().f$");

            AssertReferenceExampleBody(
                VarDecl("$0", Type("C")),
                Assign(
                    "$0",
                    Invoke("c", Method("[{0}] [{0}].GetC()", Type("C")))),
                ExprStmt(
                    new CompletionExpression
                    {
                        VariableReference = VarRef("$0"),
                        Token = "f"
                    }));
        }

        [Test]
        public void NestedVariableReferencesExpressions()
        {
            CompleteInMethod(@"
                object o;
                Equals(o, this);
                $
            ");

            AssertBody(
                VarDecl("o", Fix.Object),
                InvokeStaticStmt(Fix.Object_static_Equals, RefExpr("o"), RefExpr("this")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void UnnecessaryNormalization_LocalVariable_ExplicitType()
        {
            CompleteInMethod(@"
                object a = new object();
                var b = a.$
            ");

            AssertBody(
                VarDecl("a", Fix.Object),
                Assign("a", InvokeCtor(Fix.Object_ctor)),
                VarDecl("b", Fix.Unknown),
                Assign(
                    "b",
                    new CompletionExpression
                    {
                        VariableReference = VarRef("a")
                    }));
        }

        [Test]
        public void UnnecessaryNormalization_LocalVariable_ExplicitType_WithToken()
        {
            CompleteInMethod(@"
                object a = new object();
                var b = a.G$
            ");

            AssertBody(
                VarDecl("a", Fix.Object),
                Assign("a", InvokeCtor(Fix.Object_ctor)),
                VarDecl("b", Fix.Unknown),
                Assign(
                    "b",
                    new CompletionExpression
                    {
                        VariableReference = VarRef("a"),
                        Token = "G"
                    }));
        }

        [Test]
        public void UnnecessaryNormalization_LocalVariable_ImplicitType()
        {
            CompleteInMethod(@"
                var a = new object();
                var b = a.$
            ");

            AssertBody(
                VarDecl("a", Fix.Object),
                Assign("a", InvokeCtor(Fix.Object_ctor)),
                VarDecl("b", Fix.Unknown),
                Assign(
                    "b",
                    new CompletionExpression
                    {
                        VariableReference = VarRef("a")
                    }));
        }

        [Test]
        public void UnnecessaryNormalization_LocalVariable_ImplicitType_WithToken()
        {
            CompleteInMethod(@"
                var a = new object();
                var b = a.G$
            ");

            AssertBody(
                VarDecl("a", Fix.Object),
                Assign("a", InvokeCtor(Fix.Object_ctor)),
                VarDecl("b", Fix.Unknown),
                Assign(
                    "b",
                    new CompletionExpression
                    {
                        VariableReference = VarRef("a"),
                        Token = "G"
                    }));
        }

        [Test]
        public void UnnecessaryNormalization_This()
        {
            CompleteInMethod(@"
                this.$
            ");

            AssertBody(
                ExprStmt(
                    new CompletionExpression
                    {
                        VariableReference = VarRef("this")
                    }));
        }

        [Test]
        public void UnnecessaryNormalization_This_WithToken()
        {
            CompleteInMethod(@"
                this.G$
            ");

            AssertBody(
                ExprStmt(
                    new CompletionExpression
                    {
                        VariableReference = VarRef("this"),
                        Token = "G"
                    }));
        }

        [Test]
        public void UnnecessaryNormalization_Base()
        {
            CompleteInMethod(@"
                base.$
            ");

            AssertBody(
                ExprStmt(
                    new CompletionExpression
                    {
                        VariableReference = VarRef("base")
                    }));
        }

        [Test]
        public void UnnecessaryNormalization_Base_WithToken()
        {
            CompleteInMethod(@"
                base.G$
            ");

            AssertBody(
                ExprStmt(
                    new CompletionExpression
                    {
                        VariableReference = VarRef("base"),
                        Token = "G"
                    }));
        }
    }
}