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
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Expressions
{
    internal class TypeCheckExpressionAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void Is_Standard()
        {
            CompleteInMethod(@"var a = this is Exception; $");

            AssertBody(
                VarDecl("a", Fix.Bool),
                Assign("a", new TypeCheckExpression {Reference = VarRef("this"), Type = Fix.Exception}),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Is_FullQualified()
        {
            CompleteInMethod(@"var a = this is System.Exception; $");

            AssertBody(
                VarDecl("a", Fix.Bool),
                Assign("a", new TypeCheckExpression {Reference = VarRef("this"), Type = Fix.Exception}),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Is_Alias()
        {
            CompleteInMethod(@"var a = this is int; $");

            AssertBody(
                VarDecl("a", Fix.Bool),
                Assign("a", new TypeCheckExpression {Reference = VarRef("this"), Type = Fix.Int}),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Is_MethodResult()
        {
            CompleteInClass(@"
                public void M() 
                {
                    var a = GetHashCode() is int;
                    $
                }");

            AssertBody(
                "M",
                VarDecl("a", Fix.Bool),
                VarDecl("$0", Fix.Int),
                Assign("$0", Invoke("this", Fix.Object_GetHashCode)),
                Assign("a", new TypeCheckExpression {Reference = VarRef("$0"), Type = Fix.Int}),
                Fix.EmptyCompletion);
        }
    }
}