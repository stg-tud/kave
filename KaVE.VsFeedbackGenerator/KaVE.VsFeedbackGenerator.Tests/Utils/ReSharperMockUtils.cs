using System;
using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Impl;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Properties;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Impl.Resolve;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.Util;
using KaVE.Model.Names.CSharp;
using Moq;
using AssemblyName = System.Reflection.AssemblyName;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    static class ReSharperMockUtils
    {
        internal static IType MockIType(string fqnOrAlias, string assemblyName, string assemblyVersion)
        {
            return MockIType(fqnOrAlias, EmptySubstitution.INSTANCE, assemblyName, assemblyVersion);
        }

        public static IType MockIType(string fqnOrAlias, IEnumerable<KeyValuePair<string, IType>> typeParameters, string assemblyName, string assemblyVersion)
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
            return MockIType(fqnOrAlias, new SubstitutionImpl(parameters, parameterSubstitutes), assemblyName, assemblyVersion);
        }

        private static IType MockIType(string fqnOrAlias,
            ISubstitution substitution,
            string assemblyName,
            string assemblyVersion)
        {
            var typeMock = new Mock<IDeclaredType>();
            var fqn = CSharpNameUtils.GetFullTypeNameFromTypeAlias(fqnOrAlias);
            var mockTypeElement = MockTypeElement(fqn, assemblyName, assemblyVersion);
            mockTypeElement.TypeParameters.AddRange(substitution.Domain);
            typeMock.Setup(t => t.GetTypeElement()).Returns(mockTypeElement);
            typeMock.Setup(t => t.GetLongPresentableName(CSharpLanguage.Instance)).Returns(fqnOrAlias);
            typeMock.Setup(t => t.Assembly).Returns(MockAssemblyInfo(assemblyName, assemblyVersion));
            var mockResolveResult = new Mock<IResolveResult>();
            mockResolveResult.Setup(rr => rr.Substitution).Returns(substitution);
            typeMock.Setup(t => t.Resolve()).Returns(mockResolveResult.Object);
            return typeMock.Object;
        }

        internal static ITypeElement MockTypeElement(string fqn, string assemblyName, string assemblyVersion)
        {
            return MockTypeElement(fqn, MockAssembly(assemblyName, assemblyVersion));
        }

        internal static ITypeElement MockTypeElement(string fqn, IModule module)
        {
            if (fqn.Contains("[["))
            {
                fqn = fqn.Substring(0, fqn.IndexOf("[[", System.StringComparison.Ordinal));
            }

            var clrMock = new Mock<IClrTypeName>();
            clrMock.Setup(clr => clr.FullName).Returns(fqn);

            var teMock = new Mock<ITypeElement>();
            teMock.Setup(te => te.GetClrName()).Returns(clrMock.Object);
            teMock.Setup(te => te.Module).Returns(MockPsiModule(module));
            teMock.Setup(te => te.TypeParameters).Returns(new List<ITypeParameter>());

            return teMock.Object;
        }

        private static AssemblyNameInfo MockAssemblyInfo(string name, string version)
        {
            var assembly = new AssemblyName { Name = name, Version = new Version(version) };
            return new AssemblyNameInfo(assembly);
        }

        private static IPsiModule MockPsiModule(IModule containingProjectModule)
        {
            var moduleMock = new Mock<IPsiModule>();
            moduleMock.Setup(m => m.ContainingProjectModule).Returns(containingProjectModule);
            return moduleMock.Object;
        }

        internal static IAssembly MockAssembly(string name, string version)
        {
            var mockAssembly = new Mock<IAssembly>();
            mockAssembly.Setup(a => a.Presentation).Returns(string.Format("{0}, Version={1}", name, version));
            mockAssembly.Setup(a => a.FullAssemblyName).Returns(string.Format("{0}, Version={1}", name, version));
            return mockAssembly.Object;
        }

        internal static IProject MockProject(string name, string version)
        {
            var mockProject = MockProjectImpl(name);
            var mockAssemblyInfo = new OutputAssemblyInfo(Guid.NewGuid(), MockAssemblyInfo(name, version), null);
            mockProject.OutputAssemblyInfo = mockAssemblyInfo;
            return mockProject;
        }

        internal static IProject MockUncompilableProject(string name)
        {
            return MockProjectImpl(name);
        }

        private static ProjectImpl MockProjectImpl(string name)
        {
            var fileSystemPath = FileSystemPath.TryParse(".");
            var mockLocks = new Mock<IShellLocks>();
            var mockSolutionElement = new SolutionElement(
                fileSystemPath,
                new Mock<ISolutionOwner>().Object,
                new Mock<ChangeManager>().Object,
                mockLocks.Object,
                null,
                null);
            return ProjectImpl.CreateProjectImpl(
                mockSolutionElement,
                new Mock<IProjectProperties>().Object,
                mockLocks.Object,
                Guid.NewGuid(),
                null,
                fileSystemPath,
                fileSystemPath,
                name);
        }
    }
}