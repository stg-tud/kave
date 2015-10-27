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
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Expressions
{
    internal class IndexAccessExpressionAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void OneIndex()
        {
            CompleteInMethod(@"
                int i = arr[1];
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                VarAssign(
                    "i",
                    new IndexAccessExpression
                    {
                        Reference = VarRef("arr"),
                        Indices = {new ConstantValueExpression()}
                    }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void TwoIndices()
        {
            CompleteInMethod(@"
                int i = arr[1, 2];
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                VarAssign(
                    "i",
                    new IndexAccessExpression
                    {
                        Reference = VarRef("arr"),
                        Indices = {new ConstantValueExpression(), new ConstantValueExpression()}
                    }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Jagged()
        {
            // Resharper is unable to infer the type of arr[1] without the previous declaration of arr.
            // Technically this code is invalid since arr is not initialized. 
            CompleteInMethod(@"
                int[][] arr;
                int i = arr[1][2];
                $");

            AssertBody(
                VarDecl("arr", TypeName.Get("System.Int32[,], mscorlib, 4.0.0.0")),
                VarDecl("i", Fix.Int),
                VarDecl("$0", Fix.IntArray),
                VarAssign(
                    "$0",
                    new IndexAccessExpression
                    {
                        Reference = VarRef("arr"),
                        Indices = {new ConstantValueExpression()}
                    }),
                VarAssign(
                    "i",
                    new IndexAccessExpression
                    {
                        Reference = VarRef("$0"),
                        Indices = {new ConstantValueExpression()}
                    }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void VariableAsIndex()
        {
            CompleteInMethod(@"
                int index = 1;
                int i = arr[index];
                $");

            AssertBody(
                VarDecl("index", Fix.Int),
                VarAssign("index", new ConstantValueExpression()),
                VarDecl("i", Fix.Int),
                VarAssign(
                    "i",
                    new IndexAccessExpression
                    {
                        Reference = VarRef("arr"),
                        Indices = {RefExpr(VarRef("index"))}
                    }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void IndexAccessOnMethodResult()
        {
            CompleteInClass(@"        
                public int[] GetArray()
                {
                    return new int[10];
                }

                public void M()
                {
                    int i = this.GetArray()[1];
                    $
                }");

            AssertBody(
                "M",
                VarDecl("i", Fix.Int),
                VarDecl("$0", Fix.IntArray),
                VarAssign("$0", Invoke("this", Fix.Method(Fix.IntArray, Type("C"), "GetArray"))),
                VarAssign(
                    "i",
                    new IndexAccessExpression
                    {
                        Reference = VarRef("$0"),
                        Indices = {new ConstantValueExpression()}
                    }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void AssigningValueToArrayElement()
        {
            CompleteInMethod(@"
                arr[1] = 1;
                $");

            AssertBody(
                Assign(
                    new IndexAccessReference
                    {
                        Expression =
                            new IndexAccessExpression
                            {
                                Reference = VarRef("arr"),
                                Indices = {new ConstantValueExpression()}
                            }
                    },
                    new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void AssigningValueToArrayElement_TwoDimensional()
        {
            CompleteInMethod(@"
                arr[1, 2] = 1;
                $");

            AssertBody(
                Assign(
                    new IndexAccessReference
                    {
                        Expression =
                            new IndexAccessExpression
                            {
                                Reference = VarRef("arr"),
                                Indices = { new ConstantValueExpression(), new ConstantValueExpression() }
                            }
                    },
                    new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void AssigningValueToArrayElement_Jagged()
        {
            CompleteInMethod(@"
                int[][] arr;
                arr[1][2] = 1;
                $");

            AssertBody(
                VarDecl("arr", TypeName.Get("System.Int32[,], mscorlib, 4.0.0.0")),
                VarDecl("$0", Fix.IntArray),
                VarAssign(
                    "$0",
                    new IndexAccessExpression {Reference = VarRef("arr"), Indices = {new ConstantValueExpression()}}),
                Assign(
                    new IndexAccessReference
                    {
                        Expression =
                            new IndexAccessExpression
                            {
                                Reference = VarRef("$0"),
                                Indices = {new ConstantValueExpression()}
                            }
                    },
                    new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }
    }
}