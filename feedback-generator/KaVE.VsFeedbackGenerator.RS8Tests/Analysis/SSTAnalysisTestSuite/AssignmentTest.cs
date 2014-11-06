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
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    [TestFixture]
    internal class AssignmentTest : BaseSSTAnalysisTest
    {
        [Test]
        public void SuperSimple()
        {
            CompleteInClass(@"
                public void A()
                {
                    int i;
                    i = 3;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantExpression()));

            AssertEntryPoints(mA);
        }

        [Test]
        public void Simple()
        {
            CompleteInClass(@"
                public void A()
                {
                    int i = 3;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantExpression()));

            AssertEntryPoints(mA);
        }

        [Test, Ignore]
        public void CompositionOfConstants()
        {
            CompleteInClass(@"
                public void A()
                {
                    var i = 5 + 3;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantExpression()));

            AssertEntryPoints(mA);
        }

        [Test, Ignore]
        public void CompositionOfPrimitives()
        {
            CompleteInClass(@"
                public void A()
                {
                    var i = 1
                    var j = i + 1;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantExpression()));
            mA.Body.Add(new VariableDeclaration("j", Fix.Int));
            mA.Body.Add(new Assignment("j", new ComposedExpression {Variables = new[] {"i"}}));

            AssertEntryPoints(mA);
        }

        [Test]
        public void SimpleVariableToVariableAssignment()
        {
            CompleteInClass(@"
                public void A()
                {
                    int i, j;
                    i = 1
                    j = i;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new VariableDeclaration("j", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantExpression()));
            mA.Body.Add(new Assignment("j", new ComposedExpression { Variables = new[] { "i" } }));

            AssertEntryPoints(mA);
        }

        [Test]
        public void VariableToVariableAssignment()
        {
            CompleteInClass(@"
                public void A()
                {
                    var i = 1
                    var j = i;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantExpression()));
            mA.Body.Add(new VariableDeclaration("j", Fix.Int));
            mA.Body.Add(new Assignment("j", new ComposedExpression { Variables = new[] { "i" } }));

            AssertEntryPoints(mA);
        }

        [Test, Ignore]
        public void ArrayInit_Constant()
        {
            CompleteInClass(@"
                public void A()
                {
                    var arr = new[] {1,2,3};
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("arr", Fix.IntArray));
            mA.Body.Add(new Assignment("arr", new ConstantExpression()));

            AssertEntryPoints(mA);
        }

        [Test, Ignore]
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

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("n", Fix.Int));
            mA.Body.Add(new Assignment("n", new ConstantExpression()));
            mA.Body.Add(new VariableDeclaration("arr", Fix.IntArray));
            mA.Body.Add(new Assignment("arr", new ComposedExpression {Variables = new[] {"n"}}));

            AssertEntryPoints(mA);
        }

        [Test, Ignore]
        public void ArrayInit_WithCalls()
        {
            CompleteInClass(@"
                public void A()
                {
                    var o = new Object();
                    var arr = new[] {1,2,o.GetHashCode()};
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("o", Fix.Object));
            mA.Body.Add(new Assignment("n", new InvocationExpression(MethodName.Get("o..ctor"))));
            mA.Body.Add(new VariableDeclaration("o", Fix.Object));
            mA.Body.Add(new Assignment("v0", new InvocationExpression("o", MethodName.Get("Object.GetHashCode"))));
            mA.Body.Add(new VariableDeclaration("arr", Fix.IntArray));
            mA.Body.Add(new Assignment("arr", new ComposedExpression {Variables = new[] {"v0"}}));

            AssertEntryPoints(mA);
        }
    }
}