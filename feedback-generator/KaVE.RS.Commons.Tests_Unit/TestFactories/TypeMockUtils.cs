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
 */

using System;
using System.Collections.Generic;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Impl.Resolve;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.Util;
using KaVE.Commons.Model.Names.CSharp;
using Moq;

namespace KaVE.RS.Commons.Tests_Unit.TestFactories
{
    public static class TypeMockUtils
    {
        public static IType MockIType(string fqnOrAlias, string assemblyName, string assemblyVersion)
        {
            return MockIType(fqnOrAlias, EmptySubstitution.INSTANCE, assemblyName, assemblyVersion);
        }

        public static IType MockIType(string fqnOrAlias,
            IEnumerable<KeyValuePair<string, IType>> typeParameters,
            string assemblyName,
            string assemblyVersion)
        {
            var parameters = new List<ITypeParameter>();
            var parameterSubstitutes = new List<IType>();
            foreach (var typeParameter in typeParameters)
            {
                var mockTypeParameter = new Mock<ITypeParameter>();
                mockTypeParameter.Setup(tp => tp.ShortName).Returns(typeParameter.Key);
                parameters.Add(mockTypeParameter.Object);
                parameterSubstitutes.Add(typeParameter.Value);
            }
            return MockIType(
                fqnOrAlias,
                new SubstitutionImpl(parameters, parameterSubstitutes),
                assemblyName,
                assemblyVersion);
        }

        private static IType MockIType(string fqnOrAlias,
            ISubstitution substitution,
            string assemblyName,
            string assemblyVersion)
        {
            var typeMock = new Mock<IDeclaredType>();
            typeMock.Setup(t => t.Classify).Returns(TypeClassification.REFERENCE_TYPE);
            var fqn = CSharpNameUtils.GetFullTypeNameFromTypeAlias(fqnOrAlias);
            var mockTypeElement = MockTypeElement(fqn, assemblyName, assemblyVersion);
            mockTypeElement.TypeParameters.AddRange(substitution.Domain);
            typeMock.Setup(t => t.GetTypeElement()).Returns(mockTypeElement);
            typeMock.Setup(t => t.GetLongPresentableName(CSharpLanguage.Instance)).Returns(fqnOrAlias);
            typeMock.Setup(t => t.Assembly).Returns(MockAssemblyNameInfo(assemblyName, assemblyVersion));
            var mockResolveResult = new Mock<IResolveResult>();
            mockResolveResult.Setup(rr => rr.Substitution).Returns(substitution);
            typeMock.Setup(t => t.Resolve()).Returns(mockResolveResult.Object);
            return typeMock.Object;
        }

        public static IType MockTypeParamIType(string name)
        {
            var typeMock = new Mock<IDeclaredType>();
            typeMock.Setup(t => t.Classify).Returns(TypeClassification.UNKNOWN);
            return typeMock.Object;
        }

        public static ITypeElement MockTypeElement(string fqn, string assemblyName, string assemblyVersion)
        {
            return MockTypeElement(fqn, MockAssembly(assemblyName, assemblyVersion));
        }

        public static ITypeElement MockTypeElement(string fqn, IModule module)
        {
            if (fqn.Contains("[["))
            {
                fqn = fqn.Substring(0, fqn.IndexOf("[[", StringComparison.Ordinal));
            }

            var clrMock = new Mock<IClrTypeName>();
            clrMock.Setup(clr => clr.FullName).Returns(fqn);

            var teMock = new Mock<ITypeElement>();
            teMock.Setup(te => te.GetClrName()).Returns(clrMock.Object);
            teMock.Setup(te => te.Module).Returns(MockPsiModule(module));
            teMock.Setup(te => te.TypeParameters).Returns(new List<ITypeParameter>());

            return teMock.Object;
        }

        private static IPsiModule MockPsiModule(IModule containingProjectModule)
        {
            var moduleMock = new Mock<IPsiModule>();
            moduleMock.Setup(m => m.ContainingProjectModule).Returns(containingProjectModule);
            return moduleMock.Object;
        }

        public static IAssembly MockAssembly(string name, string version)
        {
            var mockAssembly = new Mock<IAssembly>();
            mockAssembly.Setup(a => a.AssemblyName).Returns(MockAssemblyNameInfo(name, version));
            return mockAssembly.Object;
        }

        public static IProject MockProject(string name)
        {
            var project = Mock.Of<IProject>();
            Mock.Get(project).Setup(p => p.Name).Returns(name);
            return project;
        }

        private static AssemblyNameInfo MockAssemblyNameInfo(string name, string version)
        {
            return AssemblyNameInfo.Create(name, new Version(version));
        }
    }
}