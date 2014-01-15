using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Model.Tests.Names.CSharp
{
    [TestFixture]
    public class NamespaceNameTest
    {
        private const string TestNamespaceIdentifier = "foo.bar";

        private INamespaceName _testNamespaceName;
        private INamespaceName _testNamespaceParentName;

        [TestFixtureSetUp]
        public void SetUpTestNamespace()
        {
            _testNamespaceName = NamespaceName.Get(TestNamespaceIdentifier);
            _testNamespaceParentName = _testNamespaceName.ParentNamespace;
        }
        
        [Test]
        public void ShouldHaveLastIdentifierSegmentAsName()
        {
            Assert.AreEqual("bar", _testNamespaceName.Name);
        }

        [Test]
        public void ShouldNotBeGlobalNamespace()
        {
            Assert.IsFalse(_testNamespaceName.IsGlobalNamespace);
        }

        [Test]
        public void ShouldHaveFullqualifiedIdentifier()
        {
            Assert.AreEqual(TestNamespaceIdentifier, _testNamespaceName.Identifier);
        }

        [Test]
        public void ShouldHaveParentNamespaceName()
        {
            Assert.IsNotNull(_testNamespaceParentName);
        }

        [Test]
        public void ShouldHaveParentNamespaceFoo()
        {
            Assert.AreEqual("foo", _testNamespaceParentName.Name);
            Assert.AreEqual("foo", _testNamespaceParentName.Identifier);
            Assert.IsFalse(_testNamespaceParentName.IsGlobalNamespace);
        }

        [Test]
        public void ShouldHaveGlobalNamespaceAsGrandParent()
        {
            Assert.AreEqual(NamespaceName.GlobalNamespace, _testNamespaceParentName.ParentNamespace);
        }
    }
}
