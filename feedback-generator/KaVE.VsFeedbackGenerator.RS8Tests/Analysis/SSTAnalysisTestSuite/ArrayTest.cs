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
 * 
 * Contributors:
 *    - Sebastian Proksch
 */

using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs.Impl.Declarations;
using KaVE.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    internal class ArrayTest : BaseSSTAnalysisTest
    {
        [Test]
        public void ArrayInit_Constant()
        {
            CompleteInClass(@"
                public void A()
                {
                    var arr = new[] {1,2,3};
                    $
                }
            ");

            var mA = NewMethodDeclaration(SSTAnalysisFixture.Void, "A");
            mA.Body.Add(VariableDeclaration.Create("arr", SSTAnalysisFixture.IntArray));
            mA.Body.Add(new Assignment("arr", new ConstantValueExpression()));

            AssertAllMethods(mA);
        }

        [Test]
        public void ArrayInit_Composed()
        {
            CompleteInClass(@"
                public void A()
                {
                    var n = 1;
                    var arr = new[] {1,2,n};
                    $
                }
            ");

            var mA = NewMethodDeclaration(SSTAnalysisFixture.Void, "A");
            mA.Body.Add(VariableDeclaration.Create("n", SSTAnalysisFixture.Int));
            mA.Body.Add(new Assignment("n", new ConstantValueExpression()));
            mA.Body.Add(VariableDeclaration.Create("arr", SSTAnalysisFixture.IntArray));
            mA.Body.Add(new Assignment("arr", ComposedExpression.New("n")));

            AssertAllMethods(mA);
        }

        [Test]
        public void ArrayInit_WithArrays()
        {
            CompleteInClass(@"
                public void A()
                {
                    var arr = new[] {new[] {1, 2}, new[] {3, 4}};
                    $
                }
            ");

            var mA = NewMethodDeclaration(SSTAnalysisFixture.Void, "A");
            mA.Body.Add(VariableDeclaration.Create("arr", TypeName.Get("System.Int32[][], mscorlib, 4.0.0.0")));
            mA.Body.Add(VariableDeclaration.Create("$0", SSTAnalysisFixture.IntArray));
            mA.Body.Add(new Assignment("$0", new ConstantValueExpression()));
            mA.Body.Add(VariableDeclaration.Create("$1", SSTAnalysisFixture.IntArray));
            mA.Body.Add(new Assignment("$1", new ConstantValueExpression()));
            mA.Body.Add(new Assignment("arr", ComposedExpression.New("$0", "$1")));

            AssertAllMethods(mA);
        }

        [Test]
        public void ArrayInit_WithCalls()
        {
            CompleteInClass(@"
                public void A(object o)
                {
                    var arr = new[] {1,2,o.GetHashCode()};
                    $
                }
            ");

            var mA = NewMethodDeclaration(
                SSTAnalysisFixture.Void,
                "A",
                string.Format("[{0}] o", SSTAnalysisFixture.Object));
            mA.Body.Add(VariableDeclaration.Create("arr", SSTAnalysisFixture.IntArray));
            mA.Body.Add(VariableDeclaration.Create("$0", SSTAnalysisFixture.Int));
            mA.Body.Add(new Assignment("$0", InvocationExpression.New("o", Fix.Object_GetHashCode)));
            mA.Body.Add(new Assignment("arr", ComposedExpression.New("$0")));

            AssertAllMethods(mA);
        }
    }
}