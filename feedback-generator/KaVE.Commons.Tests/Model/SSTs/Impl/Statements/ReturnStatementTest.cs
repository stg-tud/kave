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
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Statements
{
    public class ReturnStatementTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new ReturnStatement();
            Assert.AreEqual(new UnknownExpression(), sut.Expression);
            Assert.False(sut.IsVoid);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new ReturnStatement
            {
                Expression = new ConstantValueExpression(),
                IsVoid = true
            };
            Assert.AreEqual(new ConstantValueExpression(), sut.Expression);
            Assert.True(sut.IsVoid);
        }

        [Test]
        public void Equality_default()
        {
            var a = new ReturnStatement();
            var b = new ReturnStatement();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_reallyTheSame()
        {
            var a = new ReturnStatement
            {
                Expression = new ConstantValueExpression(),
                IsVoid = true
            };
            var b = new ReturnStatement
            {
                Expression = new ConstantValueExpression(),
                IsVoid = true
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_differentExpression()
        {
            var a = new ReturnStatement {Expression = new ConstantValueExpression()};
            var b = new ReturnStatement {Expression = new NullExpression()};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_differentKind()
        {
            var a = new ReturnStatement {IsVoid = true};
            var b = new ReturnStatement();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new ReturnStatement();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new ReturnStatement();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new ReturnStatement());
        }
    }
}