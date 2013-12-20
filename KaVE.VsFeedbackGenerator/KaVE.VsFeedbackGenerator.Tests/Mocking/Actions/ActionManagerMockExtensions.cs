using JetBrains.ActionManagement;
using Moq;

namespace KaVE.VsFeedbackGenerator.Tests.Mocking.Actions
{
    internal static class ActionManagerMockExtensions
    {
        public static void SetupExecutableAction(this Mock<IActionManager> manager, string actionId)
        {
            // TODO configure triggerable action mocks...
            manager.Setup(m => m.GetExecutableAction(actionId)).Returns(new MockExecutableAction(actionId));
        }
    }
}