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

using System;
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs.Blocks;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    internal class TriggerLocationsTest : BaseSSTAnalysisTest
    {
        [Test, ExpectedException(typeof (NotSupportedException))]
        public void TriggeredOutsideMethod()
        {
            CompleteInClass(@"
                public void A() {}
                $
            ");
        }

        [Test, ExpectedException(typeof (NotSupportedException))]
        public void TriggeredOutsideMethod_WithToken()
        {
            CompleteInClass(@"
                public void A() {}
                B$
            ");
        }

        [Test, ExpectedException(typeof (NotSupportedException))]
        public void TriggeredInMethodDeclaration()
        {
            CompleteInClass(@"
                public void$ A() {}
            ");
        }

        [Test, ExpectedException(typeof (NotSupportedException))]
        public void TriggeredInParameterList()
        {
            CompleteInClass(@"
                public void A($) {}
            ");
        }

        [Test, ExpectedException(typeof (NotSupportedException))]
        public void TriggeredInParameterList2()
        {
            CompleteInClass(@"
                public void A(object o) {
                    o.Equals($);
                }
            ");
        }

        [Test, ExpectedException(typeof (NotSupportedException))]
        public void TriggeredInParameterList3()
        {
            CompleteInClass(@"
                public void A(object o) {
                    o.Equals(o.$);
                }
            ");
        }

        [Test, ExpectedException(typeof (NotSupportedException))]
        public void TriggeredInInterface()
        {
            CompleteInCSharpFile(@"
                public interface I {
                    public void A();
                    $
                }
            ");

            // TODO think about this again... what about abstract base classes?
        }

        [Test]
        public void TriggeredInMethod()
        {
            CompleteInClass(@"
                public void A()
                {
                    $
                }
            ");

            var trigger = new StatementCompletion();

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(trigger);
            AssertMethod(mA);
        }

        [Test]
        public void TriggeredInMethodOnPrefix()
        {
            CompleteInClass(@"
                public void A()
                {
                    o$
                }
            ");

            var trigger = new StatementCompletion {Token = "o"};

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(trigger);
            AssertMethod(mA);
        }

        [Test]
        public void TriggeredInMethod_OnReference()
        {
            CompleteInClass(@"
                public void A()
                {
                    a.$
                }
            ");

            var trigger = new StatementCompletion {Identifier = "a"};

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(trigger);
            AssertMethod(mA);
        }

        [Test]
        public void TriggeredInMethod_OnReference_WithToken()
        {
            CompleteInClass(@"
                public void A()
                {
                    a.b$
                }
            ");

            var trigger = new StatementCompletion {Identifier = "a", Token = "b"};

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(trigger);
            AssertMethod(mA);
        }

        [Test]
        public void TriggeredInMethod_OnReferenceChain()
        {
            CompleteInClass(@"
                public void A()
                {
                    a.b.c$
                }
            ");

            var trigger = new StatementCompletion {Identifier = "a", Token = "b"};

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(trigger);
            AssertMethod(mA);
        }

        [Test]
        public void TriggeredInMethod_BetweenStatements()
        {
            CompleteInClass(@"
                public void A()
                {
                    this.GetHashCode();
                    $
                    this.GetHashCode();
                }
            ");

            var preceedingInvocation = new InvocationStatement
            {
                Target =
                    new InvocationExpression
                    {
                        Identifier = "this",
                        Name =
                            MethodName.Get(
                                "[System.Boolean, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0].GetHashCode()")
                    }
            };
            var trigger = new StatementCompletion {Identifier = "a", Token = "b"};
            var subsequentInvocation = new InvocationStatement
            {
                Target =
                    new InvocationExpression
                    {
                        Identifier = "this",
                        Name =
                            MethodName.Get(
                                "[System.Boolean, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0].GetHashCode()")
                    }
            };

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(preceedingInvocation);
            mA.Body.Add(trigger);
            mA.Body.Add(subsequentInvocation);
            AssertMethod(mA);
        }

        [Test]
        public void TriggeredInMethod_InAssignment()
        {
            CompleteInClass(@"
                public void A()
                {
                    var v = $
                }
            ");

            var trigger = new ExpressionCompletion();
            var assignment = new Assignment("v", trigger);

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(assignment);
            AssertMethod(mA);
        }

        [Test]
        public void TriggeredInMethod_InLock()
        {
            CompleteInClass(@"
                public void A()
                {
                    while (true) {
                        $
                    }
                }
            ");

            var trigger = new StatementCompletion();
            var whileLoop = new WhileLoop {Condition = new ConstantExpression()};
            whileLoop.Body.Add(trigger);

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(whileLoop);
            AssertMethod(mA);
        }

        [Test]
        public void METHOD_FOR_DEBUGGING_ONLY()
        {
            CompleteInClass(@"
                public void A()
                {
                    while (true) {
                        a.b$
                    }
                }
            ");

            var trigger = new StatementCompletion();
            var whileLoop = new WhileLoop {Condition = new ConstantExpression()};
            whileLoop.Body.Add(trigger);

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(whileLoop);
            AssertMethod(mA);
        }

        [Test]
        public void TriggeredInMethod_InBranch()
        {
            CompleteInClass(@"
                public void A()
                {
                    var v = true ? $
                }
            ");

            var trigger = new ExpressionCompletion();
            var ifElseExpression = new IfElseExpression
            {
                Condition = new ConstantExpression(),
                ThenExpression = trigger,
                ElseExpression = null
            };
            var assignment = new Assignment {Identifier = "v", Value = ifElseExpression};

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(assignment);
            AssertMethod(mA);
        }
    }
}