using JetBrains.ActionManagement;
using JetBrains.UI.ToolWindowManagement;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    [ActionHandler(ActionId)]
    class SessionManagerWindowActionHandler : ActivateToolWindowActionHandler<SessionManagerWindowDescriptor>
    {
        private const string ActionId = "KaVE.VsFeedbackGenerator.SessionManager";
    }
}
