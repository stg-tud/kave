using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.ToolWindowManagement;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    [ShellComponent]
    public class SessionManagerWindowRegistrar
    {
        private readonly ToolWindowClass _toolWindowClass;

        public SessionManagerWindowRegistrar(Lifetime lifetime,
            ToolWindowManager toolWindowManager,
            SessionManagerWindowDescriptor descriptor,
            FeedbackViewModel feedbackViewModel)
        {
            _toolWindowClass = toolWindowManager.Classes[descriptor];
            _toolWindowClass.RegisterEmptyContent(
                lifetime,
                lt =>
                {
                    var window = new SessionManagerControl(feedbackViewModel);
                    var control = new EitherControl(window);
                    return control.BindToLifetime(lt);
                });

        }

        public ToolWindowClass ToolWindow
        {
            get { return _toolWindowClass; }
        }
    }
}