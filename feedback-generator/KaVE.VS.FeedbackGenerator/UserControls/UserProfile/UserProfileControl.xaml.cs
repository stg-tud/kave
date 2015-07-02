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

namespace KaVE.VS.FeedbackGenerator.UserControls.UserProfile
{
    public partial class UserProfileControl
    {
        private UserProfileContext MyDataContext
        {
            get { return (UserProfileContext) DataContext; }
        }

        public UserProfileControl()
        {
            InitializeComponent();

            DataContextChanged += (sender, e) =>
            {
                //  UserName = MyDataContext.UserName;
                // Email = MyDataContext.Email;
            };
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var numberRegex = new Regex("[^0-9]+");
            e.Handled = numberRegex.IsMatch(e.Text);
        }
    }
}