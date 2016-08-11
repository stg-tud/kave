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

using System.Linq;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Impl.v0.Types.Organization;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
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
            Assert.AreEqual(fullName, new PredefinedTypeName(id).FullName);
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
            var sut = new PredefinedTypeName(id);

            var structPart = sut.IsStructType ? "s:" : "";
            Assert.AreEqual(
                new TypeName("{0}{1}, mscorlib, {2}".FormatEx(structPart, fullName, new AssemblyVersion().Identifier)),
                sut.FullType);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldCrashWhenFullTypeIsRequestedFromArray()
        {
            // ReSharper disable once UnusedVariable
            var n = new PredefinedTypeName("p:int[]").FullType;
        }

        [TestCaseSource("PredefinedTypeSource")]
        public void ShouldRecognizeIdentifier(string shortName, string fullName, string id)
        {
            Assert.IsTrue(PredefinedTypeName.IsPredefinedTypeNameIdentifier(id));
            Assert.IsFalse(ArrayTypeName.IsArrayTypeNameIdentifier(id));
            Assert.IsFalse(TypeUtils.IsUnknownTypeIdentifier(id));
            Assert.IsFalse(TypeParameterName.IsTypeParameterNameIdentifier(id));
            Assert.IsFalse(DelegateTypeName.IsDelegateTypeNameIdentifier(id));
            Assert.IsFalse(TypeName.IsTypeNameIdentifier(id));
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

            Assert.IsFalse(sut.IsUnknown);
            Assert.IsFalse(sut.IsHashed);

            Assert.IsTrue(sut.IsPredefined);

            Assert.IsFalse(sut.IsArray);
            Assert.IsFalse(sut.IsDelegateType);
            Assert.IsFalse(sut.IsEnumType);
            Assert.IsFalse(sut.IsInterfaceType);
            Assert.IsFalse(sut.IsNestedType);
            Assert.Null(sut.DeclaringType);
            Assert.IsFalse(sut.IsNullableType);
            Assert.IsFalse(sut.IsTypeParameter);
            Assert.IsFalse(sut.HasTypeParameters);

            AssertIsTrueIf(sut.IsClassType, shortName, "object", "string");
            AssertIsTrueIf(sut.IsReferenceType, shortName, "object", "string");
            AssertIsFalseIf(sut.IsSimpleType, shortName, "object", "string", "void");
            AssertIsFalseIf(sut.IsStructType, shortName, "object", "string");
            AssertIsFalseIf(sut.IsValueType, shortName, "object", "string");
            AssertIsTrueIf(sut.IsVoidType, shortName, "void");
        }

        [TestCaseSource("PredefinedTypeSource")]
        public void ShouldParseArrays(string shortName, string fullName, string id)
        {
            if ("void".Equals(shortName))
            {
                return;
            }
            // ran kand basetype are tested in ArrayTypeNameTest
            foreach (var arrSuffix in new[] {"[]", "[,]"})
            {
                var sut = new PredefinedTypeName(id + arrSuffix);

                Assert.AreEqual(shortName + arrSuffix, sut.Name);
                Assert.AreEqual(fullName + arrSuffix, sut.FullName);
                Assert.AreEqual("[]".Equals(arrSuffix) ? 1 : 2, sut.Rank);

                Assert.IsFalse(sut.IsUnknown);
                Assert.IsFalse(sut.IsHashed);

                Assert.IsTrue(sut.IsArray);
                Assert.IsFalse(sut.IsClassType);
                Assert.IsFalse(sut.IsDelegateType);
                Assert.IsFalse(sut.IsEnumType);
                Assert.IsFalse(sut.IsInterfaceType);
                Assert.IsFalse(sut.IsNestedType);
                Assert.Null(sut.DeclaringType);
                Assert.IsFalse(sut.IsNullableType);
                Assert.IsFalse(sut.IsPredefined);
                Assert.IsTrue(sut.IsReferenceType);
                Assert.IsFalse(sut.IsSimpleType);
                Assert.IsFalse(sut.IsStructType);
                Assert.IsFalse(sut.IsTypeParameter);
                Assert.IsFalse(sut.IsValueType);
                Assert.IsFalse(sut.IsVoidType);
                Assert.IsFalse(sut.HasTypeParameters);
            }
        }

        [Test]
        public void ShouldReturnItselfOnConversion_Array()
        {
            var sut = new PredefinedTypeName("p:int[]");
            Assert.AreSame(sut, sut.AsArrayTypeName);
        }

        [Test]
        public void ShouldReturnItselfOnConversion_Predef()
        {
            var sut = new PredefinedTypeName("p:int");
            Assert.AreSame(sut, sut.AsPredefinedTypeName);
        }

        private static void AssertIsFalseIf(bool actual, string shortName, params string[] isFalseSet)
        {
            var expected = !isFalseSet.Contains(shortName);
            Assert.AreEqual(expected, actual);
        }

        private static void AssertIsTrueIf(bool actual, string shortName, params string[] isTrueSet)
        {
            var expected = isTrueSet.Contains(shortName);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldDifferentiatePredefinedTypeAndArray()
        {
            Assert.IsFalse(new PredefinedTypeName("p:int").IsArray);
            Assert.IsTrue(new PredefinedTypeName("p:int[]").IsArray);
            Assert.IsTrue(new PredefinedTypeName("p:int").IsPredefined);
            Assert.IsFalse(new PredefinedTypeName("p:int[]").IsPredefined);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldCrashIfConversionIsNotAppropriate_Array()
        {
            // ReSharper disable once UnusedVariable
            var n = new PredefinedTypeName("p:int[]").AsPredefinedTypeName;
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldCrashIfConversionIsNotAppropriate_NonArray()
        {
            // ReSharper disable once UnusedVariable
            var n = new PredefinedTypeName("p:int").AsArrayTypeName;
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldCrashIfConversionIsNotAppropriate_TypeParameter()
        {
            // ReSharper disable once UnusedVariable
            var n = new PredefinedTypeName("p:int").AsTypeParameterName;
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldCrashIfConversionIsNotAppropriate_Delegate()
        {
            // ReSharper disable once UnusedVariable
            var n = new PredefinedTypeName("p:int").AsDelegateTypeName;
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldCrashIfConversionIsNotAppropriate_FullType()
        {
            // ReSharper disable once UnusedVariable
            var n = new PredefinedTypeName("p:int[]").FullType;
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldRejectVoidArrays()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new PredefinedTypeName("p:void[]");
        }

        [ExpectedException(typeof(ValidationException)), TestCase("p:int["), TestCase("p:int]"), TestCase("p:int]["),
         TestCase(null)]
        public void ShouldRejectInvalidNames(string invalidId)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new PredefinedTypeName(invalidId);
        }
    }
}