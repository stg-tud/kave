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
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite
{
    internal class InvocationExpressionTest : BaseSSTAnalysisTest
    {
        [Test]
        public void Invocation()
        {
            CompleteInMethod(@"
                object o = null;
                o.GetHashCode();
                $
            ");

            AssertBody(
                VarDecl("o", Fix.Object),
                Assign("o", new NullExpression()),
                InvokeStmt("o", Fix.Object_GetHashCode),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Invocation_Static()
        {
            CompleteInMethod(@"
                object.Equals(null, null);
                $
            ");

            AssertBody(
                InvokeStaticStmt(Fix.Object_static_Equals, new NullExpression(), new NullExpression()),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Invocation_Constructor()
        {
            CompleteInMethod(@"
                new object();
                $
            ");

            AssertBody(
                ExprStmt(InvokeCtor(Fix.Object_ctor)),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Invocation_ConstructorWithParams()
        {
            CompleteInMethod(@"
                new Exception(""..."");
                $
            ");

            AssertBody(
                ExprStmt(InvokeCtor(Fix.Exception_ctor, new ConstantValueExpression())),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Assignment_Constructor()
        {
            CompleteInMethod(@"
                var o = new object();
                $
            ");

            AssertBody(
                VarDecl("o", Fix.Object),
                Assign("o", InvokeCtor(Fix.Object_ctor)),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Assignment_Invocation()
        {
            CompleteInMethod(@"
                int j = this.GetHashCode();
                $
            ");

            AssertBody(
                VarDecl("j", Fix.Int),
                Assign("j", Invoke("this", Fix.Object_GetHashCode)),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Assignment_InvocationStatic()
        {
            CompleteInMethod(@"
                var b = object.Equals(null, null);
                $
            ");

            AssertBody(
                VarDecl("b", Fix.Bool),
                Assign(
                    "b",
                    InvokeStatic(Fix.Object_static_Equals, new NullExpression(), new NullExpression())),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void This_Implicitly()
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
        public void This_Explicitly()
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
        public void Base_Explicitly()
        {
            CompleteInMethod(@"
                base.GetHashCode(); // invalid code
                $
            ");

            AssertBody(
                InvokeStmt("base", Fix.Object_GetHashCode),
                ExprStmt(new CompletionExpression()));
        }

        // TODO @seb: the following tests mix "invocation in hierarchy" and "this/base", split the tests into both concerns

        [Test, Ignore]
        public void This_Hierarchy()
        {
            CompleteInCSharpFile(@"
                namespace N {
                    public interface I {
                        void A();
                        void B();
                    }
                    public class C : I {
                        public void A()
                        {
                            B();
                            $
                        }
                        public void B() {}
                    }
                }
            ");

            // TODO @seb: implement test
            Assert.Fail();
        }

        [Test, Ignore]
        public void Base_Overridden()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    public class D
                    {
                        public virtual void B() {}
                    }
                    public class C : D
                    {
                        public void A()
                        {
                            base.B();
                            $
                        }
                        public override void B() {}
                    }
                }
            ");

            // TODO @seb: implement test
            Assert.Fail();
        }

        [Test, Ignore]
        public void Base_Shadowed()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    public class D
                    {
                        public void B() {}
                    }
                    public class C : D
                    {
                        public void A()
                        {
                            base.B();
                            $
                        }
                        public void B() {}
                    }
                }
            ");

            // TODO @seb: implement test
            Assert.Fail();
        }
    }
}