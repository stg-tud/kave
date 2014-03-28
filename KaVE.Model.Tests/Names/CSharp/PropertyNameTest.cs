using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Model.Tests.Names.CSharp
{
    [TestFixture, TestFixture]
    class PropertyNameTest
    {
        [Test]
        public void ShouldBeSimpleProperty()
        {
            const string valueTypeIdentifier = "A, B, 1.0.0.0";
            const string declaringTypeIdentifier = "C, D, 0.9.8.7";
            var propertyName = PropertyName.Get("[" + valueTypeIdentifier + "] [" + declaringTypeIdentifier + "].Property");

            Assert.AreEqual("Property", propertyName.Name);
            Assert.AreEqual(valueTypeIdentifier, propertyName.ValueType.Identifier);
            Assert.AreEqual(declaringTypeIdentifier, propertyName.DeclaringType.Identifier);
            Assert.IsFalse(propertyName.IsStatic);
        }

        [Test]
        public void ShoudBePropertyWithGetter()
        {
            var propertyName = PropertyName.Get("get [Z, Y, 0.5.6.1] [X, W, 0.3.4.2].Prop");

            Assert.IsTrue(propertyName.HasGetter);
        }

        [Test]
        public void ShoudBePropertyWithSetter()
        {
            var propertyName = PropertyName.Get("set [Z, Y, 0.5.6.1] [X, W, 0.3.4.2].Prop");

            Assert.IsTrue(propertyName.HasSetter);
        }

        [Test]
        public void ShouldBeStaticProperty()
        {
            var propertyName = PropertyName.Get("static [A, B, 1.2.3.4] [C, D, 5.6.7.8].E");

            Assert.IsTrue(propertyName.IsStatic);
        }
    }
}
