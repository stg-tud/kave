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

using KaVE.Model.Collections;
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Expressions.LoopCondition;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Expressions
{
    internal class BlockExpressionTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new BlockExpression();
            Assert.NotNull(sut.Body);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new BlockExpression();
            sut.Body.Add(new ReturnStatement());

            var expected = Lists.NewList<IStatement>();
            expected.Add(new ReturnStatement());

            Assert.AreEqual(expected, sut.Body);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new BlockExpression();
            var b = new BlockExpression();

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_SettingValues()
        {
            var a = new BlockExpression();
            a.Body.Add(new ReturnStatement());
            var b = new BlockExpression();
            b.Body.Add(new ReturnStatement());

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentBody()
        {
            var a = new BlockExpression();
            a.Body.Add(new ReturnStatement());
            var b = new BlockExpression();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}