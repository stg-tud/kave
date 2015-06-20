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
 *    - Sven Amann
 */

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace KaVE.VS.FeedbackGenerator.SessionManager.Presentation
{
    public static class MultiSelection
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItems",
                typeof (IList),
                typeof (MultiSelection),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemsChanged));

        private static readonly DependencyProperty SelectedItemsBehaviorProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItemsBehavior",
                typeof (SelectedItemsBehavior),
                typeof (ListBox),
                null);

        public static IList GetSelectedItems(DependencyObject d)
        {
            return (IList) d.GetValue(SelectedItemsProperty);
        }

        public static void SetSelectedItems(DependencyObject d, IList value)
        {
            d.SetValue(SelectedItemsProperty, value);
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listBox = d as ListBox;
            if (listBox != null)
            {
                var modelSelection = e.NewValue as IList;
                var behavior = listBox.GetValue(SelectedItemsBehaviorProperty) as SelectedItemsBehavior;
                if (behavior == null)
                {
                    behavior = new SelectedItemsBehavior(listBox, modelSelection);
                    listBox.SetValue(SelectedItemsBehaviorProperty, behavior);
                }
                else
                {
                    behavior.SetModelSelection(modelSelection);
                }
            }
        }

        /// <summary>
        /// Implements synchronization between ListBox.SelectedItems and the bound model property.
        /// </summary>
        private class SelectedItemsBehavior
        {
            private readonly ListBox _listBox;
            private IList _modelSelection;

            public SelectedItemsBehavior(ListBox listBox, IList modelSelection)
            {
                _listBox = listBox;
                SetModelSelection(modelSelection);
            }

            public void SetModelSelection(IList modelSelection)
            {
                RemoveModelSelectionChangeHandler();
                _modelSelection = modelSelection ?? new List<object>();
                SyncFromModelToListBox();
                AddModelSelectionChangeHandler();
            }

            private void AddModelSelectionChangeHandler()
            {
                var collection = _modelSelection as INotifyCollectionChanged;
                if (collection != null)
                {
                    collection.CollectionChanged += OnModelSelectionChanged;
                }
            }

            private void RemoveModelSelectionChangeHandler()
            {
                var collection = _modelSelection as INotifyCollectionChanged;
                if (collection != null)
                {
                    collection.CollectionChanged -= OnModelSelectionChanged;
                }
            }

            private void OnModelSelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                SyncFromModelToListBox();
            }

            private void SyncFromModelToListBox()
            {
                RemoveListBoxSelectionHandler();
                _listBox.SelectedItems.Clear();
                foreach (var item in _modelSelection)
                {
                    _listBox.SelectedItems.Add(item);
                }
                AddListBoxSelectionHandler();
            }

            private void RemoveListBoxSelectionHandler()
            {
                _listBox.SelectionChanged -= OnListBoxSelectionChanged;
            }

            private void AddListBoxSelectionHandler()
            {
                _listBox.SelectionChanged += OnListBoxSelectionChanged;
            }

            private void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                RemoveModelSelectionChangeHandler();
                _modelSelection.Clear();
                foreach (var item in _listBox.SelectedItems)
                {
                    _modelSelection.Add(item);
                }
                SetSelectedItems(_listBox, _modelSelection);
                AddModelSelectionChangeHandler();
            }
        }
    }
}