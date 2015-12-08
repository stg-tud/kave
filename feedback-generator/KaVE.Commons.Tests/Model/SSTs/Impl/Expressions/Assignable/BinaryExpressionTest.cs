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

using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Expressions.Assignable
{
    internal class BinaryExpressionTest : SSTBaseTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new BinaryExpression();
            Assert.AreEqual(BinaryOperator.Unknown, sut.Operator);
            Assert.AreEqual(new UnknownExpression(), sut.LeftOperand);
            Assert.AreEqual(new UnknownExpression(), sut.RightOperand);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new BinaryExpression
            {
                LeftOperand = new ConstantValueExpression(),
                Operator = BinaryOperator.And,
                RightOperand = new ReferenceExpression()
            };
            Assert.AreEqual(BinaryOperator.And, sut.Operator);
            Assert.AreEqual(new ConstantValueExpression(), sut.LeftOperand);
            Assert.AreEqual(new ReferenceExpression(), sut.RightOperand);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new BinaryExpression();
            var b = new BinaryExpression();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new BinaryExpression
            {
                LeftOperand = new ConstantValueExpression(),
                Operator = BinaryOperator.And,
                RightOperand = new ReferenceExpression()
            };
            var b = new BinaryExpression
            {
                LeftOperand = new ConstantValueExpression(),
                Operator = BinaryOperator.And,
                RightOperand = new ReferenceExpression()
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentLeft()
        {
            var a = new BinaryExpression
            {
                LeftOperand = new ConstantValueExpression()
            };
            var b = new BinaryExpression();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentOp()
        {
            var a = new BinaryExpression
            {
                Operator = BinaryOperator.And
            };
            var b = new BinaryExpression();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentRight()
        {
            var a = new BinaryExpression
            {
                RightOperand = new ReferenceExpression()
            };
            var b = new BinaryExpression();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new BinaryExpression();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new BinaryExpression();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new BinaryExpression());
        }

        [Test]
        public void NumberingOfEnumIsStable()
        {
            // IMPORTANT! do not change any of these because it will affect serialization

            Assert.AreEqual(0, (int) BinaryOperator.Unknown);

            // Logical
            Assert.AreEqual(1, (int) BinaryOperator.LessThan);
            Assert.AreEqual(2, (int) BinaryOperator.LessThanOrEqual);
            Assert.AreEqual(3, (int) BinaryOperator.Equal);
            Assert.AreEqual(4, (int) BinaryOperator.GreaterThanOrEqual);
            Assert.AreEqual(5, (int) BinaryOperator.GreaterThan);
            Assert.AreEqual(6, (int) BinaryOperator.NotEqual);
            Assert.AreEqual(7, (int) BinaryOperator.And);
            Assert.AreEqual(8, (int) BinaryOperator.Or);

            // Arithmetic
            Assert.AreEqual(9, (int) BinaryOperator.Plus);
            Assert.AreEqual(10, (int) BinaryOperator.Minus);
            Assert.AreEqual(11, (int) BinaryOperator.Multiply);
            Assert.AreEqual(12, (int) BinaryOperator.Divide);
            Assert.AreEqual(13, (int) BinaryOperator.Modulo);

            // Bitwise
            Assert.AreEqual(14, (int) BinaryOperator.BitwiseAnd);
            Assert.AreEqual(15, (int) BinaryOperator.BitwiseOr);
            Assert.AreEqual(16, (int) BinaryOperator.BitwiseXor);
            Assert.AreEqual(17, (int) BinaryOperator.ShiftLeft);
            Assert.AreEqual(18, (int) BinaryOperator.ShiftRight);
        }
    }
}