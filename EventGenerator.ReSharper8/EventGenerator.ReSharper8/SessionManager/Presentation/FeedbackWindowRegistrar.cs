using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.DataFlow;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.ToolWindowManagement;
using Microsoft.VisualStudio.Shell.Interop;

namespace KaVE.EventGenerator.ReSharper8.SessionManager.Presentation
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class FeedbackWindowRegistrar
    {
        public FeedbackWindowRegistrar(Lifetime lifetime,
            ToolWindowManager toolWindowManager,
            IVsUIShell shell,
            FeedbackWindowDescriptor descriptor)
        {
            var toolWindowClass = toolWindowManager.Classes[descriptor];
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