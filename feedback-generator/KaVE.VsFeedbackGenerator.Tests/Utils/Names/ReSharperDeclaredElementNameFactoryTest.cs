/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Contributors:
 *    - Sven Amann
 */

using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Impl.Resolve;
using JetBrains.ReSharper.Psi.Resolve;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.Names.CSharp.MemberNames;
using KaVE.Model.Names.CSharp.Modularization;
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
            AssertUnknownName<IDeclaredElement>(Name.UnknownName);
        }

        [Test]
        public void ShouldGetNameForINamespace()
        {
            var nsMock = new Mock<INamespace>();
            nsMock.Setup(ns => ns.QualifiedName).Returns("My.Test.Namespace");

            AssertName(nsMock.Object, NamespaceName.Get("My.Test.Namespace"));
        }

        [Test]
        public void ShouldGetUnknownNamespaceName()
        {
            AssertUnknownName<INamespace>(NamespaceName.UnknownName);
        }

        [Test]
        public void ShouldGetNameForTypeElementInAssembly()
        {
            var typeElement = TypeMockUtils.MockTypeElement(
                "Full.Qualified.TypeName",
                TypeMockUtils.MockAssembly("AssemblyName", "0.9.8.7"));

            AssertName(typeElement, TypeName.Get("Full.Qualified.TypeName, AssemblyName, 0.9.8.7"));
        }

        [Test]
        public void SouldGetQualifiedNameForTypeElementInProject()
        {
            var typeElement = TypeMockUtils.MockTypeElement("TypeName", TypeMockUtils.MockProject("Project", "1.0.0.0"));

            AssertName(typeElement, TypeName.Get("TypeName, Project, 1.0.0.0"));
        }

        [Test]
        public void SouldGetQualifiedNameForTypeElementInUncompilableProject()
        {
            var typeElement = TypeMockUtils.MockTypeElement(
                "TypeName",
                TypeMockUtils.MockUncompilableProject("Project"));

            AssertName(typeElement, TypeName.Get("TypeName, Project"));
        }

        [Test]
        public void ShouldGetUnknownTypeName()
        {
            AssertUnknownName<ITypeElement>(TypeName.UnknownName);
        }

        [TestCase("param0", "ParameterType", "AnAssembly", "1.2.3.4", ParameterKind.VALUE, false, false,
            "[ParameterType, AnAssembly, 1.2.3.4] param0"),
         TestCase("length", "int", "mscore", "4.0.0.0", ParameterKind.VALUE, false, false,
             "[System.Int32, mscore, 4.0.0.0] length"),
         TestCase("str", "string", "mscore", "4.0.0.0", ParameterKind.VALUE, false, false,
             "[System.String, mscore, 4.0.0.0] str"),
         TestCase("optional", "bool", "mscore", "4.0.0.0", ParameterKind.VALUE, true, false,
             "opt [System.Boolean, mscore, 4.0.0.0] optional"),
         TestCase("output", "Type", "Assembly", "0.1.9.2", ParameterKind.OUTPUT, false, false,
             "out [Type, Assembly, 0.1.9.2] output"),
         TestCase("reference", "long", "mscore", "4.0.0.0", ParameterKind.REFERENCE, false, false,
             "ref [System.Int64, mscore, 4.0.0.0] reference")]
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

            AssertName(paramMock.Object, ParameterName.Get(identifier));
        }

        [Test]
        public void ShouldGetUnknownParameterName()
        {
            AssertUnknownName<IParameter>(ParameterName.UnknownName);
        }

        [Test]
        public void ShouldGetNameForIField()
        {
            var fieldMock = new Mock<IField>();
            fieldMock.Setup(f => f.GetContainingType())
                     .Returns(TypeMockUtils.MockTypeElement("DeclaringType", "DTA", "0.9.8.7"));
            fieldMock.Setup(f => f.Type).Returns(TypeMockUtils.MockIType("ValueType", "VTA", "1.2.3.4"));
            fieldMock.Setup(f => f.ShortName).Returns("FieldName");

            AssertName(
                fieldMock.Object,
                FieldName.Get("[ValueType, VTA, 1.2.3.4] [DeclaringType, DTA, 0.9.8.7].FieldName"));
        }

        [Test]
        public void ShouldGetNameForStaticIField()
        {
            var fieldMock = new Mock<IField>();
            fieldMock.Setup(f => f.GetContainingType())
                     .Returns(TypeMockUtils.MockTypeElement("DeclaringType", "DTA", "0.9.8.7"));
            fieldMock.Setup(f => f.Type).Returns(TypeMockUtils.MockIType("ValueType", "VTA", "1.2.3.4"));
            fieldMock.Setup(f => f.ShortName).Returns("FieldName");
            fieldMock.Setup(f => f.IsStatic).Returns(true);

            AssertName(
                fieldMock.Object,
                FieldName.Get("static [ValueType, VTA, 1.2.3.4] [DeclaringType, DTA, 0.9.8.7].FieldName"));
        }

        [Test]
        public void ShouldGetUnknownFieldName()
        {
            AssertUnknownName<IField>(FieldName.UnknownName);
        }

        [Test]
        public void ShouldGetNameForIProperty()
        {
            var propertyMock = new Mock<IProperty>();
            propertyMock.Setup(p => p.GetContainingType())
                        .Returns(TypeMockUtils.MockTypeElement("DeclaringType", "DTA", "0.9.8.7"));
            propertyMock.Setup(p => p.ReturnType).Returns(TypeMockUtils.MockIType("ValueType", "VTA", "1.2.3.4"));
            propertyMock.Setup(p => p.ShortName).Returns("PropertyName");
            propertyMock.Setup(p => p.IsReadable).Returns(true);
            propertyMock.Setup(p => p.IsWritable).Returns(true);
            propertyMock.Setup(p => p.Parameters).Returns(new List<IParameter>());

            AssertName(
                propertyMock.Object,
                PropertyName.Get("set get [ValueType, VTA, 1.2.3.4] [DeclaringType, DTA, 0.9.8.7].PropertyName()"));
        }

        [Test]
        public void ShouldGetUnknownPropertyName()
        {
            AssertUnknownName<IProperty>(PropertyName.UnknownName);
        }

        [Test]
        public void ShouldGetNameForIFunction()
        {
            var functionMock = new Mock<IFunction>();
            functionMock.Setup(f => f.GetContainingType())
                        .Returns(TypeMockUtils.MockTypeElement("DeclaringType", "DTA", "0.9.8.7"));
            functionMock.Setup(f => f.ReturnType).Returns(TypeMockUtils.MockIType("ReturnType", "RTA", "1.2.3.4"));
            functionMock.Setup(f => f.ShortName).Returns("MethodName");
            functionMock.Setup(f => f.Parameters).Returns(new List<IParameter>());

            AssertName(
                functionMock.Object,
                MethodName.Get("[ReturnType, RTA, 1.2.3.4] [DeclaringType, DTA, 0.9.8.7].MethodName()"));
        }

        // TODO @Sven: Write tests for function with parameters

        [Test]
        public void ShouldGetNameForIMethodWithTypeParameters()
        {
            var method = new Mock<IMethod>();
            method.Setup(m => m.GetContainingType())
                  .Returns(TypeMockUtils.MockTypeElement("DeclaringType", "DTA", "0.9.8.7"));
            method.Setup(m => m.ReturnType).Returns(TypeMockUtils.MockIType("ReturnType", "RTA", "1.2.3.4"));
            method.Setup(m => m.ShortName).Returns("MethodName");
            method.Setup(m => m.Parameters).Returns(new List<IParameter>());

            method.Setup(m => m.TypeParameters).Returns(new[] {MockTypeParameter("T"), MockTypeParameter("U")});

            AssertName(
                method.Object,
                MethodName.Get("[ReturnType, RTA, 1.2.3.4] [DeclaringType, DTA, 0.9.8.7].MethodName`2[[T],[U]]()"));
        }

        // TODO @Sven: Write tests for type parameters

        [Test]
        public void ShouldGetNameForStaticIFunction()
        {
            var functionMock = new Mock<IFunction>();
            functionMock.Setup(f => f.GetContainingType())
                        .Returns(TypeMockUtils.MockTypeElement("DeclaringType", "DTA", "0.9.8.7"));
            functionMock.Setup(f => f.ReturnType).Returns(TypeMockUtils.MockIType("ReturnType", "RTA", "1.2.3.4"));
            functionMock.Setup(f => f.ShortName).Returns("MethodName");
            functionMock.Setup(f => f.Parameters).Returns(new List<IParameter>());
            functionMock.Setup(f => f.IsStatic).Returns(true);

            AssertName(
                functionMock.Object,
                MethodName.Get("static [ReturnType, RTA, 1.2.3.4] [DeclaringType, DTA, 0.9.8.7].MethodName()"));
        }

        [Test]
        public void ShouldGetUnknownMethodName()
        {
            AssertUnknownName<IMethod>(MethodName.UnknownName);
        }

        [Test]
        public void ShouldGetNameForVariableDeclaration()
        {
            var mockVariable = new Mock<ITypeOwner>();
            mockVariable.Setup(v => v.ShortName).Returns("variable");
            mockVariable.Setup(v => v.Type).Returns(TypeMockUtils.MockIType("Type", "Assembly", "1.2.3.4"));

            AssertName(mockVariable.Object, LocalVariableName.Get("[Type, Assembly, 1.2.3.4] variable"));
        }

        [Test]
        public void ShouldGetUnknownLocalVariableName()
        {
            AssertUnknownName<ITypeOwner>(LocalVariableName.UnknownName);
        }

        [Test]
        public void ShouldGetNameForEventDeclaration()
        {
            var mockEvent = new Mock<IEvent>();
            mockEvent.Setup(p => p.GetContainingType())
                     .Returns(TypeMockUtils.MockTypeElement("DeclaringType", "DTA", "0.9.8.7"));
            mockEvent.Setup(p => p.Type).Returns(TypeMockUtils.MockIType("HandlerType", "HTA", "1.2.3.4"));
            mockEvent.Setup(p => p.ShortName).Returns("EventName");

            AssertName(
                mockEvent.Object,
                EventName.Get("[HandlerType, HTA, 1.2.3.4] [DeclaringType, DTA, 0.9.8.7].EventName"));
        }

        [Test]
        public void ShouldGetUnknownEventName()
        {
            AssertUnknownName<IEvent>(EventName.UnknownName);
        }

        [Test]
        public void ShouldGetNameForAlias()
        {
            var aliasMock = new Mock<IAlias>();
            aliasMock.Setup(a => a.ShortName).Returns("global");

            AssertName(aliasMock.Object, AliasName.Get("global"));
        }

        [Test]
        public void ShouldGetUnknownAliasName()
        {
            AssertUnknownName<IAlias>(AliasName.UnknownName);
        }

        private static void AssertUnknownName<TDeclaredElement>(IName unknownName)
            where TDeclaredElement : class, IDeclaredElement
        {
            var unknownElement = new Mock<TDeclaredElement>();
            unknownElement.Setup(e => e.ShortName).Returns(SharedImplUtil.MISSING_DECLARATION_NAME);

            AssertName(unknownElement.Object, unknownName);
        }

        private static void AssertName<TDeclaredElement, TName>(TDeclaredElement declaredElement, TName expected)
            where TDeclaredElement : class, IDeclaredElement where TName : IName
        {
            AssertName(declaredElement, expected, EmptySubstitution.INSTANCE);
        }

        private static void AssertName<TDeclaredElement, TName>(TDeclaredElement declaredElement,
            TName expected,
            ISubstitution substitution) where TDeclaredElement : class, IDeclaredElement where TName : IName
        {
            var actual = declaredElement.GetName(substitution);
            Assert.AreSame(expected, actual);
        }

        private static ITypeParameter MockTypeParameter(string value)
        {
            var mockTypeParameter = new Mock<ITypeParameter>();
            mockTypeParameter.Setup(tp => tp.ShortName).Returns(value);

            var typeParameter = mockTypeParameter.Object;
            return typeParameter;
        }

        // ReSharper disable once UnusedMember.Local
        private static ISubstitution MockSubstitution(IDictionary<ITypeParameter, IType> substitution)
        {
            return new SubstitutionImpl(substitution.Keys.ToList(), substitution.Values.ToList());
        }
    }
}