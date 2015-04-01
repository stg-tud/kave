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
using System.Linq;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    internal class TriggerLocationsTest : BaseSSTAnalysisTest
    {
        [Test, ExpectedException(typeof (NotSupportedException)), Ignore]
        public void TriggeredOutsideMethod()
        {
            CompleteInClass(@"
                public void A() {}
                $
            ");
        }

        [Test, ExpectedException(typeof (NotSupportedException)), Ignore]
        public void TriggeredOutsideMethod_WithToken()
        {
            CompleteInClass(@"
                public void A() {}
                B$
            ");
        }

        [Test, ExpectedException(typeof (NotSupportedException)), Ignore]
        public void TriggeredInMethodDeclaration()
        {
            CompleteInClass(@"
                public void$ A() {}
            ");
        }

        [Test, ExpectedException(typeof (NotSupportedException)), Ignore]
        public void TriggeredInParameterList()
        {
            CompleteInClass(@"
                public void A($) {}
            ");
        }

        [Test, ExpectedException(typeof (NotSupportedException)), Ignore]
        public void TriggeredInParameterList2()
        {
            CompleteInClass(@"
                public void A(object o) {
                    o.Equals($);
                }
            ");
        }

        [Test, ExpectedException(typeof (NotSupportedException)), Ignore]
        public void TriggeredInParameterList3()
        {
            CompleteInClass(@"
                public void A(object o) {
                    o.Equals(o.$);
                }
            ");
        }

        [Test, ExpectedException(typeof (NotSupportedException)), Ignore]
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

        [Test, Ignore]
        public void TriggeredInMethod()
        {
            CompleteInClass(@"
                public void A()
                {
                    $
                }
            ");

            var trigger = new CompletionExpression();

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new ExpressionStatement {Expression = trigger});

            var first = ResultSST.Methods.FirstOrDefault();
            var hc1 = mA.GetHashCode();
            var hc2 = first.GetHashCode();

            var eq = mA.Equals(first);
            var eq2 = first.Equals(mA);

            AssertMethod(mA);
        }

        [Test, Ignore]
        public void TriggeredInMethodOnPrefix()
        {
            CompleteInClass(@"
                public void A()
                {
                    o$
                }
            ");

            var trigger = new CompletionExpression {Token = "o"};

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new ExpressionStatement {Expression = trigger});
            AssertMethod(mA);
        }

        [Test, Ignore]
        public void TriggeredInMethod_OnReference()
        {
            CompleteInClass(@"
                public void A()
                {
                    a.$
                }
            ");

            var trigger = new CompletionExpression {Token = "a."};

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new ExpressionStatement {Expression = trigger});
            AssertMethod(mA);
        }

        [Test, Ignore]
        public void TriggeredInMethod_OnReference_WithToken()
        {
            CompleteInClass(@"
                public void A()
                {
                    a.b$
                }
            ");

            var trigger = new CompletionExpression {Token = "a.b"};

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new ExpressionStatement {Expression = trigger});
            AssertMethod(mA);
        }

        [Test, Ignore]
        public void TriggeredInMethod_OnReferenceChain()
        {
            CompleteInClass(@"
                public void A()
                {
                    a.b.c$
                }
            ");

            var trigger = new CompletionExpression {Token = "a.b.c"};

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new ExpressionStatement {Expression = trigger});
            AssertMethod(mA);
        }

        [Test, Ignore]
        public void TriggeredInMethod_BetweenStatements()
        {
            // TODO wieder alles auf ausgang :)
            CompleteInClass(@"
                public void A()
                {
                    var o = new object();
                    this.GetHashCode();
                    this.Equals(o);
                    this.Equals(this);
                    $
                    this.GetHashCode();
                }
            ");

            var preceedingInvocation = SSTUtil.InvocationStatement("this", Fix.Object_GetHashCode);
            var trigger = new CompletionExpression {Token = "a.b.c"};
            var subsequentInvocation = SSTUtil.InvocationStatement("this", Fix.Object_GetHashCode);

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(preceedingInvocation);
            mA.Body.Add(new ExpressionStatement {Expression = trigger});
            mA.Body.Add(subsequentInvocation);
            AssertMethod(mA);
        }

        [Test, Ignore]
        public void TriggeredInMethod_InAssignment()
        {
            CompleteInClass(@"
                public void A()
                {
                    var v = $
                }
            ");

            var trigger = new CompletionExpression();
            var assignment = SSTUtil.AssignmentToLocal("v", trigger);

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(assignment);
            AssertMethod(mA);
        }

        [Test, Ignore]
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

            var trigger = new CompletionExpression();
            var whileLoop = new WhileLoop {Condition = new ConstantValueExpression()};
            whileLoop.Body.Add(new ExpressionStatement {Expression = trigger});

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(whileLoop);
            AssertMethod(mA);
        }

        [Test, Ignore]
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

            var trigger = new CompletionExpression();
            var whileLoop = new WhileLoop {Condition = new ConstantValueExpression()};
            whileLoop.Body.Add(new ExpressionStatement {Expression = trigger});

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(whileLoop);
            AssertMethod(mA);
        }

        [Test, Ignore]
        public void TriggeredInMethod_InBranch()
        {
            CompleteInClass(@"
                public void A()
                {
                    var v = true ? $
                }
            ");

            var trigger = new CompletionExpression();
            var ifElseExpression = new IfElseExpression
            {
                Condition = new ConstantValueExpression(),
                ThenExpression = null, //trigger,
                ElseExpression = null
            };
            var assignment = SSTUtil.AssignmentToLocal("v", ifElseExpression);

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(assignment);
            AssertMethod(mA);
        }
    }
}