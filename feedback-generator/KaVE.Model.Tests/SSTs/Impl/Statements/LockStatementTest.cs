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

using KaVE.Model.SSTs.Impl;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.Impl.Statements;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Statements
{
    public class LockStatementTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new LockStatement();
            Assert.IsNull(sut.Reference);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new LockStatement {Reference = new VariableReference {Identifier = "x"}};
            Assert.AreEqual(new VariableReference {Identifier = "x"}, sut.Reference);
        }

        [Test]
        public void Equality_default()
        {
            var a = new LockStatement();
            var b = new LockStatement();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_reallyTheSame()
        {
            var a = SSTUtil.LockStatement("a");
            var b = SSTUtil.LockStatement("a");
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_differentIdentifier()
        {
            var a = SSTUtil.LockStatement("a");
            var b = SSTUtil.LockStatement("b");
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new LockStatement();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new LockStatement();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }
    }
}