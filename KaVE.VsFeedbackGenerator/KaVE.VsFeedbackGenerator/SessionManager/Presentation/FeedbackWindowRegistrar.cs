using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.ToolWindowManagement;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    [ShellComponent]
    internal class FeedbackWindowRegistrar
    {
        public FeedbackWindowRegistrar(Lifetime lifetime,
            ToolWindowManager toolWindowManager,
            FeedbackWindowDescriptor descriptor,
            FeedbackView feedbackView)
        {
            var toolWindowClass = toolWindowManager.Classes[descriptor];
            toolWindowClass.RegisterEmptyContent(
                lifetime,
                lt =>
                {
                    var window = new FeedbackWindowControl(feedbackView);
                    var control = new EitherControl(window);
                    return control.BindToLifetime(lt);
                });
        }
    }
}