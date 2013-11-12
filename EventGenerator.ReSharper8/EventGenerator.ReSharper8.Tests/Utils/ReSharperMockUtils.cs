using System;
using System.Reflection;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Modules;
using Moq;

namespace KaVE.EventGenerator.ReSharper8.Tests.Utils
{
    static class ReSharperMockUtils
    {
        internal static IType MockIType(string fqnOrAlias, string assemblyName, string assemblyVersion)
        {
            var typeMock = new Mock<IDeclaredType>();
            typeMock.Setup(t => t.GetLongPresentableName(CSharpLanguage.Instance)).Returns(fqnOrAlias);
            typeMock.Setup(t => t.Assembly).Returns(MockAssembly(assemblyName, assemblyVersion));
            return typeMock.Object;
        }

        public static ITypeElement MockTypeElement(string fqn, string assemblyName, string assemblyVersion)
        {
            var clrMock = new Mock<IClrTypeName>();
            clrMock.Setup(clr => clr.FullName).Returns(fqn);

            var teMock = new Mock<ITypeElement>();
            teMock.Setup(te => te.GetClrName()).Returns(clrMock.Object);
            teMock.Setup(te => te.Module).Returns(MockModule(assemblyName, assemblyVersion));

            return teMock.Object;
        }

        static AssemblyNameInfo MockAssembly(string name, string version)
        {
            var assembly = new AssemblyName { Name = name, Version = new Version(version) };
            return new AssemblyNameInfo(assembly);
        }

        static IPsiModule MockModule(string name, string version)
        {
            var containingProjectModuleMock = new Mock<IAssembly>();
            containingProjectModuleMock.Setup(a => a.Presentation).Returns(String.Format("{0}, Version={1}", name, version));

            var moduleMock = new Mock<IPsiModule>();
            moduleMock.Setup(m => m.ContainingProjectModule).Returns(containingProjectModuleMock.Object);
            return moduleMock.Object;
        }
    }
}