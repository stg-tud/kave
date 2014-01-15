using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Model.Tests.Names.CSharp
{
    [TestFixture]
    class AssemblyVersionTest
    {
        [Test]
        public void ShouldEqualsSameVersion()
        {
            var v1 = AssemblyVersion.Get("1.2.3.4");
            var v2 = AssemblyVersion.Get("1.2.3.4");

            Assert.AreEqual(v1, v2);
            Assert.IsTrue(v1 == v2);
        }

        [Test]
        public void ShouldBeGreaterThenPreviousVersions()
        {
            var assemblyVersion = AssemblyVersion.Get("4.3.2.1");

            Assert.IsTrue(AssemblyVersion.Get("3.4.2.1") < assemblyVersion);
            Assert.IsTrue(AssemblyVersion.Get("4.2.99.10") < assemblyVersion);
            Assert.IsTrue(AssemblyVersion.Get("4.3.1.23") < assemblyVersion);
            Assert.IsTrue(AssemblyVersion.Get("4.3.2.0") < assemblyVersion);
        }

        [Test]
        public void ShouldBeSmallerThenLaterVersions()
        {
            var assemblyVersion = AssemblyVersion.Get("4.3.2.1");

            Assert.IsTrue(AssemblyVersion.Get("5.3.2.1") > assemblyVersion);
            Assert.IsTrue(AssemblyVersion.Get("4.13.0.0") > assemblyVersion);
            Assert.IsTrue(AssemblyVersion.Get("4.3.1337.69") > assemblyVersion);
            Assert.IsTrue(AssemblyVersion.Get("4.3.2.666") > assemblyVersion);
        }
    }
}
