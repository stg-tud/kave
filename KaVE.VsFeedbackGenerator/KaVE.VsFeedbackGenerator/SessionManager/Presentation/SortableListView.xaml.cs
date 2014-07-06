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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    public partial class SortableListView
    {
        private GridViewColumnHeader _lastHeaderClicked;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public void GridViewColumnHeaderClicked(GridViewColumnHeader clickedHeader)
        {
            if (clickedHeader == null)
            {
                return;
            }

            if (clickedHeader.Role == GridViewColumnHeaderRole.Padding)
            {
                return;
            }

            var direction = LastDirection(clickedHeader); 
            var sortString = ((Binding) clickedHeader.Column.DisplayMemberBinding).Path.Path;

            Sort(sortString, direction);
            ShowSortArrow(direction, clickedHeader);

            _lastHeaderClicked = clickedHeader;
            _lastDirection = direction;
        }

        private ListSortDirection LastDirection(GridViewColumnHeader headerClicked)
        {
            ListSortDirection direction;
            if (!Equals(headerClicked, _lastHeaderClicked))
            {
                direction = ListSortDirection.Ascending;
            }
            else
            {
                direction = _lastDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
            return direction;
        }

        private void ShowSortArrow(ListSortDirection direction, GridViewColumnHeader headerClicked)
        {
            var gridView = (GridView) View;
            if (direction == ListSortDirection.Ascending)
            {
                gridView.ColumnHeaderTemplate = FindResource("HeaderTemplateArrowUp") as DataTemplate;
            }
            else
            {
                gridView.ColumnHeaderTemplate = FindResource("HeaderTemplateArrowDown") as DataTemplate;
            }

            // Remove arrow from previously sorted header
            if (_lastHeaderClicked != null && !Equals(_lastHeaderClicked, headerClicked))
            {
                _lastHeaderClicked.Column.HeaderTemplate = null;
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            var dataView = CollectionViewSource.GetDefaultView(ItemsSource ?? Items);

            dataView.SortDescriptions.Clear();
            var sort = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sort);
            dataView.Refresh();
        }
    }
}