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
using System.Text;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Visitor;
using KaVE.Commons.Utils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.SSTPrintingVisitorTestSuite
{
    internal class SSTPrintingVisitorTestBase
    {
        private SSTPrintingVisitor _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new SSTPrintingVisitor();
        }

        protected void AssertPrint(ISSTNode sst, params string[] expectedLines)
        {
            var context = new StringBuilder();
            sst.Accept(_sut, context);
            var actual = context.ToString();
            var expected = String.Join(Environment.NewLine, expectedLines);
            Assert.AreEqual(expected, actual);
        }

        protected void AssertTypeFormat(string expected, string typeIdentifier)
        {
            var sb = new StringBuilder();
            Assert.AreEqual(expected, sb.AppendTypeName(TypeName.Get(typeIdentifier)).ToString());
        }
    }
}