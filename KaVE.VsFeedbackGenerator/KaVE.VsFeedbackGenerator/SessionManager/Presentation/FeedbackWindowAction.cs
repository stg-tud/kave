using JetBrains.ActionManagement;
using JetBrains.UI.ToolWindowManagement;

namespace KaVE.EventGenerator.ReSharper8.SessionManager.Presentation
{
    [ActionHandler("EventGenerator.ReSharper8.ShowFeedbackWindow")]
    class FeedbackWindowAction : ActivateToolWindowActionHandler<FeedbackWindowDescriptor>
    {

    }
}
