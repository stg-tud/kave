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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.Statements;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.SSTPrinter.SSTPrintingVisitorTestSuite
{
    internal class StatementPrinterTest : SSTPrintingVisitorTestBase
    {
        [Test]
        public void BreakStatement()
        {
            var sst = new BreakStatement();
            AssertPrint(sst, "break;");
        }

        [Test]
        public void ContinueStatement()
        {
            var sst = new ContinueStatement();
            AssertPrint(sst, "continue;");
        }

        [Test]
        public void Assignment()
        {
            var sst = SSTUtil.AssignmentToLocal("var", new ConstantValueExpression {Value = "true"});
            AssertPrint(sst, "var = true;");
        }

        [Test]
        public void GotoStatement()
        {
            var sst = new GotoStatement
            {
                Label = "L"
            };

            AssertPrint(sst, "goto L;");
        }

        [Test]
        public void LabelledStatement()
        {
            var sst = new LabelledStatement
            {
                Label = "L",
                Statement = new ContinueStatement()
            };

            AssertPrint(
                sst,
                "L:",
                "continue;");
        }

        [Test]
        public void ThrowStatement()
        {
            var sst = new ThrowStatement
            {
                Reference = new VariableReference {Identifier = "e"}
            };

            AssertPrint(sst, "throw e;");
        }

        [Test]
        public void ThrowStatement_ReThrow()
        {
            var sst = new ThrowStatement();

            AssertPrint(sst, "throw;");
        }

        [Test]
        public void ReturnStatement()
        {
            var sst = new ReturnStatement
            {
                Expression = new ConstantValueExpression {Value = "val"}
            };

            AssertPrint(sst, "return \"val\";");
        }

        [Test]
        public void ReturnStatement_Void()
        {
            var sst = new ReturnStatement {IsVoid = true};
            AssertPrint(sst, "return;");
        }

        [Test]
        public void ExpressionStatement()
        {
            var invocation = new InvocationExpression
            {
                Reference = SSTUtil.VariableReference("this"),
                MethodName = Names.Method("[ReturnType,P] [DeclaringType,P].M([ParameterType,P] p)"),
                Parameters = {new ConstantValueExpression {Value = "1"}}
            };

            var sst = new ExpressionStatement {Expression = invocation};

            AssertPrint(sst, "this.M(1);");
        }

        [Test]
        public void UnknownStatement()
        {
            var sst = new UnknownStatement();
            AssertPrint(sst, "???;");
        }

        [Test]
        public void EventSubscriptionStatement_Subscribe()
        {
            var sst = new EventSubscriptionStatement
            {
                Reference = new EventReference
                {
                    EventName = Names.Event("[EventType,P] [DeclaringType,P].SomeEvent"),
                    Reference = SSTUtil.VariableReference("o")
                },
                Expression = new ReferenceExpression
                {
                    Reference = new MethodReference
                    {
                        MethodName = Names.Method("[ReturnType,P] [DeclaringType,P].Handler()"),
                        Reference = SSTUtil.VariableReference("this")
                    }
                },
                Operation = EventSubscriptionOperation.Add
            };

            AssertPrint(sst, "o.SomeEvent += this.Handler;");
        }

        [Test]
        public void EventSubscriptionStatement_Unsubscribe()
        {
            var sst = new EventSubscriptionStatement
            {
                Reference = new EventReference
                {
                    EventName = Names.Event("[EventType,P] [DeclaringType,P].SomeEvent"),
                    Reference = SSTUtil.VariableReference("o")
                },
                Expression = new ReferenceExpression
                {
                    Reference = new MethodReference
                    {
                        MethodName = Names.Method("[ReturnType,P] [DeclaringType,P].Handler()"),
                        Reference = SSTUtil.VariableReference("this")
                    }
                },
                Operation = EventSubscriptionOperation.Remove
            };

            AssertPrint(sst, "o.SomeEvent -= this.Handler;");
        }
    }
}