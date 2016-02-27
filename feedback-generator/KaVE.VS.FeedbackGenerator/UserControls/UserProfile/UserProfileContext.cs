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
using System.Collections;
using System.ComponentModel;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.Settings;

namespace KaVE.VS.FeedbackGenerator.UserControls.UserProfile
{
    public class UserProfileContext : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly UserProfileSettings _userProfileSettings;
        private readonly IRandomizationUtils _rnd;

        public UserProfileContext(UserProfileSettings userProfileSettings,
            IRandomizationUtils rnd)
        {
            _userProfileSettings = userProfileSettings;
            _rnd = rnd;

            if ("".Equals(_userProfileSettings.ProfileId))
            {
                _userProfileSettings.ProfileId = _rnd.GetRandomGuid().ToString();
            }
        }

        public string ProfileId
        {
            get { return _userProfileSettings.ProfileId; }
            set
            {
                _userProfileSettings.ProfileId = value;
                OnPropertyChanged("ProfileId");
            }
        }

        public void GenerateNewProfileId()
        {
            ProfileId = _rnd.GetRandomGuid().ToString();
        }

        public Educations Education
        {
            get { return _userProfileSettings.Education; }
            set
            {
                _userProfileSettings.Education = value;
                OnPropertyChanged("Education");
            }
        }

        public Positions Position
        {
            get { return _userProfileSettings.Position; }
            set
            {
                _userProfileSettings.Position = value;
                OnPropertyChanged("Position");
            }
        }

        public bool ProjectsCourses
        {
            get { return _userProfileSettings.ProjectsCourses; }
            set
            {
                _userProfileSettings.ProjectsCourses = value;
                OnPropertyChanged("ProjectsCourses");
            }
        }

        public bool ProjectsPersonal
        {
            get { return _userProfileSettings.ProjectsPersonal; }
            set
            {
                _userProfileSettings.ProjectsPersonal = value;
                OnPropertyChanged("ProjectsPersonal");
            }
        }

        public bool ProjectsSharedSmall
        {
            get { return _userProfileSettings.ProjectsSharedSmall; }
            set
            {
                _userProfileSettings.ProjectsSharedSmall = value;
                OnPropertyChanged("ProjectsSharedSmall");
            }
        }

        public bool ProjectsSharedMedium
        {
            get { return _userProfileSettings.ProjectsSharedMedium; }
            set
            {
                _userProfileSettings.ProjectsSharedMedium = value;
                OnPropertyChanged("ProjectsSharedMedium");
            }
        }

        public bool ProjectsSharedLarge
        {
            get { return _userProfileSettings.ProjectsSharedLarge; }
            set
            {
                _userProfileSettings.ProjectsSharedLarge = value;
                OnPropertyChanged("ProjectsSharedLarge");
            }
        }

        public bool TeamsSolo
        {
            get { return _userProfileSettings.TeamsSolo; }
            set
            {
                _userProfileSettings.TeamsSolo = value;
                OnPropertyChanged("TeamsSolo");
            }
        }

        public bool TeamsSmall
        {
            get { return _userProfileSettings.TeamsSmall; }
            set
            {
                _userProfileSettings.TeamsSmall = value;
                OnPropertyChanged("TeamsSmall");
            }
        }

        public bool TeamsMedium
        {
            get { return _userProfileSettings.TeamsMedium; }
            set
            {
                _userProfileSettings.TeamsMedium = value;
                OnPropertyChanged("TeamsMedium");
            }
        }

        public bool TeamsLarge
        {
            get { return _userProfileSettings.TeamsLarge; }
            set
            {
                _userProfileSettings.TeamsLarge = value;
                OnPropertyChanged("TeamsLarge");
            }
        }

        public YesNoUnknown CodeReviews
        {
            get { return _userProfileSettings.CodeReviews; }
            set
            {
                _userProfileSettings.CodeReviews = value;
                OnPropertyChanged("CodeReviews");
            }
        }

        public Likert7Point ProgrammingGeneral
        {
            get { return _userProfileSettings.ProgrammingGeneral; }
            set
            {
                _userProfileSettings.ProgrammingGeneral = value;
                OnPropertyChanged("ProgrammingGeneral");
            }
        }

        public Likert7Point ProgrammingCSharp
        {
            get { return _userProfileSettings.ProgrammingCSharp; }
            set
            {
                _userProfileSettings.ProgrammingCSharp = value;
                OnPropertyChanged("ProgrammingCSharp");
            }
        }

        public IEnumerable EducationOptions
        {
            get { return Enum.GetValues(typeof (Educations)); }
        }

        public IEnumerable PositionOptions
        {
            get { return Enum.GetValues(typeof (Positions)); }
        }

        public IEnumerable YesNoOptions
        {
            get { return Enum.GetValues(typeof (YesNoUnknown)); }
        }

        public IEnumerable LikertOptions
        {
            get { return Enum.GetValues(typeof (Likert7Point)); }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string this[string prop]
        {
            get
            {
                return "ProfileId".Equals(prop)
                    ? ValidateProfileId()
                    : null;
            }
        }

        private string ValidateProfileId()
        {
            const int minChars = 5;
            if (ProfileId == null || ProfileId != null && ProfileId.Length < minChars)
            {
                return string.Format("profile id is too short (<{0} chars)", minChars);
            }

            return null;
        }

        public string Error
        {
            get { return ValidateProfileId(); }
        }
    }
}