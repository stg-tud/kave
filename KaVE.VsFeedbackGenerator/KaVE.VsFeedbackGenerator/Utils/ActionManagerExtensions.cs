using JetBrains.ActionManagement;
using JetBrains.DataFlow;
using JetBrains.Threading;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public static class ActionManagerExtensions
    {
        public static void ExecuteActionGuarded(this IActionManager actionManager, string actionId, IThreading threading, string executeName)
        {
            var action = actionManager.GetExecutableAction(actionId);
            if (action != null)
            {
                threading.ReentrancyGuard.ExecuteOrQueue(EternalLifetime.Instance, executeName,
                    () => actionManager.ExecuteActionIfAvailable(action));
            }
        }
    }
}
