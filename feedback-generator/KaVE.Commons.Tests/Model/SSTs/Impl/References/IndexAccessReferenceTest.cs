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
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.References
{
    public class IndexAccessReferenceTest
    {
        private static IIndexAccessExpression SomeIndexAccess
        {
            get
            {
                return new IndexAccessExpression
                {
                    Reference = new VariableReference {Identifier = "arr"},
                    Indices = {new ConstantValueExpression()}
                };
            }
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new IndexAccessReference();
            Assert.AreEqual(new IndexAccessExpression(), sut.Expression);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new IndexAccessReference {Expression = SomeIndexAccess};
            Assert.AreEqual(SomeIndexAccess, sut.Expression);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new IndexAccessReference();
            var b = new IndexAccessReference();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new IndexAccessReference {Expression = SomeIndexAccess};
            var b = new IndexAccessReference {Expression = SomeIndexAccess};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentName()
        {
            var a = new IndexAccessReference {Expression = SomeIndexAccess};
            var b = new IndexAccessReference();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new IndexAccessReference();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new IndexAccessReference();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new FieldReference());
        }
    }
}