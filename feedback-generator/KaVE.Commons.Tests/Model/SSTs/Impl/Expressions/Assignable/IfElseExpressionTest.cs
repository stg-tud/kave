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

using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Expressions.Assignable
{
    internal class IfElseExpressionTest : SSTBaseTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new IfElseExpression();
            Assert.AreEqual(new UnknownExpression(), sut.Condition);
            Assert.AreEqual(new UnknownExpression(), sut.ThenExpression);
            Assert.AreEqual(new UnknownExpression(), sut.ElseExpression);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new IfElseExpression
            {
                Condition = new ConstantValueExpression(),
                ThenExpression = new NullExpression(),
                ElseExpression = new ReferenceExpression()
            };
            Assert.AreEqual(new ConstantValueExpression(), sut.Condition);
            Assert.AreEqual(new NullExpression(), sut.ThenExpression);
            Assert.AreEqual(new ReferenceExpression(), sut.ElseExpression);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new IfElseExpression();
            var b = new IfElseExpression();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new IfElseExpression
            {
                Condition = new ConstantValueExpression(),
                ThenExpression = new ConstantValueExpression(),
                ElseExpression = new ReferenceExpression()
            };
            var b = new IfElseExpression
            {
                Condition = new ConstantValueExpression(),
                ThenExpression = new ConstantValueExpression(),
                ElseExpression = new ReferenceExpression()
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentCondition()
        {
            var a = new IfElseExpression {Condition = new ConstantValueExpression()};
            var b = new IfElseExpression();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentThen()
        {
            var a = new IfElseExpression {ThenExpression = new ConstantValueExpression()};
            var b = new IfElseExpression();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentElse()
        {
            var a = new IfElseExpression {ElseExpression = new ConstantValueExpression()};
            var b = new IfElseExpression();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new IfElseExpression();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new IfElseExpression();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new IfElseExpression());
        }
    }
}