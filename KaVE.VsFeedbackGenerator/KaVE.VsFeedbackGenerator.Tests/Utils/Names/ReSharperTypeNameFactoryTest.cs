using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using KaVE.VsFeedbackGenerator.Tests.TestFactories;
using KaVE.VsFeedbackGenerator.Utils.Names;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Names
{
    [TestFixture]
    public class ReSharperTypeNameFactoryTest
    {
        [TestCase("System.String", "mscore", "4.0.0.0", "System.String, mscore, 4.0.0.0"),
         TestCase("Some.Outer+Inner", "Assembly", "5.4.3.2", "Some.Outer+Inner, Assembly, 5.4.3.2"),
         TestCase("ValueType[,,]", "A", "9.8.7.6", "ValueType[,,], A, 9.8.7.6"),
         TestCase("ValueType[][]", "B", "5.4.3.2", "ValueType[][], B, 5.4.3.2")]
        public void ShouldGetNameForIType(string typeFqn, string assemblyName, string assemblyVersion, string identifier)
        {
            var type = TypeMockUtils.MockIType(typeFqn, assemblyName, assemblyVersion);

            AssertNameIdentifier(type, identifier);
        }

        [Test]
        public void ShouldGetNameForITypeWithOneTypeParameter()
        {
            var type = TypeMockUtils.MockIType(
                "System.Nullable`1",
                new Dictionary<string, IType>
                {
                    {"T", TypeMockUtils.MockIType("System.Int32", "mscore", "4.0.0.0")}
                },
                "mscore",
                "4.0.0.0");

            AssertNameIdentifier(type, "System.Nullable`1[[T -> System.Int32, mscore, 4.0.0.0]], mscore, 4.0.0.0");
        }

        [Test]
        public void ShouldGetNameForITypeWithTwoTypeParameters()
        {
            var type = TypeMockUtils.MockIType(
                "System.Collections.IDictionary`2",
                new Dictionary<string, IType>
                {
                    {"TKey", TypeMockUtils.MockIType("System.String", "mscore", "1.2.3.4")},
                    {"TValue", TypeMockUtils.MockIType("System.Object", "mscore", "5.6.7.8")}
                },
                "mscore",
                "9.10.11.12");

            AssertNameIdentifier(
                type,
                "System.Collections.IDictionary`2[[TKey -> System.String, mscore, 1.2.3.4],[TValue -> System.Object, mscore, 5.6.7.8]], mscore, 9.10.11.12");
        }

        private static void AssertNameIdentifier(IType type, string identifier)
        {
            var name = type.GetName();
            Assert.AreEqual(identifier, name.Identifier);
        }
    }
}