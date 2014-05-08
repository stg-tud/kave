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
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Model.Tests.Names.CSharp
{
    [TestFixture]
    class CSharpNameUtilsTest
    {
        [Test] 
        public void ShouldBeVoidType()
        {
            var voidTypeName = CSharpNameUtils.GetFullTypeNameFromTypeAlias("void");

            Assert.AreEqual("System.Void", voidTypeName);
        }

        [Test]
        public void ShouldBeValueTypeInt()
        {
            var intTypeName = CSharpNameUtils.GetFullTypeNameFromTypeAlias("int");

            Assert.AreEqual("System.Int32", intTypeName);
        }

        [Test]
        public void ShouldBeNullableType()
        {
            var nullableTypeName = CSharpNameUtils.GetFullTypeNameFromTypeAlias("int?");

            Assert.AreEqual("System.Nullable`1[[System.Int32]]", nullableTypeName);
        }

        [Test]
        public void ObjectShouldBeClassType()
        {
            var objectTypeName = CSharpNameUtils.GetFullTypeNameFromTypeAlias("object");

            Assert.AreEqual("System.Object", objectTypeName);
        }

        [Test]
        public void ShouldReplaceAliasesInArrayTypes()
        {
            var arrayTypeName = CSharpNameUtils.GetFullTypeNameFromTypeAlias("long[]");

            Assert.AreEqual("System.Int64[]", arrayTypeName);
        }
    }
}
