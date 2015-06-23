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

using System.Text.RegularExpressions;
using System.Windows.Input;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Settings;

namespace KaVE.VS.FeedbackGenerator.UserControls.UserProfile
{
    /// <summary>
    ///     Interaction logic for UserSettingsGrid.xaml
    /// </summary>
    public partial class UserProfileGrid
    {
        public UserProfileGrid()
        {
            InitializeComponent();
            DataContext = new UserProfileViewModel(Registry.GetComponent<ISettingsStore>());
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var numberRegex = new Regex("[^0-9]+");
            e.Handled = numberRegex.IsMatch(e.Text);
        }
    }
}