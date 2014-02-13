using System.Windows.Threading;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.ToolWindowManagement;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    [ShellComponent]
    internal class SessionManagerWindowRegistrar
    {
        private readonly Dispatcher _windowDispatcher;

        public SessionManagerWindowRegistrar(Lifetime lifetime,
            ToolWindowManager toolWindowManager,
            SessionManagerWindowDescriptor descriptor,
            FeedbackView feedbackView)
        {
            var toolWindowClass = toolWindowManager.Classes[descriptor];
            toolWindowClass.RegisterEmptyContent(
                lifetime,
                lt =>
                {
                    var window = new SessionManagerControl(feedbackView);
                    var control = new EitherControl(window);
                    return control.BindToLifetime(lt);
                });

            _windowDispatcher = Dispatcher.CurrentDispatcher;
        }

        public Dispatcher WindowDispatcher
        {
            get { return _windowDispatcher; }
        }
    }
}