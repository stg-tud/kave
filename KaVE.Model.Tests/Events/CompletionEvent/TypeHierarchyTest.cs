using System.Collections.Generic;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Model.Tests.Events.CompletionEvent
{
    [TestFixture]
    class TypeHierarchyTest
    {
        private TypeHierarchy _uut;

        [SetUp]
        public void SetUpHierarchyUnderTest()
        {
            _uut = new TypeHierarchy
            {
                Element = TypeName.Get("MyType, MyAssembly"),
                Extends = new TypeHierarchy
                {
                    Element = TypeName.Get("System.Object, mscorlib, Version=4.0.0.0")
                },
                Implements = new HashSet<ITypeHierarchy>
                {
                    new TypeHierarchy
                    {
                        Element = TypeName.Get("ISomeinterface, MyAssembly")
                    }
                }
            };
        }

        [Test]
        public void ShouldEqualACloneOfItself()
        {
            var clone = new TypeHierarchy
            {
                Element = TypeName.Get("MyType, MyAssembly"),
                Extends = new TypeHierarchy
                {
                    Element = TypeName.Get("System.Object, mscorlib, Version=4.0.0.0")
                },
                Implements = new HashSet<ITypeHierarchy>
                {
                    new TypeHierarchy
                    {
                        Element = TypeName.Get("ISomeinterface, MyAssembly")
                    }
                }
            };

            Assert.AreEqual(_uut, clone);
            Assert.AreEqual(_uut.GetHashCode(), clone.GetHashCode());
        }

        [Test]
        public void ShouldNotEqualOtherType()
        {
            var other = new TypeHierarchy
            {
                Element = TypeName.Get("OtherType, MyAssembly"),
                Extends = new TypeHierarchy
                {
                    Element = TypeName.Get("System.Object, mscorlib, Version=4.0.0.0")
                }
            };

            Assert.AreNotEqual(_uut, other);
            Assert.AreNotEqual(_uut.GetHashCode(), other.GetHashCode());
        }
    }
}
