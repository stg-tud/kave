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

using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Impl.v0.Types.Organization;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Exceptions;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.Types
{
    internal class PredefinedTypeNameTest
    {
        private static string[][] PredefinedTypeSource()
        {
            return new[]
            {
                new[] {"sbyte", "System.SByte", "p:sbyte"},
                new[] {"byte", "System.Byte", "p:byte"},
                new[] {"short", "System.Int16", "p:short"},
                new[] {"ushort", "System.UInt16", "p:ushort"},
                new[] {"int", "System.Int32", "p:int"},
                new[] {"uint", "System.UInt32", "p:uint"},
                new[] {"long", "System.Int64", "p:long"},
                new[] {"ulong", "System.UInt64", "p:ulong"},
                new[] {"char", "System.Char", "p:char"},
                new[] {"float", "System.Single", "p:float"},
                new[] {"double", "System.Double", "p:double"},
                new[] {"bool", "System.Boolean", "p:bool"},
                new[] {"decimal", "System.Decimal", "p:decimal"},
                new[] {"void", "System.Void", "p:void"},
                new[] {"object", "System.Object", "p:object"},
                new[] {"string", "System.String", "p:string"}
            };
        }

        [TestCaseSource("PredefinedTypeSource")]
        public void ShouldParseShortName(string shortName, string fullName, string id)
        {
            Assert.AreEqual(shortName, new PredefinedTypeName(id).Name);
        }

        [TestCaseSource("PredefinedTypeSource")]
        public void ShouldParseFullName(string shortName, string fullName, string id)
        {
            Assert.AreEqual(shortName, new PredefinedTypeName(id).FullName);
        }

        [TestCaseSource("PredefinedTypeSource")]
        public void ShouldParseNamespace(string shortName, string fullName, string id)
        {
            Assert.AreEqual(new NamespaceName("System"), new PredefinedTypeName(id).Namespace);
        }

        [TestCaseSource("PredefinedTypeSource")]
        public void ShouldParseAssembly(string shortName, string fullName, string id)
        {
            Assert.AreEqual(
                new AssemblyName("mscorlib, {0}".FormatEx(new AssemblyVersion().Identifier)),
                new PredefinedTypeName(id).Assembly);
        }

        [TestCaseSource("PredefinedTypeSource")]
        public void ShouldReturnFullType(string shortName, string fullName, string id)
        {
            Assert.AreEqual(
                new TypeName("{0}, mscorlib, {1}".FormatEx(fullName, new AssemblyVersion().Identifier)),
                new PredefinedTypeName(id).FullType);
        }

        [TestCaseSource("PredefinedTypeSource")]
        public void ShouldRecognizeIdentifier(string shortName, string fullName, string id)
        {
            Assert.IsTrue(PredefinedTypeName.IsPredefinedTypeNameIdentifier(id));
            Assert.IsFalse(ArrayTypeName.IsArrayTypeNameIdentifier(id));
            Assert.IsFalse(TypeUtils.IsUnknownTypeIdentifier(id));
            Assert.IsFalse(TypeParameterName.IsTypeParameterNameIdentifier(id));
            Assert.IsFalse(DelegateTypeName.IsDelegateTypeNameIdentifier(id));
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldRejectUnknownIds()
        {
            const string invalidId = "p:...";
            Assert.IsFalse(PredefinedTypeName.IsPredefinedTypeNameIdentifier(invalidId));
            new PredefinedTypeName(invalidId);
        }

        [TestCaseSource("PredefinedTypeSource")]
        public void DefaultValues(string shortName, string fullName, string id)
        {
            var sut = new PredefinedTypeName(id);

            Assert.IsTrue(sut.IsUnknown);
            Assert.IsTrue(sut.IsHashed);

            Assert.IsTrue(sut.IsArray);
            Assert.IsTrue(sut.IsClassType);
            Assert.IsTrue(sut.IsDelegateType);
            Assert.IsTrue(sut.IsEnumType);
            Assert.IsTrue(sut.IsInterfaceType);
            Assert.IsTrue(sut.IsNestedType);
            Assert.IsTrue(sut.IsNullableType);
            Assert.IsTrue(sut.IsPredefined);
            Assert.IsTrue(sut.IsReferenceType);
            Assert.IsTrue(sut.IsSimpleType);
            Assert.IsTrue(sut.IsStructType);
            Assert.IsTrue(sut.IsTypeParameter);
            Assert.IsTrue(sut.IsValueType);
            Assert.IsTrue(sut.IsVoidType);

            Assert.IsTrue(sut.HasTypeParameters);

            Assert.AreSame(sut, sut.AsArrayTypeName);
            Assert.AreSame(sut, sut.AsPredefinedTypeName);
        }

        [Test]
        public void ShouldDifferentiatePreDefTypeAndArray(string shortName, string fullName, string id)
        {
            Assert.IsFalse(new PredefinedTypeName("p:int").IsArray);
            Assert.IsTrue(new PredefinedTypeName("p:int[]").IsArray);
            Assert.IsTrue(new PredefinedTypeName("p:int").IsPredefined);
            Assert.IsFalse(new PredefinedTypeName("p:int[]").IsPredefined);
        }
    }
}