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
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using JetBrains.Application;
using KaVE.VS.Achievements.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.Properties;

namespace KaVE.VS.Achievements.UI.AchievementGUI
{
    [ShellComponent]
    public class PopUpControl
    {
        private const double PopUpDisplayTime = 2;

        private const double FadeoutDelay = PopUpDisplayTime/3;

        private const double TicksInterval = 1.0/60.0;

        private readonly ResourceManager _rm = AchievementText.ResourceManager;

        private double _actualTicks;

        private Popup _popUp;

        private DispatcherTimer _timer;

        public PopUpControl()
        {
            BaseAchievement.CompletedEventHandler += ShowPopUpOnBaseAchievementCompleted;
            SetupTimer();

            CreateNewPopupInstance();
        }

        private void SetupTimer()
        {
            _timer = new DispatcherTimer(DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromSeconds(TicksInterval),
                IsEnabled = false
            };

            _timer.Tick += delegate { UpdatePopUp(); };
        }

        private void ShowPopUpOnBaseAchievementCompleted(object achievement, EventArgs eventArgs)
        {
            var baseAchievement = achievement as BaseAchievement;
            if (baseAchievement == null)
            {
                return;
            }
            Application.Current.Dispatcher.BeginInvoke((Action) (() => ShowPopUp(new PopUpGrid(baseAchievement, _rm))));
        }

        private void CreateNewPopupInstance()
        {
            _popUp = new Popup
            {
                Placement = PlacementMode.Center,
                AllowsTransparency = true,
                PopupAnimation = PopupAnimation.Fade,
                PlacementTarget = Application.Current.MainWindow,
                HorizontalOffset = Application.Current.MainWindow.ActualWidth/2 - 320,
                VerticalOffset = Application.Current.MainWindow.ActualHeight/2 - 110
            };
        }

        public void ShowPopUp(Grid grid)
        {
            if (_popUp.IsOpen)
            {
                _popUp.IsOpen = false;
                CreateNewPopupInstance();
            }

            grid.MouseDown += ClosePopup;

            _popUp.Child = grid;
            _popUp.IsOpen = true;

            _actualTicks = 0.0;
            _timer.IsEnabled = true;
        }

        private void ClosePopup(object sender = null, MouseButtonEventArgs e = null)
        {
            _popUp.IsOpen = false;
        }

        private void UpdatePopUp()
        {
            if (_actualTicks >= PopUpDisplayTime)
            {
                _timer.IsEnabled = false;
                ClosePopup();
            }
            else
            {
                _actualTicks += TicksInterval;
                CalculateOpacity();
            }
        }

        private void CalculateOpacity()
        {
            if (_actualTicks >= FadeoutDelay)
            {
                _popUp.Child.Opacity = 1 - ((_actualTicks - FadeoutDelay)/(PopUpDisplayTime - FadeoutDelay));
            }
        }
    }
}