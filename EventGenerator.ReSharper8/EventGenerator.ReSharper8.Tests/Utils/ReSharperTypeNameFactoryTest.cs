using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using KaVE.EventGenerator.ReSharper8.Utils;
using NUnit.Framework;

namespace KaVE.EventGenerator.ReSharper8.Tests.Utils
{
    [TestFixture]
    public class ReSharperTypeNameFactoryTest
    {
        [TestCase("System.String", "mscore", "4.0.0.0", "System.String, mscore, Version=4.0.0.0"),
         TestCase("Some.Outer+Inner", "Assembly", "5.4.3.2", "Some.Outer+Inner, Assembly, Version=5.4.3.2"),
         TestCase("ValueType[,,]", "A", "9.8.7.6", "ValueType[,,], A, Version=9.8.7.6"),
         TestCase("ValueType[][]", "B", "5.4.3.2", "ValueType[][], B, Version=5.4.3.2")]
        public void ShouldGetNameForIType(string typeFqn, string assemblyName, string assemblyVersion, string identifier)
        {
            var type = ReSharperMockUtils.MockIType(typeFqn, assemblyName, assemblyVersion);

            AssertNameIdentifier(type, identifier);
        }

        public struct GenericTypeCase
        {
            private readonly string _fullName;
            private readonly IDictionary<string, IType> _typeParameters;
            private readonly string _assemblyName;
            private readonly string _assemblyVersion;
            private readonly string _expectedIdentifier;

            public GenericTypeCase(string fullName,
                IDictionary<string, IType> typeParameters,
                string assemblyName,
                string assemblyVersion,
                string identifier)
            {
                _fullName = fullName;
                _typeParameters = typeParameters;
                _assemblyName = assemblyName;
                _assemblyVersion = assemblyVersion;
                _expectedIdentifier = identifier;
            }

            public string FullName
            {
                get { return _fullName; }
            }

            public IEnumerable<KeyValuePair<string, IType>> TypeParameters
            {
                get { return _typeParameters.AsEnumerable(); }
            }

            public string AssemblyName
            {
                get { return _assemblyName; }
            }

            public string AssemblyVersion
            {
                get { return _assemblyVersion; }
            }

            public string ExpectedIdentifier
            {
                get { return _expectedIdentifier; }
            }
        }

        private static readonly GenericTypeCase[] GenericTypeCases =
        {
            new GenericTypeCase(
                "System.Nullable`1",
                new Dictionary<string, IType> {{"T", ReSharperMockUtils.MockIType("System.Int32", "mscore", "4.0.0.0")}},
                "mscore",
                "4.0.0.0",
                "System.Nullable`1[[T -> System.Int32, mscore, Version=4.0.0.0]], mscore, Version=4.0.0.0"),
            new GenericTypeCase("System.Collections.IDictionary`2",
                new Dictionary<string, IType>
                {
                    {"TKey", ReSharperMockUtils.MockIType("System.String", "mscore", "1.2.3.4")},
                    {"TValue", ReSharperMockUtils.MockIType("System.Object", "mscore", "5.6.7.8")}
                },
                "mscore",
                "9.10.11.12",
                "System.Collections.IDictionary`2[[TKey -> System.String, mscore, Version=1.2.3.4],[TValue -> System.Object, mscore, Version=5.6.7.8]], mscore, Version=9.10.11.12")
        };

        [TestCaseSource("GenericTypeCases")]
        public void ShouldGetNameForGenericIType(GenericTypeCase gtc)
        {
            var type = ReSharperMockUtils.MockIType(gtc.FullName, gtc.TypeParameters, gtc.AssemblyName, gtc.AssemblyVersion);

            AssertNameIdentifier(type, gtc.ExpectedIdentifier);
        }

        private static void AssertNameIdentifier(IType type, string identifier)
        {
            var name = type.GetName();
            Assert.AreEqual(identifier, name.Identifier);
        }
    }
}