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
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Statements
{
    public class ReturnStatementTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new ReturnStatement();
            Assert.IsNull(sut.Expression);
        }

        [Test]
        public void SettingValues()
        {
            var sut = new ReturnStatement {Expression = new ConstantExpression()};
            Assert.AreEqual(new ConstantExpression(), sut.Expression);
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
            var a = new ReturnStatement {Expression = new ConstantExpression()};
            var b = new ReturnStatement {Expression = new ConstantExpression()};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_differentIdentifier()
        {
            var a = new ReturnStatement {Expression = new ConstantExpression()};
            var b = new ReturnStatement {Expression = new ComposedExpression()};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}