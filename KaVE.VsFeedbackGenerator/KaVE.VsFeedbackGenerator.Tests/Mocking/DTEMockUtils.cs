using EnvDTE;
using KaVE.JetBrains.Annotations;
using Moq;

namespace KaVE.VsFeedbackGenerator.Tests.Mocking
{
    static class DTEMockUtils
    {
        public static Solution MockSolution([NotNull] string fullName)
        {
            var mockSolution = new Mock<Solution>();
            mockSolution.Setup(solution => solution.FullName).Returns(fullName);
            mockSolution.Setup(solution => solution.DTE).Returns(MockDTE(mockSolution.Object));
            return mockSolution.Object;
        }

        private static DTE MockDTE(Solution solution = null)
        {
            var mockDTE = new Mock<DTE>();
            mockDTE.Setup(dte => dte.Solution).Returns(solution);
            return mockDTE.Object;
        }
    }
}
