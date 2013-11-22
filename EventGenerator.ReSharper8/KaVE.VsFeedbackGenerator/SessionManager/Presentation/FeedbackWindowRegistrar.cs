using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.ToolWindowManagement;

namespace KaVE.EventGenerator.ReSharper8.SessionManager.Presentation
{
    [ShellComponent]
    internal class FeedbackWindowRegistrar
    {
        public FeedbackWindowRegistrar(Lifetime lifetime,
            ToolWindowManager toolWindowManager,
            FeedbackWindowDescriptor descriptor,
            SessionHolder sessionHolder)
        {
            var toolWindowClass = toolWindowManager.Classes[descriptor];
            toolWindowClass.RegisterEmptyContent(
                lifetime,
                lt =>
                {
                    var window = new FeedbackWindowControl(sessionHolder);
                    var control = new EitherControl(window);
                    return control.BindToLifetime(lt);
                });
        }
    }
}