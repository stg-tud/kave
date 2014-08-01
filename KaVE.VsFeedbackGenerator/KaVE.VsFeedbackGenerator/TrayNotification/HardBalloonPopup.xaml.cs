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
 * 
 * Contributors:
 *    - Uli Fahrer
 */

using System.Windows;
using System.Windows.Input;
using JetBrains;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.Utils.Logging;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    /// <summary>
    /// Interaction logic for HardBalloonPopup.xaml
    /// </summary>
    public partial class HardBalloonPopup
    {
        public HardBalloonPopup(ILogManager<IDEEvent> logManager)
        {
            InitializeComponent();
            SetPopupMessage(logManager);
        }

        private void SetPopupMessage(ILogManager<IDEEvent> logManager)
        {
            var size = logManager.GetTotalLogsSizeInMB();
            Message.Text = Properties.PopupNotification.InformationHardpopup.FormatEx(size);
        }

        private void Wizard_Button_OnClick(object sender, RoutedEventArgs e)
        {
            OpenUploadWizard();
        }

        private void ImgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClosePopup();
        }
    }
}




   
