using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.ToolWindowManagement;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    [ShellComponent]
    public class SessionManagerWindowRegistrar
    {
        public SessionManagerWindowRegistrar(Lifetime lifetime,
            ToolWindowManager toolWindowManager,
            SessionManagerWindowDescriptor descriptor,
            FeedbackViewModel feedbackViewModel)
        {
            var toolWindowClass = toolWindowManager.Classes[descriptor];
            toolWindowClass.RegisterEmptyContent(
                lifetime,
                lt =>
                {
                    var window = new SessionManagerControl(feedbackViewModel);
                    var control = new EitherControl(window);
                    return control.BindToLifetime(lt);
                });

        }
    }
}