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
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;

namespace KaVE.VS.FeedbackGenerator.UserControls.UserProfile
{
    public class UserProfileContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ExportSettings _exportSettings;
        private readonly UserProfileSettings _userProfileSettings;

        public UserProfileContext(ExportSettings exportSettings, UserProfileSettings userProfileSettings)
        {
            _exportSettings = exportSettings;
            _userProfileSettings = userProfileSettings;
        }

        public bool IsDatev
        {
            get { return _exportSettings.IsDatev; }
        }

        public bool IsProvidingProfile
        {
            get { return _userProfileSettings.IsProvidingProfile && !_exportSettings.IsDatev; }
            set
            {
                _userProfileSettings.IsProvidingProfile = value;
                OnPropertyChanged("IsProvidingProfile");
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

        public bool ProjectsNoAnswer
        {
            get { return _userProfileSettings.ProjectsNoAnswer; }
            set
            {
                if (value)
                {
                    _userProfileSettings.ProjectsNoAnswer = true;
                    _userProfileSettings.ProjectsCourses = false;
                    _userProfileSettings.ProjectsPrivate = false;
                    _userProfileSettings.ProjectsTeamSmall = false;
                    _userProfileSettings.ProjectsTeamLarge = false;
                    _userProfileSettings.ProjectsCommercial = false;
                }
                OnPropertyChanged("ProjectsNoAnswer");
                OnPropertyChanged("ProjectsCourses");
                OnPropertyChanged("ProjectsPrivate");
                OnPropertyChanged("ProjectsTeamSmall");
                OnPropertyChanged("ProjectsTeamLarge");
                OnPropertyChanged("ProjectsCommercial");
            }
        }

        public bool ProjectsCourses
        {
            get { return _userProfileSettings.ProjectsCourses; }
            set
            {
                _userProfileSettings.ProjectsCourses = value;
                OnPropertyChanged("ProjectsCourses");
                if (value)
                {
                    _userProfileSettings.ProjectsNoAnswer = false;
                    OnPropertyChanged("ProjectsNoAnswer");
                }
            }
        }

        public bool ProjectsPrivate
        {
            get { return _userProfileSettings.ProjectsPrivate; }
            set
            {
                _userProfileSettings.ProjectsPrivate = value;
                OnPropertyChanged("ProjectsPrivate");
                if (value)
                {
                    _userProfileSettings.ProjectsNoAnswer = false;
                    OnPropertyChanged("ProjectsNoAnswer");
                }
            }
        }

        public bool ProjectsTeamSmall
        {
            get { return _userProfileSettings.ProjectsTeamSmall; }
            set
            {
                _userProfileSettings.ProjectsTeamSmall = value;
                OnPropertyChanged("ProjectsTeamSmall");
                if (value)
                {
                    _userProfileSettings.ProjectsNoAnswer = false;
                    OnPropertyChanged("ProjectsNoAnswer");
                }
            }
        }

        public bool ProjectsTeamLarge
        {
            get { return _userProfileSettings.ProjectsTeamLarge; }
            set
            {
                _userProfileSettings.ProjectsTeamLarge = value;
                OnPropertyChanged("ProjectsTeamLarge");
                if (value)
                {
                    _userProfileSettings.ProjectsNoAnswer = false;
                    OnPropertyChanged("ProjectsNoAnswer");
                }
            }
        }

        public bool ProjectsCommercial
        {
            get { return _userProfileSettings.ProjectsCommercial; }
            set
            {
                _userProfileSettings.ProjectsCommercial = value;
                OnPropertyChanged("ProjectsCommercial");
                if (value)
                {
                    _userProfileSettings.ProjectsNoAnswer = false;
                    OnPropertyChanged("ProjectsNoAnswer");
                }
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
    }
}