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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Utils.SSTPrinter;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.SSTPrinter
{
    [TestFixture]
    class DeclarationPrinterTest
    {
        private string BuildExpectedResult(params string[] lines)
        {
            return String.Join(Environment.NewLine, lines);
        }

        [Test]
        public void EmptyClassDeclaration()
        {
            var sst = new SST {EnclosingType = TypeName.Get("TestClass, TestProject")};
            var expected = BuildExpectedResult(
                "class TestClass",
                "{",
                "}");
            var visitor = new SSTPrintingVisitor();
            var context = new StringBuilder();

            sst.Accept(visitor, context);

            Assert.AreEqual(expected, context.ToString());
        }

        [Test]
        public void FieldDeclaration()
        {
            var sst = new FieldDeclaration() { Name = FieldName.Get("[FieldType,TestProject] [DeclaringType,TestProject].DummyField") };
            const string expected = "FieldType DummyField;";

            var visitor = new SSTPrintingVisitor();
            var context = new StringBuilder();

            sst.Accept(visitor, context);

            Assert.AreEqual(expected, context.ToString());
        }
    }
}
