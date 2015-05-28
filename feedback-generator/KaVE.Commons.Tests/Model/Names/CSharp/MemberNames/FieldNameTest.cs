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
 *    - Sven Amann
 */

using KaVE.Commons.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Names.CSharp.MemberNames
{
    internal class FieldNameTest
    {
        private const string Identifier = "[b.ValueType, B, 1.3.3.7] [a.ValueType, A, 4.2.2.3].fieldName";

        [Test]
        public void ShouldImplementIsUnknown()
        {
            Assert.That(FieldName.UnknownName.IsUnknown);
        }

        [Test]
        public void ShouldBeInstanceFieldName()
        {
            var fieldName = FieldName.Get(Identifier);

            Assert.AreEqual(Identifier, fieldName.Identifier);
            Assert.AreEqual("fieldName", fieldName.Name);
            Assert.AreEqual("a.ValueType, A, 4.2.2.3", fieldName.DeclaringType.Identifier);
            Assert.AreEqual("b.ValueType, B, 1.3.3.7", fieldName.ValueType.Identifier);
            Assert.IsFalse(fieldName.IsStatic);
        }

        [Test]
        public void ShouldBeStaticFieldName()
        {
            var fieldName = FieldName.Get("static " + Identifier);

            Assert.IsTrue(fieldName.IsStatic);
        }

        [Test]
        public void ShouldBeFieldWithTypeParameters()
        {
            const string valueTypeIdentifier = "T`1[[A, B, 1.0.0.0]], A, 9.1.8.2";
            const string declaringTypeIdentifier = "U`2[[B, C, 6.7.5.8],[C, D, 8.3.7.4]], Z, 0.0.0.0";
            var fieldName = FieldName.Get("[" + valueTypeIdentifier + "] [" + declaringTypeIdentifier + "].bar");

            Assert.AreEqual("bar", fieldName.Name);
            Assert.AreEqual(valueTypeIdentifier, fieldName.ValueType.Identifier);
            Assert.IsTrue(fieldName.ValueType.HasTypeParameters);
            Assert.AreEqual(declaringTypeIdentifier, fieldName.DeclaringType.Identifier);
            Assert.IsTrue(fieldName.DeclaringType.HasTypeParameters);
        }

        [Test]
        public void ShouldBeUnknownField()
        {
            Assert.AreSame(TypeName.UnknownName, FieldName.UnknownName.ValueType);
            Assert.AreSame(TypeName.UnknownName, FieldName.UnknownName.DeclaringType);
            Assert.AreEqual("???", FieldName.UnknownName.Name);
        }

        [Test]
        public void HandlesDelegateValueType()
        {
            var fieldName = FieldName.Get("[d:[V,A] [D,A].()] [D,A].fieldName");

            Assert.AreEqual("fieldName", fieldName.Name);
            Assert.AreEqual("d:[V,A] [D,A].()", fieldName.ValueType.Identifier);
        }
    }
}