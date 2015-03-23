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
 *    - Andreas Bauer
 */

using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.SSTPrinter.SSTPrintingVisitorTestSuite
{
    internal class ExpressionPrinterTest : SSTPrintingVisitorTestBase
    {
        [Test]
        public void ConstantValueExpression_String()
        {
            var sst = new ConstantValueExpression {Value = "val"};
            AssertPrint(sst, "\"val\"");
        }

        [Test]
        public void ConstantValueExpression_Int()
        {
            var sst = new ConstantValueExpression {Value = "1"};
            AssertPrint(sst, "1");
        }

        [Test]
        public void ConstantValueExpression_Float()
        {
            var sst = new ConstantValueExpression {Value = "-3.141592"};
            AssertPrint(sst, "-3.141592");
        }

        [Test]
        public void ConstantValueExpression_BoolTrue()
        {
            var sst = new ConstantValueExpression {Value = "true"};
            AssertPrint(sst, "true");
        }

        [Test]
        public void ConstantValueExpression_BoolFalse()
        {
            var sst = new ConstantValueExpression {Value = "false"};
            AssertPrint(sst, "false");
        }

        [Test]
        public void ConstantValueExpression_Null()
        {
            var sst = new ConstantValueExpression {Value = "null"};
            AssertPrint(sst, "null");
        }

        [Test]
        public void IfElseExpression()
        {
            var sst = new IfElseExpression
            {
                Condition = new ConstantValueExpression {Value = "true"},
                ThenExpression = new ConstantValueExpression {Value = "1"},
                ElseExpression = new ConstantValueExpression {Value = "2"}
            };

            AssertPrint(sst, "(true) ? 1 : 2");
        }

        [Test]
        public void InvocationExpression()
        {
            var sst = new InvocationExpression
            {
                Reference = SSTUtil.VariableReference("this"),
                MethodName = MethodName.Get("[ReturnType,P] [DeclaringType,P].M([ParameterType,P] p)"),
                Parameters = {new ConstantValueExpression {Value = "1"}}
            };

            AssertPrint(sst, "this.M(1)");
        }

        [Test]
        public void NullExpression()
        {
            var sst = new NullExpression();
            AssertPrint(sst, "null");
        }

        [Test]
        public void ReferenceExpression()
        {
            var sst = new ReferenceExpression {Reference = SSTUtil.VariableReference("variable")};
            AssertPrint(sst, "variable");
        }

        [Test]
        public void ComposedExpression()
        {
            var sst = new ComposedExpression
            {
                References =
                {
                    SSTUtil.VariableReference("a"),
                    SSTUtil.VariableReference("b"),
                    SSTUtil.VariableReference("c")
                }
            };

            AssertPrint(sst, "composed(a, b, c)");
        }

        [Test]
        public void LoopHeaderBlockExpression()
        {
            var sst = new LoopHeaderBlockExpression
            {
                Body =
                {
                    new ContinueStatement(),
                    new BreakStatement(),
                }
            };

            AssertPrint(
                sst,
                "",
                "{",
                "    continue;",
                "    break;",
                "}");
        }

        [Test]
        public void UnknownExpression()
        {
            var sst = new UnknownExpression();
            AssertPrint(sst, "???");
        }

        [Test]
        public void CompletionExpression_OnTypeReference()
        {
            var sst = new CompletionExpression {TypeReference = TypeName.Get("T,P"), Token = "incompl"};
            AssertPrint(sst, "T.incompl$");
        }

        [Test]
        public void CompletionExpression_OnTypeReference_GenericType()
        {
            var sst = new CompletionExpression {TypeReference = TypeName.Get("T`1[[T1 -> A,P]],P"), Token = "incompl"};
            AssertPrint(sst, "T<A>.incompl$");
        }

        [Test]
        public void CompletionExpression_OnObjectReference()
        {
            var sst = new CompletionExpression {VariableReference = SSTUtil.VariableReference("obj"), Token = "incompl"};
            AssertPrint(sst, "obj.incompl$");
        }

        [Test]
        public void CompletionExpression_OnNothing()
        {
            var sst = new CompletionExpression {Token = "incompl"};
            AssertPrint(sst, "incompl$");
        }

        [Test]
        public void LambdaExpression()
        {
            var sst = new LambdaExpression
            {
                Parameters = {ParameterName.Get("[C, A] p1"), ParameterName.Get("[C, B] p2")},
                Body =
                {
                    new ContinueStatement(),
                    new BreakStatement()
                }
            };

            AssertPrint(
                sst,
                "(C p1, C p2) =>",
                "{",
                "    continue;",
                "    break;",
                "}");
        }


        [Test]
        public void LambdaExpression_NoParametersAndEmptyBody()
        {
            var sst = new LambdaExpression();

            AssertPrint(
                sst,
                "() => { }");
        }
    }
}