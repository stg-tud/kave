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
using System.Windows.Controls.Primitives;
using Hardcodet.Wpf.TaskbarNotification;
using JetBrains.Application;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.Properties;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    [ShellComponent]
    public class NotifyTrayIcon
    {
        public NotifyTrayIcon()
        {
            Invoke.OnSTA(InitTaskbarIcon);
        }

        private void InitTaskbarIcon()
        {
            NotifyIcon = new TaskbarIcon {Icon = Resources.Bulb, Visibility = Visibility.Hidden};
        }

        private TaskbarIcon NotifyIcon { get; set; }

        public virtual void ShowSoftBalloonPopup()
        {
            ShowBalloonPopup(() => new SoftBalloonPopup());
        }

        public virtual void ShowHardBalloonPopup()
        {
            ShowBalloonPopup(() => new HardBalloonPopup());
        }

        private void ShowBalloonPopup(Func<BalloonPopupBase> popupFactory)
        {
            Invoke.OnSTA(
                () => NotifyIcon.ShowCustomBalloon(
                    popupFactory(),
                    PopupAnimation.Slide,
                    null));
        }
    }
}