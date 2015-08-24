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

using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.References;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.SSTPrinter.SSTPrintingVisitorTestSuite
{
    internal class ExpressionPrinterTest : SSTPrintingVisitorTestBase
    {
        [Test]
        public void NullExpression()
        {
            AssertPrint(Null(), "null");
        }

        [Test]
        public void ConstantValueExpression_WithoutValue()
        {
            AssertPrint(Constant(), "\"...\"");
        }

        [Test]
        public void ConstantValueExpression_WithString()
        {
            AssertPrint(Constant("val"), "\"val\"");
        }

        [Test]
        public void ConstantValueExpression_NullLiteralIsUsedAsString()
        {
            AssertPrint(Constant("null"), "\"null\"");
        }

        [Test]
        public void ConstantValueExpression_WithInt()
        {
            AssertPrint(Constant("1"), "1");
        }

        [Test]
        public void ConstantValueExpression_WithFloat()
        {
            AssertPrint(Constant("0.123"), "0.123");
        }

        [Test]
        public void ConstantValueExpression_WithBoolTrue()
        {
            AssertPrint(Constant("true"), "true");
        }

        [Test]
        public void ConstantValueExpression_WithBoolFalse()
        {
            AssertPrint(Constant("false"), "false");
        }

        [Test]
        public void InvocationExpression_ConstantValue()
        {
            var sst = new InvocationExpression
            {
                Reference = VarRef("this"),
                MethodName = MethodName.Get("[R,P] [D,P].M([T,P] p)"),
                Parameters = {Constant("1")}
            };

            AssertPrint(sst, "this.M(1)");
        }

        [Test]
        public void InvocationExpression_NullValue()
        {
            var sst = new InvocationExpression
            {
                Reference = SSTUtil.VariableReference("this"),
                MethodName = MethodName.Get("[R,P] [D,P].M([T,P] p)"),
                Parameters = {Null()}
            };

            AssertPrint(sst, "this.M(null)");
        }

        [Test]
        public void InvocationExpression_Static()
        {
            var sst = new InvocationExpression
            {
                Reference = VarRef("should be ignored anyways"),
                MethodName = MethodName.Get("static [R,P] [D,P].M([T,P] p)"),
                Parameters = {Null()}
            };

            AssertPrint(sst, "D.M(null)");
        }

        [Test]
        public void InvocationExpression_Constructor()
        {
            var sst = new InvocationExpression()
            {
                Reference = VarRef("should be ignored anyways"),
                MethodName = MethodName.Get("[System.Void, mscorlib, 4.0.0.0] [C,P]..ctor()"),
                Parameters = {Constant("1")}
            };

            AssertPrint(sst, "new C(1)");
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
        public void CompletionExpression_OnNothing()
        {
            var sst = new CompletionExpression();
            AssertPrint(sst, "$");
        }

        [Test]
        public void CompletionExpression_OnToken()
        {
            var sst = new CompletionExpression {Token = "t"};
            AssertPrint(sst, "t$");
        }

        [Test]
        public void CompletionExpression_OnVariableReference()
        {
            var sst = new CompletionExpression
            {
                VariableReference = VarRef("o"),
            };
            AssertPrint(sst, "o.$");
        }

        [Test]
        public void CompletionExpression_OnVariableReferenceWithToken()
        {
            var sst = new CompletionExpression
            {
                VariableReference = VarRef("o"),
                Token = "t"
            };
            AssertPrint(sst, "o.t$");
        }

        [Test]
        public void CompletionExpression_OnTypeReference()
        {
            var sst = new CompletionExpression
            {
                TypeReference = TypeName.Get("T,P")
            };
            AssertPrint(sst, "T.$");
        }

        [Test]
        public void CompletionExpression_OnTypeReferenceWithToken()
        {
            var sst = new CompletionExpression
            {
                TypeReference = TypeName.Get("T,P"),
                Token = "t"
            };
            AssertPrint(sst, "T.t$");
        }

        [Test]
        public void CompletionExpression_OnTypeReference_GenericType()
        {
            var sst = new CompletionExpression
            {
                TypeReference = TypeName.Get("T`1[[G -> A,P]],P")
            };
            AssertPrint(sst, "T<A>.$");
        }

        [Test]
        public void CompletionExpression_OnTypeReference_UnresolvedGenericType()
        {
            var sst = new CompletionExpression
            {
                TypeReference = TypeName.Get("T`1[[G]],P")
            };
            AssertPrint(sst, "T<?>.$");
        }

        [Test]
        public void LambdaExpression()
        {
            var sst = new LambdaExpression
            {
                Name = LambdaName.Get("[T,P]([C, A] p1, [C, B] p2)"),
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

        private static INullExpression Null()
        {
            return new NullExpression();
        }

        private static IConstantValueExpression Constant(string value = null)
        {
            return new ConstantValueExpression
            {
                Value = value
            };
        }

        private static IVariableReference VarRef(string id)
        {
            return new VariableReference
            {
                Identifier = id
            };
        }
    }
}