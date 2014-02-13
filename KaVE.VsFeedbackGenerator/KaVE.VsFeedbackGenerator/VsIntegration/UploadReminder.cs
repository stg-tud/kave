
using System;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.UI.Tooltips;
using JetBrains.Util;
using KaVE.Utils;
using KaVE.JetBrains.Annotations;

namespace KaVE.VsFeedbackGenerator.VsIntegration
{
    [ShellComponent]
    public class UploadReminder
    {
        [NotNull]
        private readonly IActionManager _actionManager;

        private System.Threading.Timer _weeklyTimer;
        private readonly Lifetime _lifetime;
        private ITooltipManager _tooltipManager;
        private const string SessionManagerActionId = "KaVE.VsFeedbackGenerator.SessionManager";

        public UploadReminder([NotNull] IActionManager manager, ITooltipManager tooltipManager, Lifetime lifetime)
        {
            _actionManager = manager;
            _tooltipManager = tooltipManager;
            _lifetime = lifetime;
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
                var sessionManagerAction = _actionManager.TryGetAction(SessionManagerActionId) as IExecutableAction;
                
                if (sessionManagerAction != null)
                {
                    //var window = Shell.Instance.GetComponent<SessionManagerWindowRegistrar>();
                    //window.WindowDispatcher.Invoke(
                    Invoke.OnDispatcherAsync(
                     new Action(
                            () => sessionManagerAction.Execute(_actionManager.DataContexts.CreateOnApplicationWideState(_lifetime))));
                    
                }
            }
        }
    }
}
