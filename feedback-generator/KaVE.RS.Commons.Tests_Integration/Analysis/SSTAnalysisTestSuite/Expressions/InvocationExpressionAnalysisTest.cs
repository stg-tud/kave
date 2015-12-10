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
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.References;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Expressions
{
    internal class InvocationExpressionAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void InvocationOnVariable()
        {
            CompleteInMethod(@"
                var o = new object();
                o.GetHashCode();
                $
            ");

            AssertBody(
                VarDecl("o", Fix.Object),
                Assign("o", InvokeCtor(Fix.Object_Init)),
                InvokeStmt("o", Fix.Object_GetHashCode),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationOnVariable_ImplicitThis()
        {
            CompleteInMethod(@"
                GetHashCode();
                $
            ");

            AssertBody(
                InvokeStmt("this", Fix.Object_GetHashCode),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationOnVariable_ExplicitThis()
        {
            CompleteInMethod(@"
                this.GetHashCode();
                $
            ");

            AssertBody(
                InvokeStmt("this", Fix.Object_GetHashCode),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationOnVariable_Unresolved()
        {
            CompleteInMethod(@"
                o.GetHashCode();
                $
            ");

            AssertBody(
                ExprStmt(
                    new InvocationExpression
                    {
                        Reference = new VariableReference(),
                        MethodName = MethodName.UnknownName
                    }),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationOnField()
        {
            CompleteInClass(@"
                private int i;
                public void M(C c) {
                    c.i.GetHashCode();
                    $
                }
            ");

            AssertBody(
                VarDecl("$0", Fix.Int),
                Assign("$0", RefExpr(FieldRef("i", Fix.Int, "c"))),
                InvokeStmt("$0", Fix.GetHashCode(Fix.Int)),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationOnField_ExplicitThis()
        {
            CompleteInClass(@"
                private int i;
                public void M() {
                    this.i.GetHashCode();
                    $
                }
            ");

            AssertBody(
                VarDecl("$0", Fix.Int),
                Assign("$0", RefExpr(FieldRef("i", Fix.Int))),
                InvokeStmt("$0", Fix.GetHashCode(Fix.Int)),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationOnField_ImplicitThis()
        {
            CompleteInClass(@"
                private int i;
                public void M() {
                    i.GetHashCode();
                    $
                }
            ");

            AssertBody(
                VarDecl("$0", Fix.Int),
                Assign("$0", RefExpr(FieldRef("i", Fix.Int))),
                InvokeStmt("$0", Fix.GetHashCode(Fix.Int)),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationOnField_Base()
        {
            CompleteInCSharpFile(@"
                namespace N {
                    class C {
                        protected int i;
                    }
                    class S : C {
                        protected int i;
                        public void M() {
                            base.i.GetHashCode();
                            $
                        }
                    }
                }
            ");

            AssertBody(
                VarDecl("$0", Fix.Int),
                Assign("$0", RefExpr(FieldRef("i", Fix.Int, "base"))),
                InvokeStmt("$0", Fix.GetHashCode(Fix.Int)),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationOnField_Unresolved()
        {
            CompleteInClass(@"
                public void M(C c) {
                    c.u.GetHashCode();
                    $
                }
            ");

            AssertBody(
                ExprStmt(
                    new InvocationExpression
                    {
                        Reference = new VariableReference(),
                        MethodName = MethodName.UnknownName
                    }),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationOnProperty()
        {
            CompleteInClass(@"
                private int I { get; set; }
                public void M(C c) {
                    c.I.GetHashCode();
                    $
                }
            ");

            AssertBody(
                VarDecl("$0", Fix.Int),
                Assign("$0", RefExpr(PropRef("I", Fix.Int, "c"))),
                InvokeStmt("$0", Fix.GetHashCode(Fix.Int)),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationOnProperty_ExplicitThis()
        {
            CompleteInClass(@"
                private int I { get; set; }
                public void M() {
                    this.I.GetHashCode();
                    $
                }
            ");

            AssertBody(
                VarDecl("$0", Fix.Int),
                Assign("$0", RefExpr(PropRef("I", Fix.Int))),
                InvokeStmt("$0", Fix.GetHashCode(Fix.Int)),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationOnProperty_ImplicitThis()
        {
            CompleteInClass(@"
                private int I { get; set; }
                public void M() {
                    I.GetHashCode();
                    $
                }
            ");

            AssertBody(
                VarDecl("$0", Fix.Int),
                Assign("$0", RefExpr(PropRef("I", Fix.Int))),
                InvokeStmt("$0", Fix.GetHashCode(Fix.Int)),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationOnProperty_Base()
        {
            CompleteInCSharpFile(@"
                namespace N {
                    class C {
                        protected int I { get; set; }
                    }
                    class S : C {
                        protected int i;
                        public void M() {
                            base.I.GetHashCode();
                            $
                        }
                    }
                }
            ");

            AssertBody(
                VarDecl("$0", Fix.Int),
                Assign("$0", RefExpr(PropRef("I", Fix.Int, "base"))),
                InvokeStmt("$0", Fix.GetHashCode(Fix.Int)),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationOnProperty_Unresolved()
        {
            CompleteInClass(@"
                public void M(C c) {
                    c.U.GetHashCode();
                    $
                }
            ");

            AssertBody(
                ExprStmt(
                    new InvocationExpression
                    {
                        Reference = new VariableReference(),
                        MethodName = MethodName.UnknownName
                    }),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void StaticInvocation_Implicit()
        {
            CompleteInMethod(@"
                Equals(null, null);
                $
            ");

            AssertBody(
                InvokeStaticStmt(Fix.Object_static_Equals, Const("null"), Const("null")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void StaticInvocation_Explicit()
        {
            CompleteInMethod(@"
                object.Equals(null, null);
                $
            ");

            AssertBody(
                InvokeStaticStmt(Fix.Object_static_Equals, Const("null"), Const("null")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void StaticInvocation_Unresolved()
        {
            CompleteInMethod(@"
                UnknownType.Equals(null, null);
                $
            ");

            AssertBody(
                ExprStmt(
                    new InvocationExpression
                    {
                        Reference = new VariableReference(),
                        MethodName = MethodName.UnknownName,
                        Parameters =
                        {
                            Const("null"),
                            Const("null")
                        }
                    }),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void NestedInOtherCalls()
        {
            CompleteInMethod(@"
                Equals(GetType());
                $
            ");

            AssertBody(
                VarDecl("$0", Fix.Type),
                Assign("$0", Invoke("this", Fix.Object_GetType)),
                InvokeStmt("this", Fix.Object_Equals, RefExpr("$0")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationOnInvocation()
        {
            CompleteInMethod(@"
                ToString().GetHashCode();
                $
            ");

            AssertBody(
                VarDecl("$0", Fix.String),
                Assign("$0", Invoke("this", Fix.ToString(Fix.Object))),
                InvokeStmt("$0", Fix.GetHashCode(Fix.String)),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationOnInvocation_Chain()
        {
            CompleteInMethod(@"
                ToString().GetHashCode().ToString();
                $
            ");

            AssertBody(
                VarDecl("$0", Fix.String),
                Assign("$0", Invoke("this", Fix.ToString(Fix.Object))),
                VarDecl("$1", Fix.Int),
                Assign("$1", Invoke("$0", Fix.GetHashCode(Fix.String))),
                InvokeStmt("$1", Fix.ToString(Fix.Int)),
                ExprStmt(new CompletionExpression()));
        }


        [Test]
        public void InvocationLambda()
        {
            CompleteInClass(@"
                private Action<int> _a;
                public void M()
                {
                    _a.Invoke(1);
                    $
                }
            ");

            AssertBody(
                "M",
                VarDecl("$0", Fix.ActionOfInt),
                Assign("$0", RefExpr(FieldRef("_a", Fix.ActionOfInt))),
                InvokeStmt("$0", Fix.Action_Invoke, Const("1")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationLambda_SyntacticSugar()
        {
            CompleteInClass(@"
                private Action<int> _a;
                public void M()
                {
                    _a(1);
                    $
                }
            ");

            AssertBody(
                "M",
                VarDecl("$0", Fix.ActionOfInt),
                Assign("$0", RefExpr(FieldRef("_a", Fix.ActionOfInt))),
                InvokeStmt("$0", Fix.Action_Invoke, Const("1")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void InvocationLambda_Local()
        {
            CompleteInMethod(@"
                Action<int> a = null;
                a(1);
                $
            ");

            AssertBody(
                VarDecl("a", Fix.ActionOfInt),
                Assign("a", Const("null")),
                InvokeStmt("a", Fix.Action_Invoke, Const("1")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void ExtensionMethod()
        {
            CompleteWithExtensionMethod(@"
                var i = 1;
                i.M0();
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", Const("1")),
                InvokeStaticStmt(
                    MethodName.Get("static [{0}] [N.H, TestProject].M0([{1}] i)", Fix.Void, Fix.Int),
                    RefExpr("i")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void ExtensionMethod_OnLiteral()
        {
            CompleteWithExtensionMethod(@"
                1.M0();
                $
            ");

            AssertBody(
                VarDecl("$0", Fix.Int),
                Assign("$0", Const("1")),
                InvokeStaticStmt(
                    MethodName.Get("static [{0}] [N.H, TestProject].M0([{1}] i)", Fix.Void, Fix.Int),
                    RefExpr("$0")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void ExtensionMethod_OnThis()
        {
            CompleteWithExtensionMethod(@"
                this.MT();
                $
            ");

            AssertBody(
                InvokeStaticStmt(
                    MethodName.Get("static [{0}] [N.H, TestProject].MT([{1}] o)", Fix.Void, Fix.Object),
                    RefExpr("this")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void ExtensionMethod_static()
        {
            CompleteWithExtensionMethod(@"
                H.M0(1);
                $
            ");

            AssertBody(
                InvokeStaticStmt(
                    MethodName.Get("static [{0}] [N.H, TestProject].M0([{1}] i)", Fix.Void, Fix.Int),
                    Const("1")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void ExtensionMethod_WithParam()
        {
            CompleteWithExtensionMethod(@"
                1.M1(0);
                $
            ");

            AssertBody(
                VarDecl("$0", Fix.Int),
                Assign("$0", Const("1")),
                InvokeStaticStmt(
                    MethodName.Get("static [{0}] [N.H, TestProject].M1([{1}] i, [{1}] j)", Fix.Void, Fix.Int),
                    RefExpr("$0"),
                    Const("0")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void ExtensionMethod_WithTwoParams()
        {
            CompleteWithExtensionMethod(@"
                1.M2(0, -1);
                $
            ");

            AssertBody(
                VarDecl("$0", Fix.Int),
                Assign("$0", Const("1")),
                InvokeStaticStmt(
                    MethodName.Get("static [{0}] [N.H, TestProject].M2([{1}] i, [{1}] j, [{1}] k)", Fix.Void, Fix.Int),
                    RefExpr("$0"),
                    Const("0"),
                    Const("-1")),
                ExprStmt(new CompletionExpression()));
        }

        private void CompleteWithExtensionMethod(string body)
        {
            CompleteInCSharpFile(@"
                namespace N {
                    class C {
                        public override int GetHashCode() {
                            " + body + @"
                        }
                    }
                    static class H {
                        public static void MT(this object o) {}
                        public static void M0(this int i) {}
                        public static void M1(this int i, int j) {}
                        public static void M2(this int i, int j, int k) {}
                    }
                }
            ");
        }
    }
}