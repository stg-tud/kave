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
using System.Windows.Markup;
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

        public void AssertPrintSingle(ISSTNode sst, SSTPrintingContext context, string expected)
        {
            var indentationLevel = context.IndentationLevel;
            sst.Accept(_sut, context);
            var actual = context.ToString();
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(indentationLevel, context.IndentationLevel);
        }

        public void AssertPrintSingle(ISSTNode sst, SSTPrintingContext context, params string[] expectedLines)
        {
            AssertPrintSingle(sst, context, String.Join(Environment.NewLine, expectedLines));
        }

        // see KaVE.VsFeedbackGenerator.SessionManager.Presentation.XamlBindableRichTextBox
        private const string DataTemplateBegin =
            "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><TextBlock xml:space=\"preserve\">";

        private const string DataTemplateEnd = "</TextBlock></DataTemplate>";

        // Asserts that the syntax highlighted result is valid (and parsable) XAML
        private void AssertPrintHighlighted(ISSTNode sst)
        {
            var context = new XamlSSTPrintingContext();
            sst.Accept(_sut, context);
            var actual = context.ToString();

            // throws and fails test if markup is invalid
            XamlReader.Parse(DataTemplateBegin + actual + DataTemplateEnd);
        }

        protected void AssertPrint(ISSTNode sst, params string[] expectedLines)
        {
            // Test if printing works as expected on indentation level 0
            AssertPrintSingle(
                sst,
                new SSTPrintingContext {IndentationLevel = 0},
                expectedLines);

            // Test if syntax highlighting produces valid XAML
            AssertPrintHighlighted(sst);

            // Expressions and references can't be indented
            if (sst is IExpression || sst is IReference)
            {
                return;
            }

            // Test if printing works as expected on indentation level 1
            var indentedLines = expectedLines.Select(line => String.IsNullOrEmpty(line) ? line : "    " + line);
            AssertPrintSingle(
                sst,
                new SSTPrintingContext {IndentationLevel = 1},
                indentedLines.ToArray());
        }
    }
}