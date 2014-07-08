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
    [TestFixture, Ignore]
    internal class SingleAssignmentTest : AbstractSSTTest
    {
        [Test]
        public void TriggeredInMethod()
        {
            CompleteInClass(@"
                public void A()
                {
                    var i;
                }
            ");

            var mA = NewMethodDeclaration("A", Fix.Void);
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));

            AssertEntryPoints(mA);
        }

        [Test]
        public void TriggeredInMethoasdd()
        {
            CompleteInClass(@"
                public void A()
                {
                    var i = 5;
                }
            ");

            var mA = NewMethodDeclaration("A", Fix.Void);
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));

            mA.Body.Add(
                new Assignment
                {
                    Identifier = "i",
                    Value = new ConstantExpression()
                });

            AssertEntryPoints(mA);
        }


        [Test]
        public void TriggeredInMethoaasdsdd()
        {
            CompleteInFile(@"
                namespace N {
                    public class C1 {
                        public Object m(Object o) {
                            return null;
                        }
                    }

                    public class Test {
                        public void ep(Object o)
                        {
                            var c = new C1();
                            var a = c.m(o);
                            $
                        }
                    }
                }
            ");

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
             */
        }
    }
}