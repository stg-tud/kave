﻿/*
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

using System;
using System.Globalization;
using System.Windows;
using KaVE.Commons.Model.Events;
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.Interactivity;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.UserControls.ValidationRules;

namespace KaVE.VS.FeedbackGenerator.UserControls.UserProfile
{
    public class UserProfileViewModel : DependencyObject
    {
        private readonly InteractionRequest<Notification> _errorNotificationRequest;

        public IInteractionRequest<Notification> ErrorNotificationRequest
        {
            get { return _errorNotificationRequest; }
        }

        private readonly MailValidationRule _mailValidationRule;

        private readonly ISettingsStore _settingsStore;

        public UserProfileViewModel(ISettingsStore settingsStore)
        {
            _settingsStore = settingsStore;
            var exportSettings = settingsStore.GetSettings<ExportSettings>();
            _errorNotificationRequest = new InteractionRequest<Notification>();

            _mailValidationRule = new MailValidationRule();

            if (exportSettings.IsDatev)
            {
                _settingsStore.UpdateSettings<UserProfileSettings>(ss => ss.ProvideUserInformation = false);
            }
        }

        public Array ValuationOptions
        {
            get { return Enum.GetValues(typeof (WorkPosition)); }
        }

        public Array CategoryOptions
        {
            get { return Enum.GetValues(typeof (SelfEstimatedExperience)); }
        }

        public UserProfileSettings UserSettings { get; set; }

        /*   public Valuation Valuation
        {
            get { return UserSettings.Valuation; }
            set { UserSettings.Valuation = value; }
        }

        public Category Category
        {
            get { return UserSettings.Category; }
            set { UserSettings.Category = value; }
        }

        public Valuation SelectedValuationOption
        {
            get { return (Valuation) GetValue(SelectedValuationOptionProperty); }
            set { SetValue(SelectedValuationOptionProperty, value); }
        }

        public Category SelectedCategoryOption
        {
            get { return (Category) GetValue(SelectedCategoryOptionProperty); }
            set { SetValue(SelectedCategoryOptionProperty, value); }
        }*/

        public bool ProvideUserInformation
        {
            get { return UserSettings.ProvideUserInformation; }
            set { UserSettings.ProvideUserInformation = value; }
        }

        public string Username
        {
            get { return UserSettings.Name; }
            set { UserSettings.Name = value; }
        }

        public string Mail
        {
            get { return UserSettings.Email; }
            set { UserSettings.Email = value; }
        }

        public string NumberText
        {
            get { return UserSettings.ExperienceYears; }
            set { UserSettings.ExperienceYears = value; }
        }

        public void SetUserSettings()
        {
            _settingsStore.SetSettings(UserSettings);
        }

        public bool ValidateEmail(string email)
        {
            var validationResult = _mailValidationRule.Validate(email, CultureInfo.CurrentUICulture);
            if (!validationResult.IsValid)
            {
                ShowInformationInvalidMessage();
            }
            return validationResult.IsValid;
        }

        private void ShowInformationInvalidMessage()
        {
            _errorNotificationRequest.Raise(
                new Notification
                {
                    Caption = Properties.UserProfileMessages.EmailValidationErrorTitle,
                    Message = Properties.UserProfileMessages.EmailValidationErrorMessage
                });
        }

        public static readonly DependencyProperty SelectedValuationOptionProperty = DependencyProperty.Register(
            "SelectedValuationOption",
            typeof (WorkPosition),
            typeof (UserProfileViewModel));

        public static readonly DependencyProperty SelectedCategoryOptionProperty = DependencyProperty.Register(
            "SelectedCategoryOption",
            typeof (SelfEstimatedExperience),
            typeof (UserProfileViewModel));
    }
}