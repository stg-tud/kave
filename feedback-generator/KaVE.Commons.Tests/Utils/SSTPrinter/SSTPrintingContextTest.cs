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
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.SSTPrinter;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.SSTPrinter
{
    internal class SSTPrintingContextTest
    {
        private static void AssertTypeFormat(string expected, string typeIdentifier)
        {
            var sut = new SSTPrintingContext();
            Assert.AreEqual(expected, sut.Type(Names.Type(typeIdentifier)).ToString());
        }

        [Test]
        public void TypeNameFormat()
        {
            AssertTypeFormat("T", "T,P");
        }

        [Test]
        public void TypeNameFormat_Generics()
        {
            AssertTypeFormat("EventHandler<EventArgsType>", "EventHandler`1[[T -> EventArgsType,P]],P");
        }

        [Test]
        public void TypeNameFormat_UnknownGenericType()
        {
            // these TypeNames are equivalent
            AssertTypeFormat("C<T>", "C`1[[T]],P");
            AssertTypeFormat("C<?>", "C`1[[T -> ?]],P");
            AssertTypeFormat("C<T>", "C`1[[T -> T]],P");
        }

        [Test]
        public void TypeNameFormat_UnknownToUnknownGenericType()
        {
            AssertTypeFormat("Task<T>", "Task`1[[TResult -> T]], mscorlib, 4.0.0.0");
        }

        [Test]
        public void TypeNameFormat_MultipleGenerics()
        {
            AssertTypeFormat("A<B, C>", "A`2[[T1 -> B,P],[T2 -> C,P]],P");
        }

        [Test]
        public void TypeNameFormat_NestedGenerics()
        {
            AssertTypeFormat("A<B<C>>", "A`1[[T -> B`1[[T -> C,P]],P]],P");
        }

        [Test]
        public void StatementBlock_NotEmpty_WithBrackets()
        {
            var stmts = new KaVEList<IStatement> {new ContinueStatement(), new BreakStatement()};
            var visitor = new SSTPrintingVisitor();
            var sut = new SSTPrintingContext();

            var expected = String.Join(
                Environment.NewLine,
                "",
                "{",
                "    continue;",
                "    break;",
                "}");

            sut.StatementBlock(stmts, visitor);
            Assert.AreEqual(expected, sut.ToString());
        }

        [Test]
        public void StatementBlock_Empty_WithBrackets()
        {
            var stmts = Lists.NewList<IStatement>();
            var visitor = new SSTPrintingVisitor();
            var sut = new SSTPrintingContext();

            sut.StatementBlock(stmts, visitor);
            Assert.AreEqual(" { }", sut.ToString());
        }

        [Test]
        public void StatementBlock_NotEmpty_WithoutBrackets()
        {
            var stmts = new KaVEList<IStatement> {new ContinueStatement(), new BreakStatement()};
            var visitor = new SSTPrintingVisitor();
            var sut = new SSTPrintingContext();

            var expected = String.Join(
                Environment.NewLine,
                "",
                "    continue;",
                "    break;");

            sut.StatementBlock(stmts, visitor, false);
            Assert.AreEqual(expected, sut.ToString());
        }

        [Test]
        public void StatementBlock_Empty_WithoutBrackets()
        {
            var stmts = Lists.NewList<IStatement>();
            var visitor = new SSTPrintingVisitor();
            var sut = new SSTPrintingContext();

            sut.StatementBlock(stmts, visitor, false);
            Assert.AreEqual("", sut.ToString());
        }

        [Test]
        public void ParameterList_NoParameters()
        {
            var parameters = Lists.NewList<IParameterName>();
            var sut = new SSTPrintingContext();
            sut.ParameterList(parameters);
            Assert.AreEqual("()", sut.ToString());
        }

        [Test]
        public void ParameterList_OneParameter()
        {
            var parameters = new KaVEList<IParameterName> {Names.Parameter("[A,P] p1")};
            var sut = new SSTPrintingContext();
            sut.ParameterList(parameters);
            Assert.AreEqual("(A p1)", sut.ToString());
        }

        [Test]
        public void ParameterList_MultipleParameters()
        {
            var parameters = new KaVEList<IParameterName> {Names.Parameter("[A,P] p1"), Names.Parameter("[B,P] p2")};
            var sut = new SSTPrintingContext();
            sut.ParameterList(parameters);
            Assert.AreEqual("(A p1, B p2)", sut.ToString());
        }
    }
}