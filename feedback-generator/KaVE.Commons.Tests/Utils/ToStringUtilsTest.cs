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

using System;
using System.Collections.Generic;
using KaVE.Commons.Utils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils
{
    public class ToStringUtilsTest
    {
        [SetUp]
        public void Setup()
        {
            CultureUtils.SetDefaultCultureForThisThread();
        }

        [Test]
        public void DifferentVisibilities()
        {
            var sut = new ClassWithDifferentVisibilities();
            var actual = sut.ToStringReflection();
            const string expected = "ClassWithDifferentVisibilities@5 {\n" +
                                    "   _privateField = 1,\n" +
                                    "   PublicField = 2,\n" +
                                    "   PrivateProperty = 3,\n" +
                                    "   PublicProperty = 4,\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DifferentKindsOfProperties()
        {
            var sut = new ClassWithDifferentKindsOfProperties();
            var actual = sut.ToStringReflection();
            const string expected = "ClassWithDifferentKindsOfProperties@7 {\n" +
                                    "   AutoProperty = 6,\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FieldsInHierarchy()
        {
            var sut = new ClassWithHierarchy();
            var actual = sut.ToStringReflection();
            const string expected = "ClassWithHierarchy@9 {\n" +
                                    "   Num = 8,\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShadowedFieldsInHierarchy()
        {
            var sut = new ClassWitShadowedField();
            var actual = sut.ToStringReflection();
            const string expected = "ClassWitShadowedField@11 {\n" +
                                    "   Num = 10,\n" +
                                    "   Num = 8,\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NullSafe()
        {
            var actual = ((object) null).ToStringReflection();
            const string expected = @"null";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Structs()
        {
            var actual = new SomeStruct {Num = 3}.ToStringReflection();
            const string expected = "SomeStruct@15 {\n" +
                                    "   Num = 3,\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OnPrimitives()
        {
            Assert.AreEqual("1", 1.ToStringReflection());
            Assert.AreEqual("True", true.ToStringReflection());
        }

        [Test]
        public void Arrays()
        {
            var actual = new[] {1, 2, 3}.ToStringReflection();
            const string expected = "[\n" +
                                    "   1,\n" +
                                    "   2,\n" +
                                    "   3,\n" +
                                    "]";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Enumerables()
        {
            var actual = new SomeList<int> {1, 2, 3}.ToStringReflection();
            const string expected = "SomeList`1@16 [\n" +
                                    "   1,\n" +
                                    "   2,\n" +
                                    "   3,\n" +
                                    "]";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void EnumerableWithNull()
        {
            var actual = new SomeList<object> {new ClassWithToString(), null}.ToStringReflection();
            const string expected = "SomeList`1@16 [\n" +
                                    "   {\n" +
                                    "      XYZ\n" +
                                    "   },\n" +
                                    "   null,\n" +
                                    "]";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Nested_WithToString()
        {
            var actual = new ClassWithNesting
            {
                HashCode = 1,
                Nested = new ClassWithToString()
            }.ToStringReflection();
            const string expected = "ClassWithNesting@1 {\n" +
                                    "   HashCode = 1,\n" +
                                    "   Nested = {\n" +
                                    "      XYZ\n" +
                                    "   },\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Nested_WithThrowingToString()
        {
            var actual = new ClassWithNesting
            {
                HashCode = 1,
                Nested = new ClassWithThrowingToString()
            }.ToStringReflection();
            Assert.That(actual.StartsWith("ToString reflection failed for 'ClassWithNesting@1': a cause\n"));
            Assert.That(
                actual.Contains("at KaVE.Commons.Tests.Utils.ToStringUtilsTest.ClassWithThrowingToString.ToString()"));
        }

        [Test]
        public void Nested_WithoutToString()
        {
            var actual = new ClassWithNesting
            {
                HashCode = 1,
                Nested = new ClassWithNesting {HashCode = 2}
            }.ToStringReflection();
            const string expected = "ClassWithNesting@1 {\n" +
                                    "   HashCode = 1,\n" +
                                    "   Nested = KaVE.Commons.Tests.Utils.ToStringUtilsTest+ClassWithNesting,\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Nested_Null()
        {
            var actual = new ClassWithNesting
            {
                HashCode = 1,
                Nested = null
            }.ToStringReflection();
            const string expected = "ClassWithNesting@1 {\n" +
                                    "   HashCode = 1,\n" +
                                    "   Nested = null,\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Nested_Array()
        {
            var actual = new ClassWithNesting
            {
                HashCode = 1,
                Nested = new[] {1, 2, 3}
            }.ToStringReflection();
            const string expected = "ClassWithNesting@1 {\n" +
                                    "   HashCode = 1,\n" +
                                    "   Nested = [\n" +
                                    "      1,\n" +
                                    "      2,\n" +
                                    "      3,\n" +
                                    "   ],\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Nested_Enumerable()
        {
            var actual = new ClassWithNesting
            {
                HashCode = 1,
                Nested = new SomeList<int> {1, 2, 3}
            }.ToStringReflection();
            const string expected = "ClassWithNesting@1 {\n" +
                                    "   HashCode = 1,\n" +
                                    "   Nested = SomeList`1@16 [\n" +
                                    "      1,\n" +
                                    "      2,\n" +
                                    "      3,\n" +
                                    "   ],\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Nested_EnumerableWithNull()
        {
            var actual = new ClassWithNesting
            {
                HashCode = 1,
                Nested = new SomeList<object> {new ClassWithToString(), null}
            }.ToStringReflection();
            const string expected = "ClassWithNesting@1 {\n" +
                                    "   HashCode = 1,\n" +
                                    "   Nested = SomeList`1@16 [\n" +
                                    "      {\n" +
                                    "         XYZ\n" +
                                    "      },\n" +
                                    "      null,\n" +
                                    "   ],\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Nested_Struct()
        {
            var actual = new ClassWithNesting
            {
                HashCode = 1,
                Nested = new SomeStruct()
            }.ToStringReflection();
            const string expected = "ClassWithNesting@1 {\n" +
                                    "   HashCode = 1,\n" +
                                    "   Nested = KaVE.Commons.Tests.Utils.ToStringUtilsTest+SomeStruct,\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RecursionPrevension()
        {
            var sut = new ClassWithNesting {HashCode = 1};
            sut.Nested = sut;
            var actual = sut.ToStringReflection();
            const string expected = "ClassWithNesting@1 {\n" +
                                    "   HashCode = 1,\n" +
                                    "   Nested = {--> ClassWithNesting@1},\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RecursionPrevensionInEnumerables()
        {
            var someList = new SomeList<object>();
            var sut = new ClassWithNesting {HashCode = 1, Nested = someList};
            someList.Add(sut);
            var actual = sut.ToStringReflection();
            const string expected = "ClassWithNesting@1 {\n" +
                                    "   HashCode = 1,\n" +
                                    "   Nested = SomeList`1@16 [\n" +
                                    "      {--> ClassWithNesting@1},\n" +
                                    "   ],\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test, Ignore("idea for enhancement")]
        public void PreventDoublePrinting()
        {
            var sut = new ClassWithNesting
            {
                HashCode = 1,
                Nested = new SomeList<object>
                {
                    new SomeStruct {Num = 1},
                    new SomeStruct {Num = 1}
                }
            };
            var actual = sut.ToStringReflection();
            const string expected = "ClassWithNesting@1 {\n" +
                                    "   HashCode = 1,\n" +
                                    "   Nested = SomeList`1@16 [\n" +
                                    "      SomeStruct@1 {\n" +
                                    "         Num = 1,\n" +
                                    "      },\n" +
                                    "      {--> SomeStruct@1},\n" +
                                    "   },\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void StringIsPrintedAsString()
        {
            var actual = "a".ToStringReflection();
            const string expected = "a";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NestedStringIsPrintedAsString()
        {
            var actual = new ClassWithNesting
            {
                HashCode = 1,
                Nested = "a"
            }.ToStringReflection();
            const string expected = "ClassWithNesting@1 {\n" +
                                    "   HashCode = 1,\n" +
                                    "   Nested = \"a\",\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NestedStringInEnumerableIsPrintedAsString()
        {
            var actual = new ClassWithNesting
            {
                HashCode = 1,
                Nested = new SomeList<object> {"a"}
            }.ToStringReflection();
            const string expected = "ClassWithNesting@1 {\n" +
                                    "   HashCode = 1,\n" +
                                    "   Nested = SomeList`1@16 [\n" +
                                    "      \"a\",\n" +
                                    "   ],\n" +
                                    "}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LongNestedString()
        {
            var actual = new ClassWithNesting
            {
                HashCode = 1,
                Nested = new string('x', 129)
            }.ToStringReflection();
            var expected = string.Format(
                "ClassWithNesting@1 {{\n" +
                "   HashCode = 1,\n" +
                "   Nested = \"{0}... (cut)\",\n" +
                "}}",
                new string('x', 128));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LongStringsAreShortened()
        {
            // added special start char to make sure substring starts at correct location
            var input = "x" + new string('*', 128);
            var actual = input.ToStringReflection();
            var expected = "x" + new string('*', 127) + "... (cut)";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AnonymousDelegatesAreNotPrinted()
        {
            var actual = new ClassWithAnonymousDelegate().ToStringReflection();
            var expected = "ClassWithAnonymousDelegate@1 {\n}";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ConstFieldsAreNotPrinted()
        {
            var actual = new ClassWithConstField().ToStringReflection();
            var expected = "ClassWithConstField@1 {\n   A = 1,\n}";
            Assert.AreEqual(expected, actual);
        }

        public class ClassWithDifferentVisibilities
        {
            // ReSharper disable once UnusedField.Compiler
            private int _privateField = 1;
            public int PublicField = 2;
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            private int PrivateProperty { get; set; }
            public int PublicProperty { get; set; }

            public ClassWithDifferentVisibilities()
            {
                PrivateProperty = 3;
                PublicProperty = 4;
            }

            public override int GetHashCode()
            {
                return 5;
            }
        }

        public class ClassWithDifferentKindsOfProperties
        {
            public int AutoProperty { get; set; }

            public int PropertyWithoutBacking
            {
                get { return AutoProperty; }
            }

            public ClassWithDifferentKindsOfProperties()
            {
                AutoProperty = 6;
            }

            public override int GetHashCode()
            {
                return 7;
            }
        }

        public class SuperClass
        {
            public int Num = 8;
        }

        public class ClassWithHierarchy : SuperClass
        {
            public override int GetHashCode()
            {
                return 9;
            }
        }

        public class ClassWitShadowedField : SuperClass
        {
            public new int Num = 10;

            public override int GetHashCode()
            {
                return 11;
            }
        }

        public class ClassWithToString
        {
            public int Num = 12;

            public override string ToString()
            {
                return "{\n   XYZ\n}";
            }
        }

        public class ClassWithToStringInHierarchy : ClassWithToString
        {
            public int Num2 = 13;
        }

        public class ClassWithThrowingToString
        {
            public override string ToString()
            {
                throw new InvalidOperationException("a cause");
            }

            public override int GetHashCode()
            {
                return 14;
            }
        }

        public struct SomeStruct
        {
            public int Num { get; set; }

            public override int GetHashCode()
            {
                return 15;
            }
        }

        public class SomeList<T> : List<T>
        {
            public new void Add(T item)
            {
                base.Add(item);
            }

            public override int GetHashCode()
            {
                return 16;
            }
        }

        public class ClassWithNesting
        {
            public int HashCode { get; set; }

            public object Nested { get; set; }

            protected bool Equals(ClassWithNesting other)
            {
                return HashCode == other.HashCode && Equals(Nested, other.Nested);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != GetType())
                {
                    return false;
                }
                return Equals((ClassWithNesting) obj);
            }

            public override int GetHashCode()
            {
                return HashCode;
            }
        }

        public class ClassWithAnonymousDelegate
        {
            public void M()
            {
                // ReSharper disable once UnusedVariable
                Action<int> d = i => { };
            }

            public override int GetHashCode()
            {
                return 1;
            }
        }

        public class ClassWithConstField
        {
            public int A = 1;
            public const int B = 2;

            public override int GetHashCode()
            {
                return 1;
            }
        }
    }
}