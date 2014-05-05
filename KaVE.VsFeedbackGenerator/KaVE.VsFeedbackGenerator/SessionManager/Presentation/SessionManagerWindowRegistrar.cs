using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.ToolWindowManagement;
using KaVE.JetBrains.Annotations;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    [ShellComponent]
    public class SessionManagerWindowRegistrar
    {
        [UsedImplicitly]
        private readonly Lifetime _lifetime;
        [UsedImplicitly]
        private readonly ToolWindowClass _toolWindowClass;

        public SessionManagerWindowRegistrar(Lifetime lifetime,
            ToolWindowManager toolWindowManager,
            SessionManagerWindowDescriptor descriptor,
            FeedbackViewModel feedbackViewModel)
        {
            // objects are kept in fields to prevent garbage collection
            _lifetime = lifetime;
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
    }
}