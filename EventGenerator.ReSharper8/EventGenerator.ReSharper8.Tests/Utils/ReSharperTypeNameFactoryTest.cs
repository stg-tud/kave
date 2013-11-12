using JetBrains.ReSharper.Psi;
using KaVE.EventGenerator.ReSharper8.Utils;
using NUnit.Framework;

namespace KaVE.EventGenerator.ReSharper8.Tests.Utils
{
    [TestFixture]
    public class ReSharperTypeNameFactoryTest
    {
        [TestCase("System.String", "mscore", "4.0.0.0", "System.String, mscore, Version=4.0.0.0")]
        [TestCase("System.Nullable`1[[System.Int32]]", "mscore", "4.0.0.0", "System.Nullable`1[[System.Int32]], mscore, Version=4.0.0.0")]
        [TestCase("Some.Outer+Inner", "Assembly", "5.4.3.2", "Some.Outer+Inner, Assembly, Version=5.4.3.2")]
        public void ShouldGetNameForIType(string typeFqn, string assemblyName, string assemblyVersion, string identifier)
        {
            // TODO test how generic and array types really look like!
            var type = ReSharperMockUtils.MockIType(typeFqn, assemblyName, assemblyVersion);

            AssertNameIdentifier(type, identifier);
        }

        private static void AssertNameIdentifier(IType type, string identifier)
        {
            var name = type.GetName();
            Assert.AreEqual(identifier, name.Identifier);
        }
        
    }
}