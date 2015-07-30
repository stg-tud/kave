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

using System.Resources;
using System.Windows.Media;
using KaVE.VS.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.Properties;
using KaVE.VS.Achievements.Utils;

namespace KaVE.VS.Achievements.UI.PopUp
{
    /// <summary>
    ///     Interaction logic for PopUpGrid.xaml
    /// </summary>
    public partial class PopUpGrid
    {
        public PopUpGrid(BaseAchievement achievement, ResourceManager rm)
        {
            InitializeComponent();
            TitleString = rm.GetString("Title_" + achievement.Id);
            Title.Text = rm.GetString("Title_" + achievement.Id);
            Description.Text = rm.GetString("Description_" + achievement.Id);
            Date.Text = achievement.CompletionDate;
            Icon.Source = IconProvider.GetIconForAchievement(achievement);
            Background = new ImageBrush(IconProvider.GetImageSourceFromBitmap(icons.blau));
            CompletionDate = achievement.CompletionDateTime;
        }
    }
}