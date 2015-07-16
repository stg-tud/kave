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

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Expressions
{
    internal class InvocationExpressionAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void RegularCall()
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
        public void ImpliciteThisCall()
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
        public void ExpliciteThisCall()
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
        public void ImpliciteStaticCall()
        {
            CompleteInMethod(@"
                Equals(null, null);
                $
            ");

            AssertBody(
                InvokeStaticStmt(Fix.Object_static_Equals, new NullExpression(), new NullExpression()),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void ExpliciteStaticCall()
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
        public void NestedCalls()
        {
            CompleteInMethod(@"
                Equals(GetType());
                $
            ");

            var t = GetType();

            AssertBody(
                VarDecl("$0", Fix.Type),
                Assign("$0", Invoke("this", Fix.Object_GetType)),
                InvokeStmt("this", Fix.Object_Equals, RefExpr("$0")),
                ExprStmt(new CompletionExpression()));
        }
    }
}