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
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Statements
{
    internal class EventSubscriptionStatementTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new EventSubscriptionStatement();
            Assert.AreEqual(new UnknownReference(), sut.Reference);
            Assert.AreEqual(EventSubscriptionOperation.Add, sut.Operation);
            Assert.AreEqual(new UnknownExpression(), sut.Expression);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new EventSubscriptionStatement
            {
                Reference = new VariableReference {Identifier = "x"},
                Operation = EventSubscriptionOperation.Remove,
                Expression = new ConstantValueExpression()
            };
            Assert.AreEqual(new VariableReference {Identifier = "x"}, sut.Reference);
            Assert.AreEqual(EventSubscriptionOperation.Remove, sut.Operation);
            Assert.AreEqual(new ConstantValueExpression(), sut.Expression);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new EventSubscriptionStatement();
            var b = new EventSubscriptionStatement();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new EventSubscriptionStatement
            {
                Reference = new VariableReference {Identifier = "x"},
                Operation = EventSubscriptionOperation.Remove,
                Expression = new ConstantValueExpression()
            };
            var b = new EventSubscriptionStatement
            {
                Reference = new VariableReference {Identifier = "x"},
                Operation = EventSubscriptionOperation.Remove,
                Expression = new ConstantValueExpression()
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentReference()
        {
            var a = new EventSubscriptionStatement
            {
                Reference = new VariableReference {Identifier = "x"}
            };
            var b = new EventSubscriptionStatement();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentOperation()
        {
            var a = new EventSubscriptionStatement
            {
                Operation = EventSubscriptionOperation.Remove
            };
            var b = new EventSubscriptionStatement();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentExpression()
        {
            var a = new EventSubscriptionStatement
            {
                Expression = new ConstantValueExpression()
            };
            var b = new EventSubscriptionStatement();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new EventSubscriptionStatement();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new EventSubscriptionStatement();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new EventSubscriptionStatement());
        }

        [Test]
        public void NumberingOfEnumIsStable()
        {
            Assert.AreEqual(0, (int) EventSubscriptionOperation.Add);
            Assert.AreEqual(1, (int) EventSubscriptionOperation.Remove);
        }
    }
}