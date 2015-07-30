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
using System.Windows.Threading;
using JetBrains.Application;
using KaVE.VS.Achievements.UI.MainWindow;

namespace KaVE.VS.Achievements.UI.Utils
{
    public interface IAchievementsUiDelegator
    {
        AchievementWindowControl AchievementUserControl { get; set; }

        void DelegateToAchievementUi(Action action);
    }

    [ShellComponent]
    public class AchievementsUiDelegator : IAchievementsUiDelegator
    {
        public AchievementWindowControl AchievementUserControl { get; set; }

        public void DelegateToAchievementUi(Action action)
        {
            if (AchievementUserControl != null)
            {
                AchievementUserControl.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
            }
        }
    }
}