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
using System.Collections.Generic;
using System.Resources;
using System.Windows.Controls;
using KaVE.VS.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.Properties;
using KaVE.VS.Achievements.UI.AchievementGrids;
using KaVE.VS.Achievements.UI.Utils;
using KaVE.VS.Achievements.Utils;
using KaVE.VS.Statistics.LogCollector;

namespace KaVE.VS.Achievements.UI.MainWindow
{
    /// <summary>
    ///     Interaction logic for AchievementWindowControl.xaml
    /// </summary>
    public partial class AchievementWindowControl
    {
        private const int AmountOfLatestAchievementsShown = 5;
        private readonly IAchievementListing _listing;

        private readonly List<SortableStackPanel> _panels = new List<SortableStackPanel>();

        private readonly FixedSizePriorityQueue<BaseAchievement> _latestAchievements =
            new FixedSizePriorityQueue<BaseAchievement>(AmountOfLatestAchievementsShown, new AchievementComparer());

        private readonly ResourceManager _rm = AchievementText.ResourceManager;

        public AchievementWindowControl(IAchievementListing listing, IAchievementsUiDelegator uiDelegator)
        {
            InitializeComponent();

            InitializeUi();

            _listing = listing;

            uiDelegator.AchievementUserControl = this;

            SortBox.SelectionChanged += SortPanels;
            FilterBox.SelectionChanged += FilterPanels;

            BaseAchievement.CompletedEventHandler += ReloadAchievements;

            LoadAchievements();
        }

        public List<SortableStackPanel> Panels
        {
            get { return _panels; }
        }

        private void FilterPanels(object o, SelectionChangedEventArgs args)
        {
            LoadAchievements();
        }

        private void SortPanels(object o, EventArgs args)
        {
            foreach (var sortableStackPanel in _panels)
            {
                sortableStackPanel.SortBy(SortBox.SelectedItem.ToString());
            }
        }

        private void LoadAchievements()
        {
            ClearAchievementPanels();

            var achievements = _listing.GetAchievementDictionary();
            foreach (var achievement in achievements)
            {
                UpdateLatestAchievements(achievement.Value);

                AddAchievementToPanels(achievement);
            }

            RefreshLatestAchievementPanel();

            SortPanels(null, EventArgs.Empty);
        }

        private void RefreshLatestAchievementPanel()
        {
            foreach (var latestAchievement in _latestAchievements.Items)
            {
                // Add Latest Achievements with BaseAchievementGrid (no progressbar necessary)
                LatestPanel.Children.Add(new BaseAchievementGrid(latestAchievement, _rm));
            }
            LatestPanel.SortBy(UIText.AchievementWindow_SortBox_CompletionDate);
        }

        private void AddAchievementToPanels(KeyValuePair<int, BaseAchievement> achievement)
        {
            var s = categories.ResourceManager.GetString("Category_" + achievement.Key);

            if (s != "FeatOfStrength")
            {
                AddAchievementToPanel(achievement.Value, AllPanel);
            }

            switch (s)
            {
                case "Resharper":
                    AddAchievementToPanel(achievement.Value, ResharperPanel);
                    break;
                case "Project":
                    AddAchievementToPanel(achievement.Value, ProjectPanel);
                    break;
                case "Coding":
                    AddAchievementToPanel(achievement.Value, CodingPanel);
                    break;
                case "FeatOfStrength":
                    if ((achievement.Value).IsCompleted)
                    {
                        AddAchievementToPanel(achievement.Value, FeatOfStrengthPanel);
                    }
                    break;
            }
        }

        private void ClearAchievementPanels()
        {
            foreach (var panel in _panels)
            {
                panel.Children.Clear();
            }
            _latestAchievements.Clear();
        }

