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

using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Statements
{
    public class LabelledStatementTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new LabelledStatement();
            Assert.AreEqual("", sut.Label);
            Assert.AreEqual(new UnknownStatement(), sut.Statement);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new LabelledStatement {Label = "a", Statement = new BreakStatement()};
            Assert.AreEqual("a", sut.Label);
            Assert.AreEqual(new BreakStatement(), sut.Statement);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new LabelledStatement();
            var b = new LabelledStatement();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new LabelledStatement {Label = "a", Statement = new BreakStatement()};
            var b = new LabelledStatement {Label = "a", Statement = new BreakStatement()};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentLabel()
        {
            var a = new LabelledStatement {Label = "a"};
            var b = new LabelledStatement {Label = "b"};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentStatement()
        {
            var a = new LabelledStatement {Statement = new ContinueStatement()};
            var b = new LabelledStatement {Statement = new BreakStatement()};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new LabelledStatement();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new LabelledStatement();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new LabelledStatement());
        }
    }
}