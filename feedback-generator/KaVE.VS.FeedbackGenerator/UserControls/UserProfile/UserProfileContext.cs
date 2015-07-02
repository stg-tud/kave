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

using System;
using System.Windows;
using KaVE.Commons.Model.Events;
using KaVE.VS.FeedbackGenerator.Settings;

namespace KaVE.VS.FeedbackGenerator.UserControls.UserProfile
{
    public class UserProfileContext
    {
        private ExportSettings ExportSettings { get; set; }
        private UserProfileSettings UserProfileSettings { get; set; }

        public UserProfileContext(ExportSettings exportSettings, UserProfileSettings userProfileSettings)
        {
            ExportSettings = exportSettings;
            UserProfileSettings = userProfileSettings;
        }

        public bool IsDatev
        {
            get { return ExportSettings.IsDatev; }
        }

        public Array ValuationOptions
        {
            get { return Enum.GetValues(typeof (WorkPosition)); }
        }

        public Array CategoryOptions
        {
            get { return Enum.GetValues(typeof (SelfEstimatedExperience)); }
        }

        public UserProfileSettings UserProfile { get; set; }

        /*   public Valuation Valuation
        {
            get { return UserSettings.Valuation; }
            set { UserSettings.Valuation = value; }
        }

        public Category Category
        {
            get { return UserSettings.Category; }
            set { UserSettings.Category = value; }
        }*/

        public string SelectedValuationOption
        {
            get { return ""; } // ( Valuation) GetValue(SelectedValuationOptionProperty); }
            set { }
            //SetValue(SelectedValuationOptionProperty, value); }
        }

        /*
        public Category SelectedCategoryOption
        {
            get { return (Category) GetValue(SelectedCategoryOptionProperty); }
            set { SetValue(SelectedCategoryOptionProperty, value); }
        }*/

        public bool ProvideUserInformation
        {
            get { return UserProfile.ProvideUserInformation; }
            set { UserProfile.ProvideUserInformation = value; }
        }

        public string Username
        {
            get { return UserProfile.Name; }
            set { UserProfile.Name = value; }
        }

        public string Mail
        {
            get { return UserProfile.Email; }
            set { UserProfile.Email = value; }
        }

        public string NumberText
        {
            get { return UserProfile.ExperienceYears.ToString(); }
            set { UserProfile.ExperienceYears = int.Parse(value); }
        }

        public WorkPosition Position
        {
            get { return UserProfile.Position; }
            set { UserProfile.Position = value; }
        }

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "SelectedPosition",
            typeof (WorkPosition),
            typeof (UserProfileContext));

        public void SetUserSettings() {}

        public bool ValidateEmail(string email)
        {
            return false;
        }
    }
}