        private void AddAchievementToPanel(BaseAchievement achievement, Panel panel)
        {
            var stagedAchievement = achievement as StagedAchievement;
            if (stagedAchievement != null)
            {
                if (StagedCheckAddable(stagedAchievement))
                {
                    panel.Children.Add(new StagedAchievementGrid(stagedAchievement, _rm));
                }
                if (!achievement.IsCompleted && CheckAddable((stagedAchievement.CurrentStageAchievement)))
                {
                    var grid = new ProgressAchievementGrid(stagedAchievement.CurrentStageAchievement, _rm);
                    grid.Progression.ValueChanged += SortPanels;
                    panel.Children.Add(grid);
                    return;
                }
            }

            var progressAchievement = achievement as ProgressAchievement;
            if (progressAchievement != null)
            {
                if (CheckAddable(achievement))
                {
                    var grid = new ProgressAchievementGrid(progressAchievement, _rm);
                    grid.Progression.ValueChanged += SortPanels;
                    panel.Children.Add(grid);
                    return;
                }
            }

            if (CheckAddable(achievement))
            {
                panel.Children.Add(new BaseAchievementGrid(achievement, _rm));
            }
        }

        private bool CheckAddable(BaseAchievement achievement)
        {
            return FilterBox.SelectedItem.ToString() == UIText.AchievementWindow_FilterBox_All ||
                   FilterBox.SelectedItem.ToString() == UIText.AchievementWindow_FilterBox_Completed &&
                   achievement.IsCompleted ||
                   FilterBox.SelectedItem.ToString() == UIText.AchievementWindow_FilterBox_Uncompleted &&
                   !achievement.IsCompleted;
        }

        private bool StagedCheckAddable(StagedAchievement achievement)
        {
            return FilterBox.SelectedItem.ToString() == UIText.AchievementWindow_FilterBox_All ||
                   FilterBox.SelectedItem.ToString() == UIText.AchievementWindow_FilterBox_Completed &&
                   achievement.CurrentStage > 0 ||
                   FilterBox.SelectedItem.ToString() == UIText.AchievementWindow_FilterBox_Uncompleted &&
                   !achievement.FirstStageAchievement.IsCompleted;
        }

        public void ReloadAchievements(object o, EventArgs args)
        {
            // if LogReplay is running don't update Achievement UI because of possible memory overflow 
            // (garbage collector doesn't clean up fast enough when Achievement UI reloads that often)
            // Log Replay calls ReloadAchievements when all Statistic Updates were send
            if (!LogReplay.LogReplayRunning)
            {
                Dispatcher.Invoke((Action) LoadAchievements);
            }
        }

        private void UpdateLatestAchievements(BaseAchievement achievement)
        {
            var stagedAchievement = achievement as StagedAchievement;
            if (stagedAchievement != null)
            {
                foreach (var stage in stagedAchievement.Stages)
                {
                    UpdateLatestAchievements(stage);
                }
                return;
            }

            if (achievement.IsCompleted)
            {
                _latestAchievements.Enqueue(achievement);
            }
        }

        private void InitializeUi()
        {
            _panels.Add(AllPanel);
            _panels.Add(LatestPanel);
            _panels.Add(CodingPanel);
            _panels.Add(ResharperPanel);
            _panels.Add(ProjectPanel);
            _panels.Add(FeatOfStrengthPanel);

            SortBox.Items.Add(UIText.AchievementWindow_SortBox_Progression);
            SortBox.Items.Add(UIText.AchievementWindow_SortBox_CompletionDate);
            SortBox.Items.Add(UIText.AchievementWindow_SortBox_Title);
            SortBox.SelectedItem = UIText.AchievementWindow_SortBox_Progression;

            FilterBox.Items.Add(UIText.AchievementWindow_FilterBox_All);
            FilterBox.Items.Add(UIText.AchievementWindow_FilterBox_Completed);
            FilterBox.Items.Add(UIText.AchievementWindow_FilterBox_Uncompleted);
            FilterBox.SelectedItem = UIText.AchievementWindow_FilterBox_All;
        }
    }
}