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
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.Types
{
    internal class ArrayTypeNameTest
    {
        private static string[][] ArrayIds()
        {
            // base type, arr1D, arr2d
            string[][] names =
            {
                new[] {"?", "?[]", "?[,]"}, // unknown
                new[] {"p:int", "p:int[]", "p:int[,]"}, // predefined type
                new[] // nullable
                {
                    "System.Nullable`1[[System.Int32, mscorlib, 1.2.3.4]], mscorlib, 1.2.3.4",
                    "System.Nullable`1[][[System.Int32, mscorlib, 1.2.3.4]], mscorlib, 1.2.3.4",
                    "System.Nullable`1[,][[System.Int32, mscorlib, 1.2.3.4]], mscorlib, 1.2.3.4"
                },
                new[] {"p:int", "p:int[]", "p:int[,]"}, // predefined type
                new[] {"T, P", "T[], P", "T[,], P"}, // regular class type (project specific)
                new[] {"A, B, 1.2.3.4", "A[], B, 1.2.3.4", "A[,], B, 1.2.3.4"}, // regular class type (assembly)
                new[] {"s:S, P", "s:S[], P", "s:S[,], P"}, // struct
                new[] {"T", "T[]", "T[,]"}, // type parameter (free)
                new[] {"T -> ?", "T[] -> ?", "T[,] -> ?"}, // type parameter (bound)
                new[] {"GT`1[[T -> PT, A]], A", "GT`1[][[T -> PT, A]], A", "GT`1[,][[T -> PT, A]], A"},
                new[] {"d:[RT, A] [DT, A].()", "d:[RT, A] [DT, A].()[]", "d:[RT, A] [DT, A].()[,]"},
                new[] // delegate
                {
                    "d:[RT[], A] [DT, A].([PT[], A] p)",
                    "d:[RT[], A] [DT, A].([PT[], A] p)[]",
                    "d:[RT[], A] [DT, A].([PT[], A] p)[,]"
                },
                new[]
                {
                    "T`1[[T -> d:[TR] [T2, P2].([T] arg)]], P",
                    "T`1[][[T -> d:[TR] [T2, P2].([T] arg)]], P",
                    "T`1[,][[T -> d:[TR] [T2, P2].([T] arg)]], P"
                },
                new[] {"n.C1+C2,P", "n.C1+C2[],P", "n.C1+C2[,],P"}, // nested
                new[] {"C1`1[[T1]],P", "C1`1[][[T1]],P", "C1`1[,][[T1]],P"}, // generic
                new[] {"C1+C2`1[[T2]],P", "C1+C2`1[][[T2]],P", "C1+C2`1[,][[T2]],P"}, // nested generic
                new[] {"C1`1[[T2]]+C2,P", "C1`1[[T2]]+C2[],P", "C1`1[[T2]]+C2[,],P"}, // generic nested
                new[] {"C1`1[[T1]]+C2`1[[T2]],P", "C1`1[[T1]]+C2`1[][[T2]],P", "C1`1[[T1]]+C2`1[,][[T2]],P"},
                // nested generic+generic
                new[] {"T -> T[],P", "T[] -> T[],P", "T[,] -> T[],P"}, // type parameter bound to array
                new[] // the most arrays I could imagine
                {
                    "T1`1[][[T2 -> T3[],P]]+T4`1[][[T5 -> T6[],P]]+T7`1[[T8 -> T9[],P]], P",
                    "T1`1[][[T2 -> T3[],P]]+T4`1[][[T5 -> T6[],P]]+T7`1[][[T8 -> T9[],P]], P",
                    "T1`1[][[T2 -> T3[],P]]+T4`1[][[T5 -> T6[],P]]+T7`1[,][[T8 -> T9[],P]], P"
                }
            };
            return names;
        }

        [TestCaseSource("ArrayIds")]
        public void ShouldDerive1DArray(string baseTypeId, string expected1DId, string expected2DId)
        {
            var baseType = TypeUtils.CreateTypeName(baseTypeId);
            var expected1D = TypeUtils.CreateTypeName(expected1DId);
            var actual1D = ArrayTypeName.From(baseType, 1);
            Assert.AreEqual(expected1D, actual1D);
        }

        [TestCaseSource("ArrayIds")]
        public void ShouldDerive2DArray(string baseTypeId, string expected1DId, string expected2DId)
        {
            var baseType = TypeUtils.CreateTypeName(baseTypeId);
            var expected2D = TypeUtils.CreateTypeName(expected2DId);
            var actual2D = ArrayTypeName.From(baseType, 2);
            Assert.AreEqual(expected2D, actual2D);
        }

        [TestCaseSource("ArrayIds")]
        public void ShouldDerive1To2DArray(string baseTypeId, string expected1DId, string expected2DId)
        {
            var expected1D = TypeUtils.CreateTypeName(expected1DId);
            var expected2D = TypeUtils.CreateTypeName(expected2DId);
            var actual1To2D = ArrayTypeName.From(expected1D, 1);
            Assert.AreEqual(expected2D, actual1To2D);
        }

        [Test]
        public void ShouldDeriveTypeParameterFromTypeParameter()
        {
            var actual = ArrayTypeName.From(new TypeParameterName("T"), 1);
            var expected = new TypeParameterName("T[]");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldDerivePredefinedFromPredefined()
        {
            var actual = ArrayTypeName.From(new PredefinedTypeName("p:int"), 1);
            var expected = new PredefinedTypeName("p:int[]");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldDeriveUnknownArray()
        {
            var actual = ArrayTypeName.From(new TypeName(), 1);
            var expected = new ArrayTypeName("?[]");
            Assert.AreEqual(expected, actual);
        }

        [TestCaseSource("ArrayIds")]
        public void ShouldParseRank(string baseTypeId, string expected1DId, string expected2DId)
        {
            var arr1D = TypeUtils.CreateTypeName(expected1DId).AsArrayTypeName;
            var arr2D = TypeUtils.CreateTypeName(expected2DId).AsArrayTypeName;
            Assert.AreEqual(1, arr1D.Rank);
            Assert.AreEqual(2, arr2D.Rank);
        }

        [TestCaseSource("ArrayIds")]
        public void ShouldParseBaseType(string baseTypeId, string expected1DId, string expected2DId)
        {
            var baseType = TypeUtils.CreateTypeName(baseTypeId);
            foreach (var id in new[] {expected1DId, expected2DId})
            {
                var arr = TypeUtils.CreateTypeName(id).AsArrayTypeName;
                Assert.AreEqual(baseType, arr.ArrayBaseType);
            }
        }

        [TestCaseSource("ArrayIds")]
        public void IsArray(string baseTypeId, string expected1DId, string expected2DId)
        {
            Assert.IsFalse(TypeUtils.CreateTypeName(baseTypeId).IsArray);
            foreach (var id in new[] {expected1DId, expected2DId})
            {
                Assert.IsTrue(TypeUtils.CreateTypeName(id).IsArray);
            }
        }

        [TestCaseSource("ArrayIds")]
        public void IsArrayName(string baseTypeId, string expected1DId, string expected2DId)
        {
            Assert.IsFalse(ArrayTypeName.IsArrayTypeNameIdentifier(baseTypeId));
            foreach (var arrId in new[] {expected1DId, expected2DId})
            {
                if (TypeParameterName.IsTypeParameterNameIdentifier(baseTypeId))
                {
                    Assert.IsTrue(TypeParameterName.IsTypeParameterNameIdentifier(arrId));
                    Assert.IsFalse(TypeName.IsTypeNameIdentifier(arrId));
                    Assert.IsFalse(ArrayTypeName.IsArrayTypeNameIdentifier(arrId));
                    Assert.IsFalse(DelegateTypeName.IsDelegateTypeNameIdentifier(arrId));
                    Assert.IsFalse(PredefinedTypeName.IsPredefinedTypeNameIdentifier(arrId));
                    Assert.IsFalse(TypeUtils.IsUnknownTypeIdentifier(arrId));
                }
                else if (PredefinedTypeName.IsPredefinedTypeNameIdentifier(baseTypeId))
                {
                    Assert.IsTrue(PredefinedTypeName.IsPredefinedTypeNameIdentifier(arrId));
                    Assert.IsFalse(TypeName.IsTypeNameIdentifier(arrId));
                    Assert.IsFalse(ArrayTypeName.IsArrayTypeNameIdentifier(arrId));
                    Assert.IsFalse(TypeParameterName.IsTypeParameterNameIdentifier(arrId));
                    Assert.IsFalse(DelegateTypeName.IsDelegateTypeNameIdentifier(arrId));
                    Assert.IsFalse(TypeUtils.IsUnknownTypeIdentifier(arrId));
                }
                else
                {
                    Assert.IsTrue(ArrayTypeName.IsArrayTypeNameIdentifier(arrId));
                    Assert.IsFalse(TypeName.IsTypeNameIdentifier(arrId));
                    Assert.IsFalse(TypeParameterName.IsTypeParameterNameIdentifier(arrId));
                    Assert.IsFalse(DelegateTypeName.IsDelegateTypeNameIdentifier(arrId));
                    Assert.IsFalse(PredefinedTypeName.IsPredefinedTypeNameIdentifier(arrId));
                    Assert.IsFalse(TypeUtils.IsUnknownTypeIdentifier(arrId));
                }
            }
        }

        [TestCaseSource("ArrayIds")]
        public void ArraysAreNothingElse(string baseTypeId, string expected1DId, string expected2DId)
        {
            foreach (var id in new[] {expected1DId, expected2DId})
            {
                var sut = TypeUtils.CreateTypeName(id);

                Assert.IsFalse(sut.IsClassType);
                Assert.IsFalse(sut.IsDelegateType);
                Assert.IsFalse(sut.IsEnumType);
                Assert.IsFalse(sut.IsInterfaceType);
                Assert.IsFalse(sut.IsNestedType);
                Assert.IsFalse(sut.IsNullableType);
                Assert.IsTrue(sut.IsReferenceType);
                Assert.IsTrue(sut.IsArray);
                Assert.IsFalse(sut.IsSimpleType);
                Assert.IsFalse(sut.IsStructType);
                Assert.IsFalse(sut.IsValueType);
                Assert.IsFalse(sut.IsVoidType);

                Assert.IsFalse(sut.IsTypeParameter);
                Assert.IsFalse(sut.IsDelegateType);
                Assert.IsFalse(sut.IsPredefined);

                Assert.IsFalse(TypeName.IsTypeNameIdentifier(id));
                Assert.IsFalse(TypeUtils.IsUnknownTypeIdentifier(id));
                Assert.IsFalse(DelegateTypeName.IsDelegateTypeNameIdentifier(id));
                if (TypeParameterName.IsTypeParameterNameIdentifier(baseTypeId))
                {
                    Assert.IsTrue(TypeParameterName.IsTypeParameterNameIdentifier(id));
                }
                if (PredefinedTypeName.IsPredefinedTypeNameIdentifier(baseTypeId))
                {
                    Assert.IsTrue(PredefinedTypeName.IsPredefinedTypeNameIdentifier(id));
                }
                if (DelegateTypeName.IsDelegateTypeNameIdentifier(baseTypeId))
                {
                    Assert.IsFalse(DelegateTypeName.IsDelegateTypeNameIdentifier(id));
                }
            }
        }

        [TestCaseSource("ArrayIds")]
        public void ShouldHaveArrayBracesInName(string baseTypeId, string expected1DId, string expected2DId)
        {
            var arr1D = TypeUtils.CreateTypeName(expected1DId);
            Assert.IsTrue(arr1D.Name.EndsWith("[]"));

            var arr2D = TypeUtils.CreateTypeName(expected2DId);
            Assert.IsTrue(arr2D.Name.EndsWith("[,]"));
        }

        [Test]
        public void TypeParameterParsingIsCached()
        {
            var sut = new ArrayTypeName("T[],P");
            var a = sut.TypeParameters;
            var b = sut.TypeParameters;
            Assert.AreSame(a, b);
        }

        [TestCase("T[[T0]],P"), TestCase("TT0],P")]
        public void ShouldNotCrashForInvalidNames(string invalidId)
        {
            Assert.IsFalse(ArrayTypeName.IsArrayTypeNameIdentifier(invalidId));
            Assert.IsFalse(TypeParameterName.IsTypeParameterNameIdentifier(invalidId));
            Assert.IsFalse(DelegateTypeName.IsDelegateTypeNameIdentifier(invalidId));
            Assert.IsFalse(PredefinedTypeName.IsPredefinedTypeNameIdentifier(invalidId));
            Assert.IsFalse(TypeUtils.IsUnknownTypeIdentifier(invalidId));
            Assert.IsFalse(TypeName.IsTypeNameIdentifier(invalidId));
        }

        [Test]
        public void ShouldRecognizeUnknownArrays()
        {
            Assert.IsFalse(ArrayTypeName.IsArrayTypeNameIdentifier("?"));
            Assert.IsTrue(ArrayTypeName.IsArrayTypeNameIdentifier("?[]"));
            Assert.IsTrue(ArrayTypeName.IsArrayTypeNameIdentifier("?[,]"));
            // unknown array is nested somewhere
            Assert.IsFalse(ArrayTypeName.IsArrayTypeNameIdentifier("d:[?] [?].([?[]] p)"));
        }
    }
}