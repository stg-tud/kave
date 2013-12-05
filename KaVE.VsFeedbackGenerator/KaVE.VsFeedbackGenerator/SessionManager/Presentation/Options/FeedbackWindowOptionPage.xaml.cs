using System.Windows.Controls;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Features.Common.Options;
using JetBrains.ReSharper.Features.Finding.Resources;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation.Options
{
    [OptionsPage(PID, "FeedbackGeneratorOptions", typeof(FeaturesFindingThemedIcons.SearchOptionsPage), ParentId = ToolsPage.PID)]
    public partial class FeedbackWindowOptionPage : UserControl, IOptionsPage
    {
        private const string PID = "FeedbackGenerator.OptionPage";

        public FeedbackWindowOptionPage(Lifetime lifetime, OptionsSettingsSmartContext ctx)
        {
            InitializeComponent();
            /*ctx.SetBinding(lifetime, (FeedbackWindowSettingsResharper s) => s.FeedbackGeneratorNames, NamesCheckBox, CheckBox.IsCheckedProperty);
            ctx.SetBinding(lifetime, (FeedbackWindowSettingsResharper s) => s.FeedbackGeneratorStartTime, StartTimeCheckBox, CheckBox.IsCheckedProperty);
            ctx.SetBinding(lifetime, (FeedbackWindowSettingsResharper s) => s.FeedbackGeneratorDuration, DurationCheckBox, CheckBox.IsCheckedProperty);
            ctx.SetBinding(lifetime, (FeedbackWindowSettingsResharper s) => s.FeedbackGeneratorSessionIDs, SessionIDCheckBox, CheckBox.IsCheckedProperty);*/
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
