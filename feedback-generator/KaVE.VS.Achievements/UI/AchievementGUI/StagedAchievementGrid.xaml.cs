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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using KaVE.VS.Achievements.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.Properties;
using KaVE.VS.Achievements.Util;

namespace KaVE.VS.Achievements.UI.AchievementGUI
{
    /// <summary>
    ///     Interaction logic for StagedAchievementGrid.xaml
    /// </summary>
    public partial class StagedAchievementGrid
    {
        public StagedAchievementGrid(StagedAchievement achievement, ResourceManager rm)
        {
            InitializeComponent();
            var shownAchievement = achievement.HighestCompletedStageOrFirst;
            Title.Text = rm.GetString("Title_" + shownAchievement.Id);
            TitleString = Title.Text;
            Description.Text = rm.GetString("Description_" + shownAchievement.Id);
            Date.Text = shownAchievement.CompletionDate;
            Icon.Source = IconProvider.GetIconForAchievement(shownAchievement);
            Background = new ImageBrush(IconProvider.GetImageSourceFromBitmap(icons.blau));
            CompletionDate = shownAchievement.CompletionDateTime;

            var progressBarBinding = new Binding("CurrentProgress") {Source = achievement.LastStageAchievement};
            BindingOperations.SetBinding(Progression, RangeBase.ValueProperty, progressBarBinding);

            InitializeStageGrids(achievement, rm);
        }

        private void InitializeStageGrids(StagedAchievement achievement, ResourceManager rm)
        {
            for (var i = 0; i < achievement.Stages.Length; i++)
            {
                var tool = new ToolTip();
                var content = new ContentControl();
                tool.Content = content;

                var smallImage = new Image
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Height = 50,
                    Width = 50,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(100 + i*75, 5, 5, 15),
                    ToolTip = tool,
                    Source = IconProvider.GetIconForAchievement(achievement.Stages[i])
                };
                Children.Add(smallImage);

                content.Content = new ProgressAchievementGrid(achievement.Stages[i], rm);
            }
        }
    }
}