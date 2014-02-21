using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Model.Tests.Events.CompletionEvent
{
    [TestFixture]
    internal class TypeHierarchyTest
    {
        private TypeHierarchy _uut;

        [SetUp]
        public void SetUpHierarchyUnderTest()
        {
            _uut = new TypeHierarchy("MyType, MyAssembly")
            {
                Extends = new TypeHierarchy("System.Object, mscorlib, Version=4.0.0.0")
            };
            _uut.Implements.Add(new TypeHierarchy("ISomeinterface, MyAssembly"));
        }

        [Test]
        public void ShouldEqualACloneOfItself()
        {
            var clone = new TypeHierarchy("MyType, MyAssembly")
            {
                Extends = new TypeHierarchy("System.Object, mscorlib, Version=4.0.0.0")
            };
            _uut.Implements.Add(new TypeHierarchy("ISomeinterface, MyAssembly"));

            Assert.AreEqual(_uut, clone);
            Assert.AreEqual(_uut.GetHashCode(), clone.GetHashCode());
        }

        [Test]
        public void ShouldNotEqualOtherType()
        {
            var other = new TypeHierarchy("OtherType, MyAssembly")
            {
                Extends = new TypeHierarchy("System.Object, mscorlib, Version=4.0.0.0")
            };

            Assert.AreNotEqual(_uut, other);
            Assert.AreNotEqual(_uut.GetHashCode(), other.GetHashCode());
        }
    }
}