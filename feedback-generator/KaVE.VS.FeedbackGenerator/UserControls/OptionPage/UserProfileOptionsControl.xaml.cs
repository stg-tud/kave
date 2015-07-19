/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;
using JetBrains.UI.Resources;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfile;
using KaVEISettingsStore = KaVE.RS.Commons.Settings.ISettingsStore;


namespace KaVE.VS.FeedbackGenerator.UserControls.OptionPage
{
    [OptionsPage(PID, "User Profile", typeof (OptionsThemedIcons.EnvironmentGeneral),
        ParentId = RootOptionPage.PID, Sequence = 3.0)]
    public partial class UserProfileOptionsControl : IOptionsPage
    {
        private const string PID = "KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UserProfileOptionsControl";

        private readonly Lifetime _lifetime;
        private readonly OptionsSettingsSmartContext _ctx;
        private readonly KaVEISettingsStore _settingsStore;
        private readonly UserProfileSettings _userProfileSettings;
        private readonly UserProfileContext _userProfileContext;

        public UserProfileOptionsControl(Lifetime lifetime,
            OptionsSettingsSmartContext ctx,
            KaVEISettingsStore settingsStore,
            OptionPageViewModel optionPageViewModel,
            IRandomizationUtils rnd)
        {
            _lifetime = lifetime;
            _ctx = ctx;
            _settingsStore = settingsStore;

            InitializeComponent();

            var exportSettings = settingsStore.GetSettings<ExportSettings>();
            _userProfileSettings = settingsStore.GetSettings<UserProfileSettings>();

            _userProfileContext = new UserProfileContext(exportSettings, _userProfileSettings, rnd);
            _userProfileContext.PropertyChanged += UserProfileContextOnPropertyChanged;

            optionPageViewModel.UserProfileContext = _userProfileContext;

            DataContext = optionPageViewModel;
            
            if (_ctx != null)
            {
                BindToUserProfileChanges();
            }
        }

        public bool OnOk()
        {
            // TODO: validation
            _settingsStore.SetSettings(_userProfileSettings);
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

        #region jetbrains smart-context bindings

        private void BindToUserProfileChanges()
        {
            // IsProviding
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.IsProvidingProfile,
                UserProfile.IsProvidingProfileCheckBox,
                ToggleButton.IsCheckedProperty);

            // ProfileId
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => s.ProfileId,
                UserProfile.UserSettingsGrid.ProfileIdTextBox,
                TextBox.TextProperty);

            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => s.Education,
                this,
                EducationProperty);

            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => s.Position,
                this,
                PositionProperty);

            BindingForProjects();
            BindingForTeams();

            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => s.CodeReviews,
                this,
                CodeReviewsProperty);

            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => s.ProgrammingGeneral,
                this,
                ProgrammingGeneralProperty);

            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => s.ProgrammingCSharp,
                this,
                ProgrammingCSharpProperty);
        }

        private void BindingForProjects()
        {
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.ProjectsCourses,
                UserProfile.UserSettingsGrid.ProjectsCoursesCheckBox,
                ToggleButton.IsCheckedProperty);
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.ProjectsPersonal,
                UserProfile.UserSettingsGrid.ProjectsPersonalCheckBox,
                ToggleButton.IsCheckedProperty);
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.ProjectsSharedSmall,
                UserProfile.UserSettingsGrid.ProjectsSharedSmallCheckBox,
                ToggleButton.IsCheckedProperty);
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.ProjectsSharedMedium,
                UserProfile.UserSettingsGrid.ProjectsSharedMediumCheckBox,
                ToggleButton.IsCheckedProperty);
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.ProjectsSharedLarge,
                UserProfile.UserSettingsGrid.ProjectsSharedLargeCheckBox,
                ToggleButton.IsCheckedProperty);
        }

        private void BindingForTeams()
        {
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.TeamsSolo,
                UserProfile.UserSettingsGrid.TeamsSoloCheckBox,
                ToggleButton.IsCheckedProperty);
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.TeamsSmall,
                UserProfile.UserSettingsGrid.TeamsSmallCheckBox,
                ToggleButton.IsCheckedProperty);
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.TeamsMedium,
                UserProfile.UserSettingsGrid.TeamsMediumCheckBox,
                ToggleButton.IsCheckedProperty);
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.TeamsLarge,
                UserProfile.UserSettingsGrid.TeamsLargeCheckBox,
                ToggleButton.IsCheckedProperty);
        }

        #endregion

        #region ugly dependency property hack for JetBrains smart context

        private void UserProfileContextOnPropertyChanged(object sender,
            PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var propertyName = propertyChangedEventArgs.PropertyName;
            switch (propertyName)
            {
                case "Education":
                    Education = _userProfileContext.Education;
                    break;
                case "Position":
                    Position = _userProfileContext.Position;
                    break;
                case "CodeReviews":
                    CodeReviews = _userProfileContext.CodeReviews;
                    break;
                case "ProgrammingGeneral":
                    ProgrammingGeneral = _userProfileContext.ProgrammingGeneral;
                    break;
                case "ProgrammingCSharp":
                    ProgrammingCSharp = _userProfileContext.ProgrammingCSharp;
                    break;
            }
        }

        public static readonly DependencyProperty EducationProperty = DependencyProperty.Register(
            "Education",
            typeof (Educations),
            typeof (GeneralOptionsControl)
            );

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position",
            typeof (Positions),
            typeof (GeneralOptionsControl)
            );

        public static readonly DependencyProperty CodeReviewsProperty = DependencyProperty.Register(
            "CodeReviews",
            typeof (YesNoUnknown),
            typeof (GeneralOptionsControl)
            );

        public static readonly DependencyProperty ProgrammingGeneralProperty = DependencyProperty.Register(
            "ProgrammingGeneral",
            typeof (Likert7Point),
            typeof (GeneralOptionsControl)
            );

        public static readonly DependencyProperty ProgrammingCSharpProperty = DependencyProperty.Register(
            "ProgrammingCSharp",
            typeof (Likert7Point),
            typeof (GeneralOptionsControl)
            );

        public Educations Education
        {
            get { return (Educations) GetValue(EducationProperty); }
            set { SetValue(EducationProperty, value); }
        }

        public Positions Position
        {
            get { return (Positions) GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public YesNoUnknown CodeReviews
        {
            get { return (YesNoUnknown) GetValue(CodeReviewsProperty); }
            set { SetValue(CodeReviewsProperty, value); }
        }

        public Likert7Point ProgrammingGeneral
        {
            get { return (Likert7Point) GetValue(ProgrammingGeneralProperty); }
            set { SetValue(ProgrammingGeneralProperty, value); }
        }

        public Likert7Point ProgrammingCSharp
        {
            get { return (Likert7Point) GetValue(ProgrammingCSharpProperty); }
            set { SetValue(ProgrammingCSharpProperty, value); }
        }

        #endregion
    }
}