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
 *    - Sebastian Proksch
 */

using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Expressions.Basic;
using KaVE.Model.SSTs.Statements.Wrapped;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Statements.Wrapped
{
    public class ExpressionStatementTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new TestStatement();
            Assert.IsNull(sut.Target);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new TestStatement {Target = new ConstantValueExpression()};
            Assert.AreEqual(new ConstantValueExpression(), sut.Target);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new TestStatement();
            var b = new TestStatement();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new TestStatement {Target = new ConstantValueExpression()};
            var b = new TestStatement {Target = new ConstantValueExpression()};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTarget()
        {
            var a = new TestStatement {Target = new ConstantValueExpression()};
            var b = new TestStatement();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }

    internal class TestStatement : ExpressionStatement {}
}