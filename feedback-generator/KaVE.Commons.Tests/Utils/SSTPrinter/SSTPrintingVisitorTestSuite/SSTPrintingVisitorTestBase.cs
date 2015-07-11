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
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Visitor;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.SSTPrinter;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.SSTPrinter.SSTPrintingVisitorTestSuite
{
    internal class SSTPrintingVisitorTestBase
    {
        private SSTPrintingVisitor _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new SSTPrintingVisitor();
        }

        protected void AssertPrintWithCustomContext(ISSTNode sst, SSTPrintingContext context, string expected)
        {
            var indentationLevel = context.IndentationLevel;
            sst.Accept(_sut, context);
            var actual = context.ToString();
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(indentationLevel, context.IndentationLevel);
        }

        protected void AssertPrintWithCustomContext(ISSTNode sst, SSTPrintingContext context, params string[] expectedLines)
        {
            AssertPrintWithCustomContext(sst, context, String.Join(Environment.NewLine, expectedLines));
        }

        protected void AssertPrint(ISSTNode sst, params string[] expectedLines)
        {
            TestPrintingWithoutIndentation(sst, expectedLines);
            TestPrintingWithHighlightingProducesValidXaml(sst);

            // Expressions and references can't be indented
            if (!(sst is IExpression || sst is IReference))
            {
                TestPrintingWithIndentation(sst, expectedLines);
            }
        }

        private void TestPrintingWithoutIndentation(ISSTNode sst, string[] expectedLines)
        {
            AssertPrintWithCustomContext(
                sst,
                new SSTPrintingContext {IndentationLevel = 0},
                expectedLines);
        }

        private void TestPrintingWithHighlightingProducesValidXaml(ISSTNode sst)
        {
            var context = new XamlSSTPrintingContext();
            sst.Accept(_sut, context);
            var actual = context.ToString();

            // throws and fails test if markup is invalid
            XamlUtils.CreateDataTemplateFromXaml(actual);
        }

        private void TestPrintingWithIndentation(ISSTNode sst, IEnumerable<string> expectedLines)
        {
            var indentedLines = expectedLines.Select(line => String.IsNullOrEmpty(line) ? line : "    " + line);
            AssertPrintWithCustomContext(
                sst,
                new SSTPrintingContext { IndentationLevel = 1 },
                indentedLines.ToArray());
        }
    }
}