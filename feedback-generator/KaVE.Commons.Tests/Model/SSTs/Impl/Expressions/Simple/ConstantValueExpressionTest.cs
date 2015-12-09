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

using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Expressions.Simple
{
    internal class ConstantValueExpressionTest : SSTBaseTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new ConstantValueExpression();
            Assert.Null(sut.Value);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new ConstantValueExpression {Value = "a"};
            Assert.AreEqual("a", sut.Value);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new ConstantValueExpression();
            var b = new ConstantValueExpression();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new ConstantValueExpression {Value = "a"};
            var b = new ConstantValueExpression {Value = "a"};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentValue()
        {
            var a = new ConstantValueExpression {Value = "a"};
            var b = new ConstantValueExpression();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new ConstantValueExpression();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new ConstantValueExpression();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringIsImplemented()
        {
            Assert.AreEqual("Const('1')", new ConstantValueExpression {Value = "1"}.ToString());
            Assert.AreEqual("Const('\"s\"')", new ConstantValueExpression {Value = "\"s\""}.ToString());
            Assert.AreEqual("Const('')", new ConstantValueExpression {Value = ""}.ToString());
            Assert.AreEqual("Const()", new ConstantValueExpression {Value = null}.ToString());
        }
    }
}