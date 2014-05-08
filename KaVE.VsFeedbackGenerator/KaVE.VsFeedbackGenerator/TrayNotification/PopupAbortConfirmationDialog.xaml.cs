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

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    public partial class PopupAbortConfirmationDialog
    {
        public PopupAbortConfirmationDialog()
        {
            InitializeComponent();
        }

        public bool DontShowAgain
        {
            get { return DontShowAgainCheckbox.IsChecked == true; }
        }

        public bool Confirmed { get; private set; }

        private void OnClickConfirm(object sender, RoutedEventArgs e)
        {
            Confirmed = true;
            Close();
        }

        private void OnClickCancel(object sender, RoutedEventArgs e)
        {
            Confirmed = false;
            Close();
        }
    }
}