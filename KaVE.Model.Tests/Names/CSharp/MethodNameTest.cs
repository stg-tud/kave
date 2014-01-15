using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Model.Tests.Names.CSharp
{
    [TestFixture]
    class MethodNameTest
    {
        [Test]
        public void ShouldBeSimpleMethod()
        {
            var methodName = MethodName.Get("[System.Void, mscore, Version=4.0.0.0] [T, P, Version=1.2.3.4].MethodName()");

            Assert.AreEqual("T, P, Version=1.2.3.4", methodName.DeclaringType.Identifier);
            Assert.AreEqual("System.Void, mscore, Version=4.0.0.0", methodName.ReturnType.Identifier);
            Assert.AreEqual("MethodName", methodName.Name);
            Assert.IsFalse(methodName.HasTypeParameters);
            Assert.IsEmpty(methodName.TypeParameters);
            Assert.IsFalse(methodName.HasParameters);
            Assert.IsEmpty(methodName.Parameters);
            Assert.IsFalse(methodName.IsConstructor);
            Assert.IsFalse(methodName.IsStatic);
        }

        [Test]
        public void ShouldBeMethodWithOneParameter()
        {
            const string declaringTypeIdentifier = "A, B, Version=9.9.9.9";
            const string returnTypeIdentifier = "R, C, Version=7.6.5.4";
            const string parameterIdentifier = "[P, D, Version=3.4.3.2] n";

            var methodName = MethodName.Get("[" + returnTypeIdentifier + "] [" + declaringTypeIdentifier + "].M(" + parameterIdentifier + ")");

            Assert.AreEqual(declaringTypeIdentifier, methodName.DeclaringType.Identifier);
            Assert.AreEqual(returnTypeIdentifier, methodName.ReturnType.Identifier);
            Assert.AreEqual("M", methodName.Name);
            Assert.IsFalse(methodName.HasTypeParameters);
            Assert.AreEqual(1, methodName.Parameters.Count);
            Assert.AreEqual(parameterIdentifier, methodName.Parameters[0].Identifier);
        }

        [Test]
        public void ShouldBeMethodWithMultipleParameters()
        {
            const string declaringTypeIdentifier = "A, B, Version=9.9.9.9";
            const string returnTypeIdentifier = "R, C, Version=7.6.5.4";
            const string param1Identifier = "[P, D, Version=3.4.3.2] n";
            const string param2Identifier = "[Q, E, Version=9.1.8.2] o";
            const string param3Identifier = "[R, F, Version=6.5.7.4] p";

            var methodName = MethodName.Get("[" + returnTypeIdentifier + "] [" + declaringTypeIdentifier + "].DoIt(" + param1Identifier + ", " + param2Identifier + ", " + param3Identifier + ")");

            Assert.AreEqual(declaringTypeIdentifier, methodName.DeclaringType.Identifier);
            Assert.AreEqual(returnTypeIdentifier, methodName.ReturnType.Identifier);
            Assert.AreEqual("DoIt", methodName.Name);
            Assert.IsFalse(methodName.HasTypeParameters);
            Assert.AreEqual(3, methodName.Parameters.Count);
            Assert.AreEqual(param1Identifier, methodName.Parameters[0].Identifier);
            Assert.AreEqual(param2Identifier, methodName.Parameters[1].Identifier);
            Assert.AreEqual(param3Identifier, methodName.Parameters[2].Identifier);
        }

        [Test]
        public void ShouldBeConstructor()
        {
            var methodName = MethodName.Get("[MyType, A, Version=0.0.0.1] [MyType, A, Version=0.0.0.1]..ctor()");

            Assert.IsTrue(methodName.IsConstructor);
        }

        [Test]
        public void ShouldBeStaticMethod()
        {
            var methodName = MethodName.Get("static [I, A, Version=1.0.2.0] [K, K, Version=0.1.0.2].m()");

            Assert.IsTrue(methodName.IsStatic);
        }
    }
}
