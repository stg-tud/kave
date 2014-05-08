using System;
using System.Linq.Expressions;
using System.Windows.Controls.Primitives;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Features.Common.Options;
using JetBrains.ReSharper.Features.Finding.Resources;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    [OptionsPage(PID, "KaVE Feedback", typeof (FeaturesFindingThemedIcons.SearchOptionsPage),
        ParentId = ToolsPage.PID)]
    public partial class FeedbackGeneratorOptionPage : IOptionsPage
    {
        private const string PID = "FeedbackGenerator.OptionPage";

        public FeedbackGeneratorOptionPage(Lifetime lifetime, OptionsSettingsSmartContext ctx)
        {
            InitializeComponent();
            SetToggleButtonBinding(ctx, lifetime, s => (bool?) s.RemoveCodeNames, RemoveCodeNamesCheckBox);
            SetToggleButtonBinding(ctx, lifetime, s => (bool?) s.RemoveStartTimes, RemoveStartTimesCheckBox);
            SetToggleButtonBinding(ctx, lifetime, s => (bool?) s.RemoveDurations, RemoveDurationsCheckBox);
            SetToggleButtonBinding(ctx, lifetime, s => (bool?) s.RemoveSessionIDs, RemoveSessionUUIDCheckBox);
        }

        private static void SetToggleButtonBinding(IContextBoundSettingsStore ctx,
            Lifetime lifetime,
            Expression<Func<ExportSettings, bool?>> settingProperty,
            ToggleButton toggleButton)
        {
            ctx.SetBinding(lifetime, settingProperty, toggleButton, ToggleButton.IsCheckedProperty);
        }

        public bool OnOk()
        {
            return true;
        }

        public bool ValidatePage()
        {
            return true;
        }

        public EitherControl Control
        {
            get { return this; }
        }

        public string Id
        {
            get { return PID; }
        }
    }
}