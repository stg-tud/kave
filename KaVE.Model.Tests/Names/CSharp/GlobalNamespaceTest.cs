using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Model.Tests.Names.CSharp
{
    [TestFixture]
    public class GlobalNamespaceTest
    {
        private readonly INamespaceName _globalNamespace = NamespaceName.GlobalNamespace;

        [Test]
        public void ShouldHaveEmptyName()
        {
            Assert.IsEmpty(_globalNamespace.Name);
        }

        [Test]
        public void ShouldBeGlobalNamespace()
        {
            Assert.IsTrue(_globalNamespace.IsGlobalNamespace);
        }

        [Test]
        public void ShouldHaveEmptyIdentifier()
        {
            Assert.IsEmpty(_globalNamespace.Identifier);
        }

        [Test]
        public void ShouldHaveNoParentNamespace()
        {
            Assert.IsNull(_globalNamespace.ParentNamespace);
        }
    }
}