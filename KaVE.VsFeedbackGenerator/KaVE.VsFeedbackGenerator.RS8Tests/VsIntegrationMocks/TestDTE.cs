using EnvDTE;
using JetBrains.Application;
using KaVE.VsFeedbackGenerator.VsIntegration;
using Moq;

namespace KaVE.VsFeedbackGenerator.RS8Tests.VsIntegrationMocks
{
    [ShellComponent]
    public class TestIDESession : IIDESession
    {
        private readonly Mock<DTE> _mockDTE = new Mock<DTE>();

        public string UUID
        {
            get
            {
                return "TestIDESessionUUID";
            }
        }

        public DTE DTE
        {
            get
            {
                return _mockDTE.Object;
            }
        }
    }
}
