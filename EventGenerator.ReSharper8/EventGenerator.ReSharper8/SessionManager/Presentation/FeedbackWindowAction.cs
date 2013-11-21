using JetBrains.ActionManagement;
using JetBrains.UI.ToolWindowManagement;

namespace KaVE.EventGenerator.ReSharper8.SessionManager.Presentation
{
    [ActionHandler("SessionManager.ShowFeedbackWindow")]
    class FeedbackWindowAction : ActivateToolWindowActionHandler<FeedbackWindowDescriptor>
    {

    }
}
