using System;
using System.Reflection;
using JetBrains.Application;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Impl;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Properties;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util;
using Moq;

namespace KaVE.EventGenerator.ReSharper8.Tests.Utils
{
    static class ReSharperMockUtils
    {
        internal static IType MockIType(string fqnOrAlias, string assemblyName, string assemblyVersion)
        {
            var typeMock = new Mock<IDeclaredType>();
            typeMock.Setup(t => t.GetLongPresentableName(CSharpLanguage.Instance)).Returns(fqnOrAlias);
            typeMock.Setup(t => t.Assembly).Returns(MockAssemblyInfo(assemblyName, assemblyVersion));
            return typeMock.Object;
        }

        internal static ITypeElement MockTypeElement(string fqn, string assemblyName, string assemblyVersion)
        {
            return MockTypeElement(fqn, MockAssembly(assemblyName, assemblyVersion));
        }

        internal static ITypeElement MockTypeElement(string fqn, IModule module)
        {
            var clrMock = new Mock<IClrTypeName>();
            clrMock.Setup(clr => clr.FullName).Returns(fqn);

            var teMock = new Mock<ITypeElement>();
            teMock.Setup(te => te.GetClrName()).Returns(clrMock.Object);
            teMock.Setup(te => te.Module).Returns(MockPsiModule(module));

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
            mockProject.OutputAssembly = MockAssembly(name, version);
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