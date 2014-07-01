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

using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    public partial class SortableListView: ListView
    {
        private GridViewColumnHeader lastHeaderClicked = null;
        private ListSortDirection lastDirection = ListSortDirection.Ascending;

        public void GridViewColumnHeaderClicked(GridViewColumnHeader clickedHeader)
        {
            ListSortDirection direction;

            if (clickedHeader != null)
            {
                if (clickedHeader.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (clickedHeader != lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string sortString = ((Binding)clickedHeader.Column.DisplayMemberBinding).Path.Path;

                    Sort(sortString, direction);

                    lastHeaderClicked = clickedHeader;
                    lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            var dataView = CollectionViewSource.GetDefaultView(this.ItemsSource ?? this.Items);

            dataView.SortDescriptions.Clear();
            var sD = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sD);
            dataView.Refresh();
        }
    }
}
