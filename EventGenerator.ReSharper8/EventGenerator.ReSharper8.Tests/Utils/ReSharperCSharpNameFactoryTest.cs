using System;
using System.Collections.Generic;
using JetBrains.Metadata.Utils;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.Modules;
using KaVE.EventGenerator.ReSharper8.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.EventGenerator.ReSharper8.Tests.Utils
{
    [TestFixture]
    public class ReSharperCSharpNameFactoryTest
    {
        [Test]
        public void ShouldGetNameForINamespace()
        {
            var nsMock = new Mock<INamespace>();
            nsMock.Setup(ns => ns.QualifiedName).Returns("My.Test.Namespace");

            AssertIdentifier(nsMock.Object, "My.Test.Namespace");
        }

        [Test]
        public void ShouldGetNameForITypeElement()
        {
            var typeElement = MockTypeElement("Full.Qualified.TypeName", "AssemblyName");

            AssertIdentifier(typeElement, "Full.Qualified.TypeName, AssemblyName");
        }

        [TestCase("param0", "ParameterType", "AnAssembly", "1.2.3.4", ParameterKind.VALUE, false, false, "[ParameterType, AnAssembly, Version=1.2.3.4] param0"),
         TestCase("length", "int", "mscore", "4.0.0.0", ParameterKind.VALUE, false, false, "[System.Int32, mscore, Version=4.0.0.0] length"),
         TestCase("str", "string", "mscore", "4.0.0.0", ParameterKind.VALUE, false, false, "[System.String, mscore, Version=4.0.0.0] str"),
         TestCase("optional", "bool", "mscore", "4.0.0.0", ParameterKind.VALUE, true, false, "opt [System.Boolean, mscore, Version=4.0.0.0] optional"),
         TestCase("output", "Type", "Assembly", "0.1.9.2", ParameterKind.OUTPUT, false, false, "out [Type, Assembly, Version=0.1.9.2] output"),
         TestCase("reference", "long", "mscore", "4.0.0.0", ParameterKind.REFERENCE, false, false, "ref [System.Int64, mscore, Version=4.0.0.0] reference")]
        public void ShouldGetNameForIParameter(string parameterName,
            string typeName,
            string assemblyName,
            string assemblyVersion,
            ParameterKind pKind,
            bool isOptional,
            bool isParameterArray,
            string identifier)
        {
            var paramMock = new Mock<IParameter>();
            paramMock.Setup(p => p.ShortName).Returns(parameterName);
            paramMock.Setup(p => p.Type).Returns(MockIType(typeName, assemblyName, assemblyVersion));
            paramMock.Setup(p => p.Kind).Returns(pKind);
            paramMock.Setup(p => p.IsOptional).Returns(isOptional);
            paramMock.Setup(p => p.IsParameterArray).Returns(isParameterArray);

            AssertIdentifier(paramMock.Object, identifier);
        }

        [Test]
        public void ShouldGetNameForIField()
        {
            var fieldMock = new Mock<IField>();
            fieldMock.Setup(f => f.GetContainingType()).Returns(MockTypeElement("DeclaringType", "DTA"));
            fieldMock.Setup(f => f.Type).Returns(MockIType("ValueType", "VTA", "1.2.3.4"));
            fieldMock.Setup(f => f.ShortName).Returns("FieldName");

            AssertIdentifier(fieldMock.Object, "[ValueType, VTA, Version=1.2.3.4] [DeclaringType, DTA].FieldName");
        }

        [Test]
        public void ShouldGetNameForStaticIField()
        {
            var fieldMock = new Mock<IField>();
            fieldMock.Setup(f => f.GetContainingType()).Returns(MockTypeElement("DeclaringType", "DTA"));
            fieldMock.Setup(f => f.Type).Returns(MockIType("ValueType", "VTA", "1.2.3.4"));
            fieldMock.Setup(f => f.ShortName).Returns("FieldName");
            fieldMock.Setup(f => f.IsStatic).Returns(true);

            AssertIdentifier(fieldMock.Object, "static [ValueType, VTA, Version=1.2.3.4] [DeclaringType, DTA].FieldName");
        }

        [Test]
        public void ShouldGetNameForIFunction()
        {
            var functionMock = new Mock<IFunction>();
            functionMock.Setup(f => f.GetContainingType()).Returns(MockTypeElement("DeclaringType", "DTA"));
            functionMock.Setup(f => f.ReturnType).Returns(MockIType("ReturnType", "RTA", "1.2.3.4"));
            functionMock.Setup(f => f.ShortName).Returns("MethodName");
            functionMock.Setup(f => f.Parameters).Returns(new List<IParameter>());

            AssertIdentifier(functionMock.Object, "[ReturnType, RTA, Version=1.2.3.4] [DeclaringType, DTA].MethodName()");
        }

        [Test]
        public void ShouldGetNameForStaticIFunction()
        {
            var functionMock = new Mock<IFunction>();
            functionMock.Setup(f => f.GetContainingType()).Returns(MockTypeElement("DeclaringType", "DTA"));
            functionMock.Setup(f => f.ReturnType).Returns(MockIType("ReturnType", "RTA", "1.2.3.4"));
            functionMock.Setup(f => f.ShortName).Returns("MethodName");
            functionMock.Setup(f => f.Parameters).Returns(new List<IParameter>());
            functionMock.Setup(f => f.IsStatic).Returns(true);

            AssertIdentifier(functionMock.Object, "static [ReturnType, RTA, Version=1.2.3.4] [DeclaringType, DTA].MethodName()");
        }

        [Test]
        public void ShouldGetNameForVariableDeclaration()
        {
            var mockVariable = new Mock<ITypeOwner>();
            mockVariable.Setup(v => v.ShortName).Returns("variable");
            mockVariable.Setup(v => v.Type).Returns(MockIType("Type", "Assembly", "1.2.3.4"));

            AssertIdentifier(mockVariable.Object, "[Type, Assembly, Version=1.2.3.4] variable");
        }

        [Test]
        public void ShouldGetNameForAlias()
        {
            var aliasMock = new Mock<IAlias>();
            aliasMock.Setup(a => a.ShortName).Returns("global");

            AssertIdentifier(aliasMock.Object, "global");
        }

        [TestCase("System.String", "mscore", "4.0.0.0", "System.String, mscore, Version=4.0.0.0")]
        [TestCase("System.Nullable`1[[System.Int32]]", "mscore", "4.0.0.0", "System.Nullable`1[[System.Int32]], mscore, Version=4.0.0.0")]
        [TestCase("Some.Outer+Inner", "Assembly", "5.4.3.2", "Some.Outer+Inner, Assembly, Version=5.4.3.2")]
        public void ShouldGetNameForIType(string typeFqn, string assemblyName, string assemblyVersion, string identifier)
        {
            // TODO test how generic and array types really look like!
            var type = MockIType(typeFqn, assemblyName, assemblyVersion);

            AssertIdentifier(type, identifier);
        }

        private static IType MockIType(string fqnOrAlias, string assemblyName, string assemblyVersion)
        {
            var typeMock = new Mock<IDeclaredType>();
            typeMock.Setup(t => t.GetLongPresentableName(CSharpLanguage.Instance)).Returns(fqnOrAlias);
            typeMock.Setup(t => t.Assembly).Returns(MockAssembly(assemblyName, assemblyVersion));
            return typeMock.Object;
        }

        private static ITypeElement MockTypeElement(string fqn, string assemblyName)
        {
            var clrMock = new Mock<IClrTypeName>();
            clrMock.Setup(clr => clr.FullName).Returns(fqn);

            var teMock = new Mock<ITypeElement>();
            teMock.Setup(te => te.GetClrName()).Returns(clrMock.Object);
            teMock.Setup(te => te.Module).Returns(MockModule(assemblyName));

            return teMock.Object;
        }

        private static AssemblyNameInfo MockAssembly(string assemblyName, string version)
        {
            var assembly = new System.Reflection.AssemblyName {Name = assemblyName, Version = new Version(version)};
            return new AssemblyNameInfo(assembly);
        }

        private static IPsiModule MockModule(string name)
        {
            var moduleMock = new Mock<IPsiModule>();
            moduleMock.Setup(m => m.Name).Returns(name);
            return moduleMock.Object;
        }

        private static void AssertIdentifier<TDeclaredElement>(TDeclaredElement declaredElement,
            string identifier) where TDeclaredElement : class, IDeclaredElement
        {
            var name = declaredElement.GetName();
            Assert.AreEqual(identifier, name.Identifier);
        }

        private static void AssertIdentifier(IType type, string identifier)
        {
            var name = type.GetName();
            Assert.AreEqual(identifier, name.Identifier);
        }
    }
}