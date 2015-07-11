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

using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Blocks
{
    internal class CatchBlockTest : SSTBaseTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new CatchBlock();
            Assert.AreEqual(ParameterName.UnknownName, sut.Parameter);
            Assert.AreEqual(Lists.NewList<IStatement>(), sut.Body);
            Assert.False(sut.IsGeneral);
            Assert.False(sut.IsUnnamed);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new CatchBlock
            {
                Parameter = SomeParameter(),
                IsGeneral = true,
                IsUnnamed = true,
                Body =
                {
                    new ReturnStatement()
                }
            };

            Assert.AreEqual(SomeParameter(), sut.Parameter);
            Assert.AreEqual(Lists.NewList(new ReturnStatement()), sut.Body);
            Assert.True(sut.IsGeneral);
            Assert.True(sut.IsUnnamed);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new CatchBlock();
            var b = new CatchBlock();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new CatchBlock
            {
                Parameter = SomeParameter(),
                IsGeneral = true,
                IsUnnamed = true,
                Body =
                {
                    new ReturnStatement()
                }
            };
            var b = new CatchBlock
            {
                Parameter = SomeParameter(),
                IsGeneral = true,
                IsUnnamed = true,
                Body =
                {
                    new ReturnStatement()
                }
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentParameter()
        {
            var a = new CatchBlock {Parameter = SomeParameter()};
            var b = new CatchBlock();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentBody()
        {
            var a = new CatchBlock
            {
                Body =
                {
                    new ReturnStatement()
                }
            };
            var b = new CatchBlock();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentIsGeneral()
        {
            var a = new CatchBlock {IsGeneral = true};
            var b = new CatchBlock();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentIsUnnamed()
        {
            var a = new CatchBlock {IsUnnamed = true};
            var b = new CatchBlock();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new CatchBlock());
        }
    }
}