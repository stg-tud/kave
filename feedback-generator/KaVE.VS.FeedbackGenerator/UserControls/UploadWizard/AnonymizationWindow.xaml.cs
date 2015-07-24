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

using System.Windows;
using KaVE.RS.Commons.Settings;

namespace KaVE.VS.FeedbackGenerator.UserControls.UploadWizard
{
    public partial class AnonymizationWindow : Window
    {
        private readonly ISettingsStore _settingsStore;
        private readonly UploadWizardContext _uploadWizardContext;

        public AnonymizationWindow(UploadWizardContext uploadWizardContext, ISettingsStore settingsStore)
        {
            _settingsStore = settingsStore;
            _uploadWizardContext = uploadWizardContext;
            InitializeComponent();

            DataContext = uploadWizardContext;
        }

        private void On_Ok_Click(object sender, RoutedEventArgs e)
        {
            _settingsStore.SetSettings(_uploadWizardContext.AnonymizationSettings);
            Close();
        }

        private void On_Abort_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
