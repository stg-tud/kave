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
using JetBrains.Application.DataContext;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;
using JetBrains.UI.Resources;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.RS.Commons;
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.UserControls.OptionPage.GeneralOptions;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfile;
using KaVE.VS.FeedbackGenerator.Utils;
using KaVEISettingsStore = KaVE.RS.Commons.Settings.ISettingsStore;


namespace KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UserProfileOptions
{
    [OptionsPage(PID, "User Profile", typeof (OptionsThemedIcons.EnvironmentGeneral),
        ParentId = RootOptionPage.PID, Sequence = 3.0)]
    public partial class UserProfileOptionsControl : IOptionsPage
    {
        private const string PID =
            "KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UserProfileOptions.UserProfileOptionsControl";

        public const ResetTypes ResetType = ResetTypes.UserProfileSettings;

        private readonly Lifetime _lifetime;
        private readonly OptionsSettingsSmartContext _ctx;
        private readonly KaVEISettingsStore _settingsStore;
        private readonly IActionExecutor _actionExecutor;
        private readonly DataContexts _dataContexts;
        private readonly UserProfileSettings _userProfileSettings;
        private readonly UserProfileContext _userProfileContext;

        public UserProfileOptionsControl(Lifetime lifetime,
            OptionsSettingsSmartContext ctx,
            KaVEISettingsStore settingsStore,
            IActionExecutor actionExecutor,
            DataContexts dataContexts,
            IMessageBoxCreator messageBoxCreator,
            IUserProfileSettingsUtils userProfileUtils)
        {
            _messageBoxCreator = messageBoxCreator;
            _userProfileUtils = userProfileUtils;
            _lifetime = lifetime;
            _ctx = ctx;
            _settingsStore = settingsStore;
            _actionExecutor = actionExecutor;
            _dataContexts = dataContexts;

            InitializeComponent();

            userProfileUtils.EnsureProfileId();

            _userProfileSettings = userProfileUtils.GetSettings();

            _userProfileContext = new UserProfileContext(_userProfileSettings, userProfileUtils);
            _userProfileContext.PropertyChanged += UserProfileContextOnPropertyChanged;

            DataContext = _userProfileContext;

            if (_ctx != null)
            {
                BindToUserProfileChanges();
            }
        }

        public bool OnOk()
        {
            if (ValidatePage())
            {
                _userProfileSettings.HasBeenAskedToFillProfile = true;
                _userProfileUtils.StoreSettings(_userProfileSettings);
                return true;
            }
            return false;
        }

        public bool ValidatePage()
        {
            return _userProfileContext.Error == null;
        }

        public EitherControl Control
        {
            get { return this; }
        }

        public string Id
        {
            get { return PID; }
        }

        private void OnResetSettings(object sender, RoutedEventArgs e)
        {
            var result = _messageBoxCreator.ShowYesNo(UserProfileOptionsMessages.SettingResetDialog);
            if (result)
            {
                var settingResetType = new SettingResetType {ResetType = ResetType};
                _actionExecutor.ExecuteActionGuarded<SettingsCleaner>(
                    settingResetType.GetDataContextForSettingResultType(_dataContexts, _lifetime));

                var window = Window.GetWindow(this);
                if (window != null)
                {
                    window.Close();
                }
            }
        }

        #region jetbrains smart-context bindings

        private void BindToUserProfileChanges()
        {
            // ProfileId
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => s.ProfileId,
                UserProfile.ProfileIdTextBox,
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
                UserProfile.ProjectsCoursesCheckBox,
                ToggleButton.IsCheckedProperty);
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.ProjectsPersonal,
                UserProfile.ProjectsPersonalCheckBox,
                ToggleButton.IsCheckedProperty);
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.ProjectsSharedSmall,
                UserProfile.ProjectsSharedSmallCheckBox,
                ToggleButton.IsCheckedProperty);
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.ProjectsSharedMedium,
                UserProfile.ProjectsSharedMediumCheckBox,
                ToggleButton.IsCheckedProperty);
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.ProjectsSharedLarge,
                UserProfile.ProjectsSharedLargeCheckBox,
                ToggleButton.IsCheckedProperty);
        }

        private void BindingForTeams()
        {
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.TeamsSolo,
                UserProfile.TeamsSoloCheckBox,
                ToggleButton.IsCheckedProperty);
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.TeamsSmall,
                UserProfile.TeamsSmallCheckBox,
                ToggleButton.IsCheckedProperty);
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.TeamsMedium,
                UserProfile.TeamsMediumCheckBox,
                ToggleButton.IsCheckedProperty);
            _ctx.SetBinding(
                _lifetime,
                (UserProfileSettings s) => (bool?) s.TeamsLarge,
                UserProfile.TeamsLargeCheckBox,
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

        private readonly IMessageBoxCreator _messageBoxCreator;
        private readonly IUserProfileSettingsUtils _userProfileUtils;

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