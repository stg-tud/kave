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
using System.Linq;
using System.Windows.Controls;
using KaVE.VS.Achievements.Properties;
using KaVE.VS.Achievements.UI.AchievementGrids;

namespace KaVE.VS.Achievements.UI.Utils
{
    public class SortableStackPanel : StackPanel
    {
        private IEnumerable<AchievementGrid> Sort(
            Func<IEnumerable<AchievementGrid>, IEnumerable<AchievementGrid>> sorter)
        {
            return sorter(Children.Cast<AchievementGrid>()).ToList();
        }

        private void RefreshUiElementCollection(IEnumerable<AchievementGrid> sortedList)
        {
            Children.Clear();
            foreach (var achievementGrid in sortedList)
            {
                Children.Add(achievementGrid);
            }
        }

        public void SortBy(string criteria)
        {
            IEnumerable<AchievementGrid> sortedList = null;

            if (criteria.Equals(UIText.AchievementWindow_SortBox_CompletionDate))
            {
                sortedList = Sort(a => a.OrderByDescending(grid => grid.CompletionDate));
            }
            else if (criteria.Equals(UIText.AchievementWindow_SortBox_Progression))
            {
                sortedList = Sort(a => a.OrderByDescending(grid => grid.Progression.Value));
            }
            else if (criteria.Equals(UIText.AchievementWindow_SortBox_Title))
            {
                sortedList = Sort(a => a.OrderBy(grid => grid.TitleString));
            }

            if (sortedList != null)
            {
                RefreshUiElementCollection(sortedList);
            }
        }
    }
}