using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json
{
    [TestFixture]
    public class NamesSerializationTest
    {
        [Test]
        public void ShouldSerializeName()
        {
            var name = Name.Get("Foobar! That's my Name.");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeAssemblyName()
        {
            var name = AssemblyName.Get("AssemblyName, Version=0.8.15.0");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeAssemblyVersion()
        {
            var name = AssemblyVersion.Get("1.3.3.7");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeFieldName()
        {
            var name = FieldName.Get("static [Declarator, B, Version=9.2.3.8] [Val, G, Version=5.4.6.3].Field");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeMethodName()
        {
            var name =
                MethodName.Get(
                    "[Declarator, B, Version=9.2.3.8] [Val, G, Version=5.4.6.3].Method(out [Param, P, Version=8.1.7.2])");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeNamespaceName()
        {
            var name = NamespaceName.Get("This.Is.My.Namespace");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeTypeName()
        {
            var name = TypeName.Get("Foo.Bar, foo, Version=1.0.0.0");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }
    }
}