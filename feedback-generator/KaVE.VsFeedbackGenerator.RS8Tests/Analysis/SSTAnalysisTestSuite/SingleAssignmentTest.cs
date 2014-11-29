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
    internal class SingleAssignmentTest : BaseSSTAnalysisTest
    {
        [Test]
        public void InlineIfElse()
        {
            CompleteInClass(@"
                public void A()
                {
                    var i = true ? 1 : 2;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(
                new Assignment(
                    "i",
                    new IfElseExpression
                    {
                        Condition = new ConstantExpression(),
                        ThenExpression = new ConstantExpression(),
                        ElseExpression = new ConstantExpression()
                    }));

            AssertEntryPoints(mA);
        }

        [Test]
        public void ComplexInlineIfElse()
        {
            CompleteInClass(@"
                public void A(object o1, object o2, object o3)
                {
                    var compare = (o2.GetHashCode() > o3.GetHashCode())
                        ? o1.ToString().Equals(o2.ToString())
                        : o1.ToString().Equals(o3.ToString());
                    $
                }
            ");

            var mA = NewMethodDeclaration(
                Fix.Void,
                "A",
                string.Format("[{0}] o1", Fix.Object),
                string.Format("[{0}] o2", Fix.Object),
                string.Format("[{0}] o3", Fix.Object));
            mA.Body.Add(new VariableDeclaration("compare", Fix.Bool));
            var ifBlock = new BlockExpression();// {Value = new[] {"$0", "$1"}};
            ifBlock.Body.Add(new VariableDeclaration("$0", Fix.Int));
            ifBlock.Body.Add(new Assignment("$0", new InvocationExpression("o2", Fix.GetHashCode(Fix.Object))));
            ifBlock.Body.Add(new VariableDeclaration("$1", Fix.Int));
            ifBlock.Body.Add(new Assignment("$1", new InvocationExpression("o3", Fix.GetHashCode(Fix.Object))));
            var thenBlock = new BlockExpression(); //) {Value = new[] {"$4"}};
            thenBlock.Body.Add(new VariableDeclaration("$2", Fix.String));
            thenBlock.Body.Add(new Assignment("$2", new InvocationExpression("o1", Fix.ToString(Fix.Object))));
            thenBlock.Body.Add(new VariableDeclaration("$3", Fix.String));
            thenBlock.Body.Add(new Assignment("$3", new InvocationExpression("o2", Fix.ToString(Fix.Object))));
            thenBlock.Body.Add(new VariableDeclaration("$4", Fix.Bool));
            thenBlock.Body.Add(
                new Assignment(
                    "$4",
                    new InvocationExpression("$2", Fix.Equals(Fix.String, Fix.String, "value"), new[] {"$3"})));
            var elseBlock = new BlockExpression();//) {Value = new[] {"$7"}};
            elseBlock.Body.Add(new VariableDeclaration("$5", Fix.String));
            elseBlock.Body.Add(new Assignment("$5", new InvocationExpression("o1", Fix.ToString(Fix.Object))));
            elseBlock.Body.Add(new VariableDeclaration("$6", Fix.String));
            elseBlock.Body.Add(new Assignment("$6", new InvocationExpression("o3", Fix.ToString(Fix.Object))));
            elseBlock.Body.Add(new VariableDeclaration("$7", Fix.Bool));
            elseBlock.Body.Add(
                new Assignment(
                    "$7",
                    new InvocationExpression("$5", Fix.Equals(Fix.String, Fix.String, "value"), new[] {"$6"})));
            mA.Body.Add(
                new Assignment(
                    "compare",
                    new IfElseExpression {Condition = ifBlock, ThenExpression = thenBlock, ElseExpression = elseBlock}));
            AssertEntryPoints(mA);
        }

        [Test]
        public void Invocation_SimpleComposition()
        {
            CompleteInFile(@"
                namespace N {
                    public class H {
                        public int Get() {
                            return 1;
                        }
                    }
                    public class C {
                        public void A(H h) {
                            var i = 1 + h.Get();
                            $
                        }
                    }
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[N.H, TestProject] h"));

            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new VariableDeclaration("$0", Fix.Int));
            mA.Body.Add(
                new Assignment(
                    "$0",
                    new InvocationExpression(
                        "h",
                        MethodName.Get(string.Format("[{0}] [N.H, TestProject].Get()", Fix.Int)))));
            mA.Body.Add(new Assignment("i", ComposedExpression.Create("$0")));

            AssertEntryPoints(mA);
        }

        [Test]
        public void Invocation_RealComposition()
        {
            CompleteInFile(@"
                namespace N {
                    public class H {
                        public int Get() {
                            return 1;
                        }
                    }
                    public class C {
                        public void A(H h1, H h2) {
                            var i = h1.Get() + h2.Get() + 1;
                            $
                        }
                    }
                }
            ");

            var mA = NewMethodDeclaration(
                Fix.Void,
                "A",
                string.Format("[N.H, TestProject] h1"),
                string.Format("[N.H, TestProject] h2"));

            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new VariableDeclaration("$0", Fix.Int));
            mA.Body.Add(
                new Assignment(
                    "$0",
                    new InvocationExpression(
                        "h1",
                        MethodName.Get(string.Format("[{0}] [N.H, TestProject].Get()", Fix.Int)))));
            mA.Body.Add(new VariableDeclaration("$1", Fix.Int));
            mA.Body.Add(
                new Assignment(
                    "$1",
                    new InvocationExpression(
                        "h2",
                        MethodName.Get(string.Format("[{0}] [N.H, TestProject].Get()", Fix.Int)))));
            mA.Body.Add(new Assignment("i", ComposedExpression.Create("$0", "$1")));

            AssertEntryPoints(mA);
        }

        [Test]
        public void Invocation_NestedComposition()
        {
            CompleteInFile(@"
                namespace N {
                    public class U {
                        public int Plus(int i) {
                            return i + 1;
                        }
                    }
                    public class C {
                        public void A(U u1, U u2) {
                            var i = u1.Plus(u2.Plus(1));
                            $
                        }
                    }
                }
            ");

            var mA = NewMethodDeclaration(
                Fix.Void,
                "A",
                string.Format("[N.U, TestProject] u1"),
                string.Format("[N.U, TestProject] u2"));

            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new VariableDeclaration("$0", Fix.Int));
            mA.Body.Add(new Assignment("$0", new ConstantExpression()));
            mA.Body.Add(new VariableDeclaration("$1", Fix.Int));
            mA.Body.Add(
                new Assignment(
                    "$1",
                    new InvocationExpression(
                        "u2",
                        MethodName.Get(string.Format("[{0}] [N.U, TestProject].Plus([{0}] i)", Fix.Int)),
                        "$0")));
            mA.Body.Add(
                new Assignment(
                    "i",
                    new InvocationExpression(
                        "u1",
                        MethodName.Get(string.Format("[{0}] [N.U, TestProject].Plus([{0}] i)", Fix.Int)),
                        "$1")));

            AssertEntryPoints(mA);
        }

        [Test]
        public void CombinedIs()
        {
            CompleteInClass(@"
                public void A(object o)
                {
                    var i = (o is int) || (1.0 is int);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] o", Fix.Object));
            mA.Body.Add(new VariableDeclaration("i", Fix.Bool));
            mA.Body.Add(new Assignment("i", ComposedExpression.Create("o")));

            AssertEntryPoints(mA);
        }

        [Test]
        public void CombinedAs()
        {
            CompleteInClass(@"
                public void A(object o)
                {
                    var i = (o as string) + (1.0 as string);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] o", Fix.Object));
            mA.Body.Add(new VariableDeclaration("i", Fix.String));
            mA.Body.Add(new Assignment("i", ComposedExpression.Create("o")));

            AssertEntryPoints(mA);
        }

        [Test]
        public void CombinedCast()
        {
            CompleteInClass(@"
                public void A(object o)
                {
                    var i = ((string) o) + ((string) 1.0);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] o", Fix.Object));
            mA.Body.Add(new VariableDeclaration("i", Fix.String));
            mA.Body.Add(new Assignment("i", ComposedExpression.Create("o")));

            AssertEntryPoints(mA);
        }

        [Test]
        public void CombinedAssignment()
        {
            CompleteInClass(@"
                public void A(int i)
                {
                    int j;
                    var sum = (j = i) + i;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] i", Fix.Int));
            mA.Body.Add(new VariableDeclaration("j", Fix.Int));
            mA.Body.Add(new VariableDeclaration("sum", Fix.Int));
            mA.Body.Add(new Assignment("j", ComposedExpression.Create("i")));
            mA.Body.Add(new Assignment("sum", ComposedExpression.Create("j", "i")));

            AssertEntryPoints(mA);
        }

        [Test]
        public void CombinedPostfixPrefix()
        {
            CompleteInClass(@"
                public void A(int i)
                {
                    var j = (i++) + (++i);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] i", Fix.Int));
            mA.Body.Add(new VariableDeclaration("j", Fix.Int));
            mA.Body.Add(new Assignment("j", ComposedExpression.Create("i")));

            AssertEntryPoints(mA);
        }

        [Test]
        public void CombinedUnary()
        {
            CompleteInClass(@"
                public void A(int i)
                {
                    var j = 0 + (-i);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] i", Fix.Int));
            mA.Body.Add(new VariableDeclaration("j", Fix.Int));
            mA.Body.Add(new Assignment("j", ComposedExpression.Create("i")));

            AssertEntryPoints(mA);
        }

        [Test]
        public void CombinedInlineIfElse()
        {
            CompleteInClass(@"
                public void A()
                {
                    var i = (true ? 1 : 2) + 3;
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new VariableDeclaration("$0", Fix.Int));
            mA.Body.Add(
                new Assignment(
                    "$0",
                    new IfElseExpression
                    {
                        Condition = new ConstantExpression(),
                        ThenExpression = new ConstantExpression(),
                        ElseExpression = new ConstantExpression()
                    }));
            mA.Body.Add(new Assignment("i", ComposedExpression.Create("$0")));

            AssertEntryPoints(mA);
        }

        [Test]
        public void CombinedDefault()
        {
            CompleteInClass(@"
                public void A()
                {
                    var i = 0 + default(int);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantExpression()));

            AssertEntryPoints(mA);
        }

        /*
         var c = new C1();
         var a = c.m(o);
         --> assignment(a, (invocation, "C.m", c, [o]))
         */

        /*
             var c = new C1();
             var a = c.m1(o) + C.m2();
             -->
             var c;
             c = new C1();
             var v1;
             v1 = c.m1(o);
             var v2;
             v2 = c.m2()
             var a;
             a = v1 + v2; -> assignment(a, cplxOp(v1, v2))
             */

        /*
             var c = new C1();
             var a = c.m1(o) + j + 1;
             -->
             var c = new C1();
             var v1 = c.m1(o);
             var a = v1 + j + 1; -> assignment(a, cplxOp(v1, j))
             */

        /*
             * var v = true ? 1 : 2;
             * -->
             * var v;
             * if true then
             *   v = 1;
             * else
             *   v = 2
             *
         */
    }
}