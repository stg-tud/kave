using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.ToolWindowManagement;
using Microsoft.VisualStudio.Shell.Interop;

namespace KaVE.EventGenerator.ReSharper8.SessionManager.Presentation
{
    [ShellComponent]
    internal class FeedbackWindowRegistrar
    {
        private readonly Lifetime _lifetime;

        public FeedbackWindowRegistrar(Lifetime lifetime,
            ToolWindowManager toolWindowManager,
            IVsUIShell shell,
            FeedbackWindowDescriptor descriptor)
        {
            this._lifetime = lifetime;

            ToolWindowClass toolWindowClass = toolWindowManager.Classes[descriptor];
            toolWindowClass.RegisterEmptyContent(
                lifetime,
                lt =>
                {
                    var window = new FeedbackWindowControl(shell);
                    var control = new EitherControl(window);
                    return control.BindToLifetime(lifetime);
                });
        }
    }
}