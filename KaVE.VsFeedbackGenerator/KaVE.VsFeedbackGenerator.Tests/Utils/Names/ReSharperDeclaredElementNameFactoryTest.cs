using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Resolve;
using KaVE.VsFeedbackGenerator.Tests.TestFactories;
using KaVE.VsFeedbackGenerator.Utils.Names;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Names
{
    [TestFixture]
    public class ReSharperDeclaredElementNameFactoryTest
    {
        [Test]
        public void ShouldReturnUnknownElementNameForUnknownElement()
        {
            var unknownElement = new Mock<IDeclaredElement>();
            unknownElement.Setup(e => e.ShortName).Returns(SharedImplUtil.MISSING_DECLARATION_NAME);

            AssertNameIdentifier(unknownElement.Object, SharedImplUtil.MISSING_DECLARATION_NAME);
        }

        [Test]
        public void ShouldGetNameForINamespace()
        {
            var nsMock = new Mock<INamespace>();
            nsMock.Setup(ns => ns.QualifiedName).Returns("My.Test.Namespace");

            AssertNameIdentifier(nsMock.Object, "My.Test.Namespace");
        }

        [Test]
        public void ShouldGetNameForTypeElementInAssembly()
        {
            var typeElement = TypeMockUtils.MockTypeElement("Full.Qualified.TypeName", TypeMockUtils.MockAssembly("AssemblyName", "0.9.8.7"));

            AssertNameIdentifier(typeElement, "Full.Qualified.TypeName, AssemblyName, 0.9.8.7");
        }

        [Test]
        public void SouldGetQualifiedNameForTypeElementInProject()
        {
            var typeElement = TypeMockUtils.MockTypeElement("TypeName", TypeMockUtils.MockProject("Project", "1.0.0.0"));

            AssertNameIdentifier(typeElement, "TypeName, Project, 1.0.0.0");
        }

        [Test]
        public void SouldGetQualifiedNameForTypeElementInUncompilableProject()
        {
            var typeElement = TypeMockUtils.MockTypeElement("TypeName", TypeMockUtils.MockUncompilableProject("Project"));

            AssertNameIdentifier(typeElement, "TypeName, Project");
        }

        [TestCase("param0", "ParameterType", "AnAssembly", "1.2.3.4", ParameterKind.VALUE, false, false, "[ParameterType, AnAssembly, 1.2.3.4] param0"),
         TestCase("length", "int", "mscore", "4.0.0.0", ParameterKind.VALUE, false, false, "[System.Int32, mscore, 4.0.0.0] length"),
         TestCase("str", "string", "mscore", "4.0.0.0", ParameterKind.VALUE, false, false, "[System.String, mscore, 4.0.0.0] str"),
         TestCase("optional", "bool", "mscore", "4.0.0.0", ParameterKind.VALUE, true, false, "opt [System.Boolean, mscore, 4.0.0.0] optional"),
         TestCase("output", "Type", "Assembly", "0.1.9.2", ParameterKind.OUTPUT, false, false, "out [Type, Assembly, 0.1.9.2] output"),
         TestCase("reference", "long", "mscore", "4.0.0.0", ParameterKind.REFERENCE, false, false, "ref [System.Int64, mscore, 4.0.0.0] reference")]
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
            paramMock.Setup(p => p.Type).Returns(TypeMockUtils.MockIType(typeName, assemblyName, assemblyVersion));
            paramMock.Setup(p => p.Kind).Returns(pKind);
            paramMock.Setup(p => p.IsOptional).Returns(isOptional);
            paramMock.Setup(p => p.IsParameterArray).Returns(isParameterArray);

            AssertNameIdentifier(paramMock.Object, identifier);
        }

        [Test]
        public void ShouldGetNameForIField()
        {
            var fieldMock = new Mock<IField>();
            fieldMock.Setup(f => f.GetContainingType()).Returns(TypeMockUtils.MockTypeElement("DeclaringType", "DTA", "0.9.8.7"));
            fieldMock.Setup(f => f.Type).Returns(TypeMockUtils.MockIType("ValueType", "VTA", "1.2.3.4"));
            fieldMock.Setup(f => f.ShortName).Returns("FieldName");

            AssertNameIdentifier(fieldMock.Object, "[ValueType, VTA, 1.2.3.4] [DeclaringType, DTA, 0.9.8.7].FieldName");
        }

        [Test]
        public void ShouldGetNameForStaticIField()
        {
            var fieldMock = new Mock<IField>();
            fieldMock.Setup(f => f.GetContainingType()).Returns(TypeMockUtils.MockTypeElement("DeclaringType", "DTA", "0.9.8.7"));
            fieldMock.Setup(f => f.Type).Returns(TypeMockUtils.MockIType("ValueType", "VTA", "1.2.3.4"));
            fieldMock.Setup(f => f.ShortName).Returns("FieldName");
            fieldMock.Setup(f => f.IsStatic).Returns(true);

            AssertNameIdentifier(fieldMock.Object, "static [ValueType, VTA, 1.2.3.4] [DeclaringType, DTA, 0.9.8.7].FieldName");
        }

        [Test]
        public void ShouldGetNameForIFunction()
        {
            var functionMock = new Mock<IFunction>();
            functionMock.Setup(f => f.GetContainingType()).Returns(TypeMockUtils.MockTypeElement("DeclaringType", "DTA", "0.9.8.7"));
            functionMock.Setup(f => f.ReturnType).Returns(TypeMockUtils.MockIType("ReturnType", "RTA", "1.2.3.4"));
            functionMock.Setup(f => f.ShortName).Returns("MethodName");
            functionMock.Setup(f => f.Parameters).Returns(new List<IParameter>());

            AssertNameIdentifier(functionMock.Object, "[ReturnType, RTA, 1.2.3.4] [DeclaringType, DTA, 0.9.8.7].MethodName()");
        }

        [Test]
        public void ShouldGetNameForStaticIFunction()
        {
            var functionMock = new Mock<IFunction>();
            functionMock.Setup(f => f.GetContainingType()).Returns(TypeMockUtils.MockTypeElement("DeclaringType", "DTA", "0.9.8.7"));
            functionMock.Setup(f => f.ReturnType).Returns(TypeMockUtils.MockIType("ReturnType", "RTA", "1.2.3.4"));
            functionMock.Setup(f => f.ShortName).Returns("MethodName");
            functionMock.Setup(f => f.Parameters).Returns(new List<IParameter>());
            functionMock.Setup(f => f.IsStatic).Returns(true);

            AssertNameIdentifier(functionMock.Object, "static [ReturnType, RTA, 1.2.3.4] [DeclaringType, DTA, 0.9.8.7].MethodName()");
        }

        [Test]
        public void ShouldGetNameForVariableDeclaration()
        {
            var mockVariable = new Mock<ITypeOwner>();
            mockVariable.Setup(v => v.ShortName).Returns("variable");
            mockVariable.Setup(v => v.Type).Returns(TypeMockUtils.MockIType("Type", "Assembly", "1.2.3.4"));

            AssertNameIdentifier(mockVariable.Object, "[Type, Assembly, 1.2.3.4] variable");
        }

        [Test]
        public void ShouldGetNameForAlias()
        {
            var aliasMock = new Mock<IAlias>();
            aliasMock.Setup(a => a.ShortName).Returns("global");

            AssertNameIdentifier(aliasMock.Object, "global");
        }

        private static void AssertNameIdentifier<TDeclaredElement>(TDeclaredElement declaredElement,
            string identifier) where TDeclaredElement : class, IDeclaredElement
        {
            var name = declaredElement.GetName(EmptySubstitution.INSTANCE);
            Assert.AreEqual(identifier, name.Identifier);
        }
    }
}