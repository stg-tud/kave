using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Model.Tests.Names.CSharp
{
    [TestFixture]
    class FieldNameTest
    {
        private const string Identifier = "[b.ValueType, B, 1.3.3.7] [a.ValueType, A, 4.2.2.3].fieldName";

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
            var fieldName = FieldName.Get("[" + valueTypeIdentifier + "] ["+ declaringTypeIdentifier + "].bar");

            Assert.AreEqual("bar", fieldName.Name);
            Assert.AreEqual(valueTypeIdentifier, fieldName.ValueType.Identifier);
            Assert.IsTrue(fieldName.ValueType.HasTypeParameters);
            Assert.AreEqual(declaringTypeIdentifier, fieldName.DeclaringType.Identifier);
            Assert.IsTrue(fieldName.DeclaringType.HasTypeParameters);
        }
    }
}
