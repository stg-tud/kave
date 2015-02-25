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
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Expressions.Basic;
using KaVE.Model.SSTs.Impl.Declarations;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
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
            mA.Body.Add(VariableDeclaration.Create("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantValueExpression()));

            AssertAllMethods(mA);
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
            mA.Body.Add(VariableDeclaration.Create("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantValueExpression()));

            AssertAllMethods(mA);
        }

        [Test]
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
            mA.Body.Add(VariableDeclaration.Create("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantValueExpression()));

            AssertAllMethods(mA);
        }

        [Test]
        public void CompositionOfPrimitives()
        {
            CompleteInClass(@"
                public void A()
                {
                    var i = 1;
                    var j = i + 1;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(VariableDeclaration.Create("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantValueExpression()));
            mA.Body.Add(VariableDeclaration.Create("j", Fix.Int));
            mA.Body.Add(new Assignment("j", new ComposedExpression {Variables = new[] {"i"}}));

            AssertAllMethods(mA);
        }

        [Test]
        public void SimpleVariableToVariableAssignment()
        {
            CompleteInClass(@"
                public void A()
                {
                    int i, j;
                    i = 1;
                    j = i;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(VariableDeclaration.Create("i", Fix.Int));
            mA.Body.Add(VariableDeclaration.Create("j", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantValueExpression()));
            mA.Body.Add(new Assignment("j", new ReferenceExpression {Identifier = "i"}));

            AssertAllMethods(mA);
        }

        [Test]
        public void VariableToVariableAssignment()
        {
            CompleteInClass(@"
                public void A()
                {
                    var i = 1;
                    var j = i;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(VariableDeclaration.Create("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantValueExpression()));
            mA.Body.Add(VariableDeclaration.Create("j", Fix.Int));
            mA.Body.Add(new Assignment("j", new ReferenceExpression {Identifier = "i"}));

            AssertAllMethods(mA);
        }


        [Test]
        public void This()
        {
            CompleteInClass(@"
                public void A()
                {
                    var obj = this;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(VariableDeclaration.Create("obj", TypeName.Get("N.C, TestProject")));
            mA.Body.Add(new Assignment("obj", ComposedExpression.Create("this")));

            AssertAllMethods(mA);
        }

        [Test]
        public void AssignmentChain()
        {
            CompleteInClass(@"
                public void A(int i)
                {
                    var j = (i = 0);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] i", Fix.Int));
            mA.Body.Add(VariableDeclaration.Create("j", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantValueExpression()));
            mA.Body.Add(new Assignment("j", ComposedExpression.Create("i")));

            AssertAllMethods(mA);
        }

        [Test]
        public void Postfix()
        {
            CompleteInClass(@"
                public void A(int i)
                {
                    var j = i++;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] i", Fix.Int));
            mA.Body.Add(VariableDeclaration.Create("j", Fix.Int));
            mA.Body.Add(new Assignment("j", ComposedExpression.Create("i")));

            AssertAllMethods(mA);
        }

        [Test]
        public void Prefix()
        {
            CompleteInClass(@"
                public void A(int i)
                {
                    var j = ++i;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] i", Fix.Int));
            mA.Body.Add(VariableDeclaration.Create("j", Fix.Int));
            mA.Body.Add(new Assignment("j", ComposedExpression.Create("i")));

            AssertAllMethods(mA);
        }

        [Test]
        public void Unary()
        {
            CompleteInClass(@"
                public void A(int i)
                {
                    var j = -i;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] i", Fix.Int));
            mA.Body.Add(VariableDeclaration.Create("j", Fix.Int));
            mA.Body.Add(new Assignment("j", ComposedExpression.Create("i")));

            AssertAllMethods(mA);
        }


        [Test]
        public void Default()
        {
            CompleteInClass(@"
                public void A()
                {
                    var i = default(int);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(VariableDeclaration.Create("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantValueExpression()));

            AssertAllMethods(mA);
        }
    }
}