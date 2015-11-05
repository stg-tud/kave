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

using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Declarations
{
    internal class FieldDeclarationTest
    {
        private static IFieldName SomeField
        {
            get { return FieldName.Get("[T1,P1] [T2,P2].Field"); }
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new FieldDeclaration();
            Assert.AreEqual(FieldName.UnknownName, sut.Name);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new FieldDeclaration {Name = SomeField};
            Assert.AreEqual(SomeField, sut.Name);
        }

        [Test]
        public void DeclaringTypeIdentity()
        {
            var sut = new FieldDeclaration
            {
                Name = SomeField
            };

            Assert.AreEqual(new TypeReference { TypeName = SomeField.DeclaringType }, sut.DeclaringType);
        }

        [Test]
        public void FieldNameIdentity()
        {
            var sut = new FieldDeclaration
            {
                Name = SomeField
            };

            Assert.AreEqual(new SimpleName { Name= SomeField.Name}, sut.FieldName);
        }

        [Test]
        public void ValueTypeIdentity()
        {
            var sut = new FieldDeclaration
            {
                Name = SomeField
            };

            Assert.AreEqual(new TypeReference { TypeName = SomeField.ValueType }, sut.ValueType);
        }

        [Test]
        public void IsStaticIdentity()
        {
            var sut = new FieldDeclaration
            {
                Name = FieldName.Get("static [T1,P1] [T2,P2].Field")
            };

            Assert.AreEqual(true, sut.IsStatic);
        }

        [Test]
        public void DeclaringTypeIsCached()
        {
            var sut = new FieldDeclaration
            {
                Name = SomeField
            };

            var declaringType = sut.DeclaringType;

            Assert.True(ReferenceEquals(declaringType, sut.DeclaringType));
        }

        [Test]
        public void FieldNameIsCached()
        {
            var sut = new FieldDeclaration
            {
                Name = SomeField
            };

            var fieldName = sut.FieldName;

            Assert.True(ReferenceEquals(fieldName, sut.FieldName));
        }

        [Test]
        public void ValueTypeIsCached()
        {
            var sut = new FieldDeclaration
            {
                Name = SomeField
            };

            var valueType = sut.ValueType;

            Assert.True(ReferenceEquals(valueType, sut.ValueType));
        }

        [Test]
        public void Equality_Default()
        {
            var a = new FieldDeclaration();
            var b = new FieldDeclaration();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyEquals()
        {
            var a = new FieldDeclaration {Name = SomeField};
            var b = new FieldDeclaration {Name = SomeField};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentType()
        {
            var a = new FieldDeclaration {Name = SomeField};
            var b = new FieldDeclaration();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new FieldDeclaration();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new FieldDeclaration();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new FieldDeclaration());
        }
    }
}