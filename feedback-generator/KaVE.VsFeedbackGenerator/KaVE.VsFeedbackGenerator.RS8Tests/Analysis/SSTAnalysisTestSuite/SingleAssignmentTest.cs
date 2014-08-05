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

using KaVE.Model.SSTs;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    [Ignore]
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
            var ifElse = new IfElse();

            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(ifElse);
            ifElse.Condition = new ConstantExpression();
            ifElse.IfExpressions.Add(new Assignment("i", new ConstantExpression()));
            ifElse.ElseExpressions.Add(new Assignment("i", new ConstantExpression()));

            AssertEntryPoints(mA);
        }

        [Test]
        public void Invocation_SimpleComposition()
        {
            CompleteInFile(@"
                namespace N {
                    public class C {
                        public int get() {
                            return 1;
                        }
                    }
                    public class Test {
                        public void A(C c) {
                            var i = 1 + c.get();
                            $
                        }
                    }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");

            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new VariableDeclaration("v0", Fix.Int));
            mA.Body.Add(new Assignment("v0", new Invocation("c", Fix.GetMethodName("C.get"))));
            mA.Body.Add(new Assignment("i", new ComposedExpression {Variables = new[] {"v0"}}));

            AssertEntryPoints(mA);
        }

        [Test]
        public void Invocation_RealComposition()
        {
            CompleteInFile(@"
                namespace N {
                    public class C {
                        public int get() {
                            return 1;
                        }
                    }
                    public class Test {
                        public void A(C c1, C c2) {
                            var i = c1.get() + c2.get() + 1;
                            $
                        }
                    }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");

            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new VariableDeclaration("v0", Fix.Int));
            mA.Body.Add(new Assignment("v0", new Invocation("c1", Fix.GetMethodName("C.get"))));
            mA.Body.Add(new VariableDeclaration("v0", Fix.Int));
            mA.Body.Add(new Assignment("v1", new Invocation("c2", Fix.GetMethodName("C.get"))));
            mA.Body.Add(new Assignment("i", new ComposedExpression {Variables = new[] {"v0", "v1"}}));

            AssertEntryPoints(mA);
        }

        [Test]
        public void Invocation_NestedComposition()
        {
            CompleteInFile(@"
                namespace N {
                    public class C {
                        public int plus(int in) {
                            return in + 1;
                        }
                    }
                    public class Test {
                        public void A(C c1, C c2) {
                            var i = c1.plus(c2.plus(1));
                            $
                        }
                    }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");

            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new VariableDeclaration("v0", Fix.Int));
            mA.Body.Add(new Assignment("v0", new ConstantExpression()));
            mA.Body.Add(new VariableDeclaration("v1", Fix.Int));
            mA.Body.Add(new Assignment("v1", new Invocation("c2", Fix.GetMethodName("C.plus"), "v0")));
            mA.Body.Add(new VariableDeclaration("v2", Fix.Int));
            mA.Body.Add(new Assignment("v2", new Invocation("c1", Fix.GetMethodName("C.plus"), "v1")));
            mA.Body.Add(new Assignment("i", new ComposedExpression {Variables = new[] {"v0", "v1", "v2"}}));
            // or do we prefer: 
            // mA.Body.Add(new Assignment("i", new ComposedExpression { Variables = new[] { "c1, c2" } }));
            // ??

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
             var c = new C1();
             var v1 = c.m1(o);
             var v2 = c.m2()
             var a = v1 + v2; -> assignment(a, cplxOp(v1, v2))
             */

        /*
             var c = new C1();
             var a = c.m1(o) + j + 1;
             -->
             var c = new C1();
             var v1 = c.m1(o);
             var j = 3;
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