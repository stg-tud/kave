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

using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Statements
{
    public class ThrowStatementTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new ThrowStatement();
            Assert.AreEqual(new VariableReference(), sut.Reference);
            Assert.IsTrue(sut.IsReThrow);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var varRef = new VariableReference {Identifier = "e"};
            var sut = new ThrowStatement {Reference = varRef};
            Assert.AreEqual(varRef, sut.Reference);
            Assert.IsFalse(sut.IsReThrow);
        }

        [Test]
        public void Equality_default()
        {
            var a = new ThrowStatement();
            var b = new ThrowStatement();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_reallyTheSame()
        {
            var varRef = new VariableReference {Identifier = "e"};
            var a = new ThrowStatement {Reference = varRef};
            var b = new ThrowStatement {Reference = varRef};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_differentException()
        {
            var a = new ThrowStatement {Reference = new VariableReference {Identifier = "e1"}};
            var b = new ThrowStatement {Reference = new VariableReference {Identifier = "e2"}};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new ThrowStatement();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new ThrowStatement();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new ThrowStatement());
        }
    }
}