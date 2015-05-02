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

using System;
using System.Linq;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Visitor;
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

        private void AssertPrintHelper(ISSTNode sst, int indentationLevel, string expected)
        {
            var context = new SSTPrintingContext {IndentationLevel = indentationLevel};
            sst.Accept(_sut, context);
            var actual = context.ToString();
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(indentationLevel, context.IndentationLevel);
        }

        protected void AssertPrint(ISSTNode sst, params string[] expectedLines)
        {
            AssertPrintHelper(sst, 0, String.Join(Environment.NewLine, expectedLines));

            // expressions and references can't be indented
            if (sst is IExpression || sst is IReference)
            {
                return;
            }

            var indentedLines = expectedLines.Select(line => String.IsNullOrEmpty(line) ? line : "    " + line);
            AssertPrintHelper(sst, 1, String.Join(Environment.NewLine, indentedLines));
        }
    }
}