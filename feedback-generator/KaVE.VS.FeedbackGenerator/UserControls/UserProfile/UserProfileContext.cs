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
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;

namespace KaVE.VS.FeedbackGenerator.UserControls.UserProfile
{
    public class UserProfileContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ExportSettings _exportSettings;
        private readonly UserProfileSettings _userProfileSettings;
        private readonly IRandomizationUtils _rnd;

        public UserProfileContext(ExportSettings exportSettings,
            UserProfileSettings userProfileSettings,
            IRandomizationUtils rnd)
        {
            _exportSettings = exportSettings;
            _userProfileSettings = userProfileSettings;
            _rnd = rnd;
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
                if (!_userProfileSettings.HasBeenAskedtoProvideProfile)
                {
                    _userProfileSettings.ProfileId = _rnd.GetRandomGuid().ToString();
                    _userProfileSettings.HasBeenAskedtoProvideProfile = true;
                    OnPropertyChanged("ProfileId");
                }
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

        public bool ProjectsNoAnswer
        {
            get { return _userProfileSettings.ProjectsNoAnswer; }
            set
            {
                if (value)
                {
                    _userProfileSettings.ProjectsNoAnswer = true;
                    _userProfileSettings.ProjectsCourses = false;
                    _userProfileSettings.ProjectsPersonal = false;
                    _userProfileSettings.ProjectsSharedSmall = false;
                    _userProfileSettings.ProjectsSharedLarge = false;
                }
                OnPropertyChanged("ProjectsNoAnswer");
                OnPropertyChanged("ProjectsCourses");
                OnPropertyChanged("ProjectsPersonal");
                OnPropertyChanged("ProjectsSharedSmall");
                OnPropertyChanged("ProjectsSharedLarge");
            }
        }

        private void CheckProjectsNoAnswer()
        {
            var s = _userProfileSettings;
            var isSomeThingChecked = s.ProjectsCourses || s.ProjectsPersonal || s.ProjectsSharedSmall ||
                                     s.ProjectsSharedLarge;
            if (!isSomeThingChecked)
            {
                _userProfileSettings.ProjectsNoAnswer = true;
                OnPropertyChanged("ProjectsNoAnswer");
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
                else
                {
                    CheckProjectsNoAnswer();
                }
            }
        }

        public bool ProjectsPersonal
        {
            get { return _userProfileSettings.ProjectsPersonal; }
            set
            {
                _userProfileSettings.ProjectsPersonal = value;
                OnPropertyChanged("ProjectsPersonal");
                if (value)
                {
                    _userProfileSettings.ProjectsNoAnswer = false;
                    OnPropertyChanged("ProjectsNoAnswer");
                }
                else
                {
                    CheckProjectsNoAnswer();
                }
            }
        }

        public bool ProjectsSharedSmall
        {
            get { return _userProfileSettings.ProjectsSharedSmall; }
            set
            {
                _userProfileSettings.ProjectsSharedSmall = value;
                OnPropertyChanged("ProjectsSharedSmall");
                if (value)
                {
                    _userProfileSettings.ProjectsNoAnswer = false;
                    OnPropertyChanged("ProjectsNoAnswer");
                }
                else
                {
                    CheckProjectsNoAnswer();
                }
            }
        }

        public bool ProjectsSharedLarge
        {
            get { return _userProfileSettings.ProjectsSharedLarge; }
            set
            {
                _userProfileSettings.ProjectsSharedLarge = value;
                OnPropertyChanged("ProjectsSharedLarge");
                if (value)
                {
                    _userProfileSettings.ProjectsNoAnswer = false;
                    OnPropertyChanged("ProjectsNoAnswer");
                }
                else
                {
                    CheckProjectsNoAnswer();
                }
            }
        }

        public bool TeamsNoAnswer
        {
            get { return _userProfileSettings.TeamsNoAnswer; }
            set
            {
                if (value)
                {
                    _userProfileSettings.TeamsNoAnswer = true;
                    _userProfileSettings.TeamsSolo = false;
                    _userProfileSettings.TeamsSmall = false;
                    _userProfileSettings.TeamsMedium = false;
                    _userProfileSettings.TeamsLarge = false;
                }
                OnPropertyChanged("TeamsNoAnswer");
                OnPropertyChanged("TeamsSolo");
                OnPropertyChanged("TeamsSmall");
                OnPropertyChanged("TeamsMedium");
                OnPropertyChanged("TeamsLarge");
            }
        }

        private void CheckTeamNoAnswer()
        {
            var s = _userProfileSettings;
            var isSomeThingChecked = s.TeamsSolo || s.TeamsSmall || s.TeamsMedium ||
                                     s.TeamsLarge;
            if (!isSomeThingChecked)
            {
                _userProfileSettings.TeamsNoAnswer = true;
                OnPropertyChanged("TeamsNoAnswer");
            }
        }

        public bool TeamsSolo
        {
            get { return _userProfileSettings.TeamsSolo; }
            set
            {
                _userProfileSettings.TeamsSolo = value;
                OnPropertyChanged("TeamsSolo");
                if (value)
                {
                    _userProfileSettings.TeamsNoAnswer = false;
                    OnPropertyChanged("TeamsNoAnswer");
                }
                else
                {
                    CheckTeamNoAnswer();
                }
            }
        }

        public bool TeamsSmall
        {
            get { return _userProfileSettings.TeamsSmall; }
            set
            {
                _userProfileSettings.TeamsSmall = value;
                OnPropertyChanged("TeamsSmall");
                if (value)
                {
                    _userProfileSettings.TeamsNoAnswer = false;
                    OnPropertyChanged("TeamsNoAnswer");
                }
                else
                {
                    CheckTeamNoAnswer();
                }
            }
        }

        public bool TeamsMedium
        {
            get { return _userProfileSettings.TeamsMedium; }
            set
            {
                _userProfileSettings.TeamsMedium = value;
                OnPropertyChanged("TeamsMedium");
                if (value)
                {
                    _userProfileSettings.TeamsNoAnswer = false;
                    OnPropertyChanged("TeamsNoAnswer");
                }
                else
                {
                    CheckTeamNoAnswer();
                }
            }
        }

        public bool TeamsLarge
        {
            get { return _userProfileSettings.TeamsLarge; }
            set
            {
                _userProfileSettings.TeamsLarge = value;
                OnPropertyChanged("TeamsLarge");
                if (value)
                {
                    _userProfileSettings.TeamsNoAnswer = false;
                    OnPropertyChanged("TeamsNoAnswer");
                }
                else
                {
                    CheckTeamNoAnswer();
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