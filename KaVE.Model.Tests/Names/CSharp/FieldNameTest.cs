using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Model.Tests.Names.CSharp
{
    [TestFixture]
    class FieldNameTest
    {
        private const string Identifier = "[b.ValueType, B, Version=1.3.3.7] [a.ValueType, A, Version=4.2.2.3].fieldName";

        [Test]
        public void ShouldBeInstanceFieldName()
        {
            var fieldName = FieldName.Get(Identifier);

            Assert.AreEqual(Identifier, fieldName.Identifier);
            Assert.AreEqual("fieldName", fieldName.Name);
            Assert.AreEqual("a.ValueType, A, Version=4.2.2.3", fieldName.DeclaringType.Identifier);
            Assert.AreEqual("b.ValueType, B, Version=1.3.3.7", fieldName.ValueType.Identifier);
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
            const string valueTypeIdentifier = "T`1[[A, B, Version=1.0.0.0]], A, Version=9.1.8.2";
            const string declaringTypeIdentifier = "U`2[[B, C, Version=6.7.5.8],[C, D, Version=8.3.7.4]], Z, Version=0.0.0.0";
            var fieldName = FieldName.Get("[" + valueTypeIdentifier + "] ["+ declaringTypeIdentifier + "].bar");

            Assert.AreEqual("bar", fieldName.Name);
            Assert.AreEqual(valueTypeIdentifier, fieldName.ValueType.Identifier);
            Assert.IsTrue(fieldName.ValueType.HasTypeParameters);
            Assert.AreEqual(declaringTypeIdentifier, fieldName.DeclaringType.Identifier);
            Assert.IsTrue(fieldName.DeclaringType.HasTypeParameters);
        }
    }
}
