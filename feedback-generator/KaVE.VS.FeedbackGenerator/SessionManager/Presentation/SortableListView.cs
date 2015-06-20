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
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using JetBrains.Util;
using KaVE.Commons.Utils.Reflection;

namespace KaVE.VS.FeedbackGenerator.SessionManager.Presentation
{
    public class SortableListView : ListView
    {
        private static readonly string ItemsSourcePropertyName =
            TypeExtensions<ListView>.GetPropertyName(lv => lv.ItemsSource);

        private const string ArrowUpPathData = "M 5,10 L 15,10 L 10,5 L 5,10";
        private const string ArrowDownPathData = "M 5,5 L 10,10 L 15,5 L 5,5";

        private const string ArrowHeaderDataTemplateRaw =
            "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">" +
            "<DockPanel LastChildFill=\"True\" Width=\"{{Binding ActualWidth, RelativeSource={{RelativeSource FindAncestor, AncestorType={{x:Type GridViewColumnHeader}}}}}}\">" +
            "    <Path StrokeThickness=\"1\" Fill=\"Gray\" Data=\"{0}\" DockPanel.Dock=\"Right\" Width=\"20\" HorizontalAlignment=\"Right\" Margin=\"5,0,5,0\" SnapsToDevicePixels=\"True\"/>" +
            "    <TextBlock Text=\"{{Binding }}\" TextAlignment=\"Center\" />" +
            "</DockPanel>" +
            "</DataTemplate>";

        private static readonly DataTemplate ArrowUpHeaderTemplate =
            (DataTemplate) XamlReader.Parse(string.Format(ArrowHeaderDataTemplateRaw, ArrowUpPathData));

        private static readonly DataTemplate ArrowDownHeaderTemplate =
            (DataTemplate) XamlReader.Parse(string.Format(ArrowHeaderDataTemplateRaw, ArrowDownPathData));

        private GridViewColumnHeader _lastClickedHeader;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;
        private IList<SortDescription> _currentSortDescriptions;

        public SortableListView()
        {
            AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnGridViewColumnHeaderClicked));
            TypeDescriptor.GetProperties(this)[ItemsSourcePropertyName].AddValueChanged(
                this,
                RestoreSortDescriptionsOnItemSourceChange);
        }

        private ICollectionView DataView
        {
            get { return CollectionViewSource.GetDefaultView(ItemsSource ?? Items); }
        }

        private void RestoreSortDescriptionsOnItemSourceChange(object sender, EventArgs eventArgs)
        {
            if (_currentSortDescriptions == null)
            {
                return;
            }
            SetSortDescription(_currentSortDescriptions);
        }

        private void OnGridViewColumnHeaderClicked(object sender, RoutedEventArgs e)
        {
            var clickedHeader = e.OriginalSource as GridViewColumnHeader;

            if (clickedHeader != null && clickedHeader.Role != GridViewColumnHeaderRole.Padding)
            {
                SortByClickedHeader(clickedHeader);
            }
        }

        private void SortByClickedHeader(GridViewColumnHeader clickedHeader)
        {
            var direction = LastDirection(clickedHeader);

            SortBy(clickedHeader.Column, direction);
            ShowSortArrow(clickedHeader, direction);

            _lastClickedHeader = clickedHeader;
            _lastDirection = direction;
        }

        private ListSortDirection LastDirection(GridViewColumnHeader clickedHeader)
        {
            if (!clickedHeader.Equals(_lastClickedHeader))
            {
                return ListSortDirection.Ascending;
            }
            return _lastDirection == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;
        }

        private void SortBy(GridViewColumn column, ListSortDirection direction)
        {
            var primaryCriteria = GetBindingName(column);

            var secondaryColumn = ((GridView) View).Columns.FirstOrDefault(col => !col.Equals(column));
            var secondaryCriteria = secondaryColumn != null ? GetBindingName(secondaryColumn) : null;

            SortBy(primaryCriteria, secondaryCriteria, direction);
        }

        private static string GetBindingName(GridViewColumn column)
        {
            return ((Binding) column.DisplayMemberBinding).Path.Path;
        }

        private void SortBy(string primaryCriteria, string secondaryCriteria, ListSortDirection direction)
        {
            _currentSortDescriptions = new[]
            {
                new SortDescription(primaryCriteria, direction),
                new SortDescription(secondaryCriteria, direction)
            };

            SetSortDescription(_currentSortDescriptions);
            DataView.Refresh();
        }

        private void SetSortDescription(IEnumerable<SortDescription> sortDescriptions)
        {
            DataView.SortDescriptions.Clear();
            DataView.SortDescriptions.AddRange(sortDescriptions);
        }

        private void ShowSortArrow(GridViewColumnHeader clickedHeader, ListSortDirection direction)
        {
            clickedHeader.Column.HeaderTemplate = direction == ListSortDirection.Ascending
                ? ArrowUpHeaderTemplate
                : ArrowDownHeaderTemplate;

            // Remove arrow from previously sorted header
            if (_lastClickedHeader != null && !clickedHeader.Equals(_lastClickedHeader))
            {
                _lastClickedHeader.Column.HeaderTemplate = null;
            }
        }
    }
}