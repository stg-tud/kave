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
