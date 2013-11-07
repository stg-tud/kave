using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
using JetBrains.ReSharper.Psi.Resources;
using Moq;
using NUnit.Framework;
using KaVE.EventGenerator.ReSharper8.Utils;

namespace KaVE.EventGenerator.ReSharper8.Tests.Utils
{
    [TestFixture]
    public class ReSharperCSharpNameFactoryTest
    {
        [Test]
        public void ShouldTranslateNamespaceToName()
        {
            var nsMock = new Mock<INamespace>();
            nsMock.Setup(ns => ns.QualifiedName).Returns("My.Test.Namespace");

            var namespaceName = nsMock.Object.GetName();

            Assert.AreEqual("My.Test.Namespace", namespaceName.Identifier);
        }
    }
}
