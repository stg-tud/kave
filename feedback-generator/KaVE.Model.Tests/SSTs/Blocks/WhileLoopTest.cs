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

using KaVE.Model.SSTs.Blocks;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Blocks
{
    public class WhileLoopTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new WhileLoop();
            Assert.IsNull(sut.Condition);
        }

        [Test]
        public void SettingValues()
        {
            var sut = new WhileLoop {Condition = new ConstantExpression()};
            Assert.AreEqual(new ConstantExpression(), sut.Condition);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new WhileLoop();
            var b = new WhileLoop();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new WhileLoop {Condition = new ConstantExpression()};
            var b = new WhileLoop {Condition = new ConstantExpression()};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentCondition()
        {
            var a = new WhileLoop {Condition = new ConstantExpression()};
            var b = new WhileLoop {Condition = new ComposedExpression()};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentBody()
        {
            var a = new WhileLoop();
            a.Body.Add(new ContinueStatement());
            var b = new WhileLoop();
            b.Body.Add(new GotoStatement());
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}