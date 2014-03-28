using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Model.Tests.Names.CSharp
{
    [TestFixture]
    class AssemblyNameTest
    {
        [Test]
        public void ShouldBeMSCorLibAssembly()
        {
            const string identifier = "mscorlib, 4.0.0.0";
            var mscoreAssembly = AssemblyName.Get(identifier);

            Assert.AreEqual("mscorlib", mscoreAssembly.Name);
            Assert.AreEqual("4.0.0.0", mscoreAssembly.AssemblyVersion.Identifier);
            Assert.AreEqual(identifier, mscoreAssembly.Identifier);
        }

        [Test]
        public void ShouldBeVersionlessAssembly()
        {
            const string identifier = "assembly";
            var assemblyName = AssemblyName.Get(identifier);

            Assert.AreEqual("assembly", assemblyName.Name);
            Assert.IsNull(assemblyName.AssemblyVersion);
            Assert.AreEqual(identifier, assemblyName.Identifier);
        }
    }
}
