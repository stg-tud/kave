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
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantExpression()));

            AssertEntryPoints(mA);
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
                    i = 1;
                    j = i;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new VariableDeclaration("j", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantExpression()));
            mA.Body.Add(new Assignment("j", new ComposedExpression {Variables = new[] {"i"}}));

            AssertEntryPoints(mA);
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
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantExpression()));
            mA.Body.Add(new VariableDeclaration("j", Fix.Int));
            mA.Body.Add(new Assignment("j", new ComposedExpression {Variables = new[] {"i"}}));

            AssertEntryPoints(mA);
        }

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

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("arr", Fix.IntArray));
            mA.Body.Add(new Assignment("arr", new ConstantExpression()));

            AssertEntryPoints(mA);
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

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("n", Fix.Int));
            mA.Body.Add(new Assignment("n", new ConstantExpression()));
            mA.Body.Add(new VariableDeclaration("arr", Fix.IntArray));
            mA.Body.Add(new Assignment("arr", new ComposedExpression {Variables = new[] {"n"}}));

            AssertEntryPoints(mA);
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

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("arr", TypeName.Get("System.Int32[][], mscorlib, 4.0.0.0")));
            mA.Body.Add(new VariableDeclaration("$0", Fix.IntArray));
            mA.Body.Add(new Assignment("$0", new ConstantExpression()));
            mA.Body.Add(new VariableDeclaration("$1", Fix.IntArray));
            mA.Body.Add(new Assignment("$1", new ConstantExpression()));
            mA.Body.Add(new Assignment("arr", ComposedExpression.Create("$0", "$1")));

            AssertEntryPoints(mA);
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

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] o", Fix.Object));
            mA.Body.Add(new VariableDeclaration("arr", Fix.IntArray));
            mA.Body.Add(new VariableDeclaration("$0", Fix.Int));
            mA.Body.Add(new Assignment("$0", new InvocationExpression("o", Fix.GetHashCode(Fix.Object))));
            mA.Body.Add(new Assignment("arr", new ComposedExpression {Variables = new[] {"$0"}}));

            AssertEntryPoints(mA);
        }

        [Test]
        public void Is_Reference()
        {
            CompleteInClass(@"
                public void A(object o)
                {
                    var isInstanceOf = o is string;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] o", Fix.Object));
            mA.Body.Add(new VariableDeclaration("isInstanceOf", Fix.Bool));
            mA.Body.Add(new Assignment("isInstanceOf", new ComposedExpression {Variables = new[] {"o"}}));

            AssertEntryPoints(mA);
        }

        [Test]
        public void Is_Const()
        {
            CompleteInClass(@"
                public void A()
                {
                    var isInstanceOf = 1 is double;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("isInstanceOf", Fix.Bool));
            mA.Body.Add(new Assignment("isInstanceOf", new ConstantExpression()));

            AssertEntryPoints(mA);
        }

        [Test]
        public void As_Reference()
        {
            CompleteInClass(@"
                public void A(object o)
                {
                    var cast = o as string;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] o", Fix.Object));
            mA.Body.Add(new VariableDeclaration("cast", Fix.String));
            mA.Body.Add(new Assignment("cast", new ComposedExpression {Variables = new[] {"o"}}));

            AssertEntryPoints(mA);
        }

        [Test]
        public void As_Const()
        {
            CompleteInClass(@"
                public void A()
                {
                    var cast = 1.0 as object;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("cast", Fix.Object));
            mA.Body.Add(new Assignment("cast", new ConstantExpression()));

            AssertEntryPoints(mA);
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
            mA.Body.Add(new VariableDeclaration("obj", TypeName.Get("N.C, TestProject")));
            mA.Body.Add(new Assignment("obj", ComposedExpression.Create("this")));

            AssertEntryPoints(mA);
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
            mA.Body.Add(new VariableDeclaration("j", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantExpression()));
            mA.Body.Add(new Assignment("j", ComposedExpression.Create("i")));

            AssertEntryPoints(mA);
        }

        [Test]
        public void Cast_Const()
        {
            CompleteInClass(@"
                public void A()
                {
                    var i = (int) 1.0;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantExpression()));

            AssertEntryPoints(mA);
        }

        [Test]
        public void Cast_Reference()
        {
            CompleteInClass(@"
                public void A(object o)
                {
                    var s = (string) o;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] o", Fix.Object));
            mA.Body.Add(new VariableDeclaration("s", Fix.String));
            mA.Body.Add(new Assignment("s", ComposedExpression.Create("o")));

            AssertEntryPoints(mA);
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
            mA.Body.Add(new VariableDeclaration("j", Fix.Int));
            mA.Body.Add(new Assignment("j", ComposedExpression.Create("i")));

            AssertEntryPoints(mA);
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
            mA.Body.Add(new VariableDeclaration("j", Fix.Int));
            mA.Body.Add(new Assignment("j", ComposedExpression.Create("i")));

            AssertEntryPoints(mA);
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
            mA.Body.Add(new VariableDeclaration("j", Fix.Int));
            mA.Body.Add(new Assignment("j", ComposedExpression.Create("i")));

            AssertEntryPoints(mA);
        }

        [Test]
        public void TypeOf()
        {
            CompleteInClass(@"
                public void A()
                {
                    var t = typeof(int);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("t", TypeName.Get("System.Type, mscorlib, 4.0.0.0")));
            mA.Body.Add(new Assignment("t", new ConstantExpression()));

            AssertEntryPoints(mA);
        }

        [Test]
        public void CompositionOfTypeOf()
        {
            CompleteInClass(@"
                public void A()
                {
                    var t = typeof(int) == typeof(string);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("t", Fix.Bool));
            mA.Body.Add(new Assignment("t", new ConstantExpression()));

            AssertEntryPoints(mA);
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
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantExpression()));

            AssertEntryPoints(mA);
        }
    }
}