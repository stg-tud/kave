
using System;
using JetBrains.Application;
using JetBrains.Threading;
using JetBrains.UI.Tooltips;
using JetBrains.Util;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;

namespace KaVE.VsFeedbackGenerator.VsIntegration
{
    [ShellComponent]
    public class UploadReminder
    {
        private System.Threading.Timer _weeklyTimer;
        private ITooltipManager _tooltipManager;
        private const string SessionManagerActionId = "KaVE.VsFeedbackGenerator.SessionManager";

        public UploadReminder(ITooltipManager tooltipManager)
        {
            _tooltipManager = tooltipManager;
            _weeklyTimer = CreateWeeklyTimer(TimeSpan.FromMinutes(new Random().Next(0, 60)));
        }

        private System.Threading.Timer CreateWeeklyTimer(TimeSpan minuteOffset)
        {
            return new System.Threading.Timer(ExecuteOnceAWeek, null, TimeSpan.FromMinutes(0.5), TimeSpan.Zero);
        }

        private void ExecuteOnceAWeek(Object obj)
        {
            var result = MessageBox.ShowYesNoCancel("Upload it or die");
            if (result == true)
            {
                var window = Shell.Instance.GetComponent<SessionManagerWindowRegistrar>();

                Invoke.OnDispatcherAsync(
                    new Action(
                        () =>
                            ReentrancyGuard.Current.Execute(
                                SessionManagerActionId,
                                new Action(() => window.ToolWindow.Show()))));
             }
        }
    }
}
