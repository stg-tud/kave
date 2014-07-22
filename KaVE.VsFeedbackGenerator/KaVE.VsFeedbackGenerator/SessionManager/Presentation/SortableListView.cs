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

using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    public class SortableListView : ListView
    {
        private GridViewColumnHeader _lastClickedHeader;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

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

        public SortableListView()
        {
            AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnGridViewColumnHeaderClicked));
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
            var bindingName = GetColumnBindingPathName(clickedHeader);
            var direction = LastDirection(clickedHeader);

            SortBy(bindingName, direction);
            ShowSortArrow(clickedHeader, direction);

            _lastClickedHeader = clickedHeader;
            _lastDirection = direction;
        }

        private static string GetColumnBindingPathName(GridViewColumnHeader header)
        {
            return ((Binding) header.Column.DisplayMemberBinding).Path.Path;
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

        private void SortBy(string bindingName, ListSortDirection direction)
        {
            var dataView = CollectionViewSource.GetDefaultView(ItemsSource ?? Items);
            dataView.SortDescriptions.Clear();

            var firstNotPrimaryColumnBindingName =
                ((GridView) View).Columns.Select(c => ((Binding)c.DisplayMemberBinding).Path.Path)
                                      .FirstOrDefault(path => !Equals(path, bindingName));

            dataView.SortDescriptions.Add(new SortDescription(bindingName, direction));
            dataView.SortDescriptions.Add(new SortDescription(firstNotPrimaryColumnBindingName, direction));
            dataView.Refresh();
        }
    }
}