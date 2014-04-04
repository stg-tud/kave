using JetBrains.Application;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Utils
{
    [TestFixture]
    internal class RegistryTest
    {
        [SetUp]
        public void SetUp()
        {
            Registry.Clear();
        }

        [Test]
        public void ShouldReturnShellComponent()
        {
            var component = Registry.GetComponent<ITestShellComponent>();

            Assert.IsTrue(component.IsReadDeal);
        }

        [Test]
        public void ShouldReturnMockComponent()
        {
            var testShellComponent = CreateMockComponent();

            Registry.RegisterComponent(testShellComponent);
            var component = Registry.GetComponent<ITestShellComponent>();

            Assert.IsFalse(component.IsReadDeal);
        }

        [Test]
        public void ShouldClearRegistrations()
        {
            var testShellComponent = CreateMockComponent();

            Registry.RegisterComponent(testShellComponent);
            Registry.Clear();
            var component = Registry.GetComponent<ITestShellComponent>();

            Assert.IsTrue(component.IsReadDeal);
        }

        private static ITestShellComponent CreateMockComponent()
        {
            var mock = new Mock<ITestShellComponent>();
            mock.Setup(c => c.IsReadDeal).Returns(false);
            return mock.Object;
        }
    }

    public interface ITestShellComponent
    {
        bool IsReadDeal { get; }
    }

    [ShellComponent]
    public class TestShellComponent : ITestShellComponent
    {
        public bool IsReadDeal
        {
            get { return true; }
        }
    }
}