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

using System;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.SSTPrinter;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.SSTPrinter
{
    internal class XamlSSTPrintingContextTest
    {
        [Test]
        public void UnknownMarker()
        {
            var sst = SetupSST(
                new ExpressionStatement
                {
                    Expression = new UnknownExpression()
                });

            AssertLinesInMethodBody(sst, "<Span Foreground=\"Blue\">???</Span>;");
        }

        [Test]
        public void CursorPosition()
        {
            var sst = SetupSST(
                new ExpressionStatement
                {
                    Expression = new CompletionExpression
                    {
                        VariableReference = new VariableReference {Identifier = "o"},
                        Token = "f"
                    }
                });

            AssertLinesInMethodBody(sst, "o.f<Bold>$</Bold>;");
        }

        private static ISST SetupSST(params IStatement[] stmts)
        {
            return new SST
            {
                EnclosingType = Names.Type("T,P"),
                Methods =
                {
                    new MethodDeclaration
                    {
                        Name = Names.Method("[R,P] [T,P].M()"),
                        Body = Lists.NewList(stmts)
                    }
                }
            };
        }

        private static void AssertFormatting(string expected, ISST sst)
        {
            var sut = new XamlSSTPrintingContext();
            sst.Accept(new SSTPrintingVisitor(), sut);
            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }

        private void AssertLinesInMethodBody(ISST sst, params string[] lines)
        {
            AssertFormatting(
                "<Span Foreground=\"Blue\">class</Span> <Span Foreground=\"#2B91AF\">T</Span>" + Environment.NewLine +
                "{" + Environment.NewLine +
                "    <Span Foreground=\"#2B91AF\">R</Span> M()" + Environment.NewLine +
                "    {" + Environment.NewLine +
                "        " + string.Join(Environment.NewLine + "        ", lines) + Environment.NewLine +
                "    }" + Environment.NewLine +
                "}",
                sst);
        }
    }
}