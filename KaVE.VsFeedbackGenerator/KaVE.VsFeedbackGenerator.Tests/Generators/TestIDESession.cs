using EnvDTE;
using KaVE.VsFeedbackGenerator.VsIntegration;
using Moq;

namespace KaVE.VsFeedbackGenerator.Tests.Generators
{
    internal class TestIDESession : IIDESession
    {
        private readonly Mock<DTE> _mockDTE;

        public TestIDESession()
        {
            _mockDTE = new Mock<DTE>();
            _mockDTE.Setup(dte => dte.ActiveWindow).Returns((Window) null);
            _mockDTE.Setup(dte => dte.ActiveDocument).Returns((Document) null);
        }

        public Mock<DTE> MockDTE
        {
            get { return _mockDTE; }
        }

        public string UUID
        {
            get { return "TestIDESessionUUID"; }
        }

        public DTE DTE
        {
            get { return _mockDTE.Object; }
        }
    }
}