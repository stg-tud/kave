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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using KaVE.VS.Achievements.Properties;

namespace KaVE.VS.Achievements.UI.StatisticUI
{
    /// <summary>
    ///     Interaction logic for StatisticWindowControl.xaml
    /// </summary>
    public partial class StatisticWindowControl
    {
        private readonly StatisticViewModel _statisticViewModel;

        public StatisticWindowControl(IUiDelegator uiDelegator, StatisticViewModel statisticViewModel)
        {
            InitializeComponent();
            uiDelegator.StatisticUserControl = this;

            _statisticViewModel = statisticViewModel;

            statisticViewModel.ResetCollections();
            InitializeAndDisplayCollections(_statisticViewModel.CollectionDictionary);
        }

        public void InitializeAndDisplayCollections(
            Dictionary<Type, ObservableCollection<StatisticElement>> collectionList)
        {
            var rm = UIText.ResourceManager;

            foreach (var keyValuePair in collectionList)
            {
                var tempGridView = new GridView();

                tempGridView.Columns.Add(
                    new GridViewColumn
                    {
                        Header = rm.GetString("Name"),
                        DisplayMemberBinding = new Binding
                        {
                            Path = new PropertyPath("Name"),
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        }
                    });

                tempGridView.Columns.Add(
                    new GridViewColumn
                    {
                        Header = rm.GetString("Value"),
                        DisplayMemberBinding = new Binding
                        {
                            Path = new PropertyPath("Value"),
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        },
                        Width = 100
                    });

                var tempListView = new SortableListView
                {
                    ItemsSource = keyValuePair.Value,
                    View = tempGridView
                };

                var tempTabItem = new TabItem
                {
                    Header = rm.GetString(keyValuePair.Key.Name),
                    Content = tempListView
                };

                UiTabControl.Items.Add(tempTabItem);
            }
        }

        public void ResetView()
        {
            UiTabControl.Items.Clear();
            InitializeAndDisplayCollections(_statisticViewModel.CollectionDictionary);
        }
    }
}