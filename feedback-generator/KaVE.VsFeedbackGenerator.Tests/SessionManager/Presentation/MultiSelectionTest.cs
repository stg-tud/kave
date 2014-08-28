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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using KaVE.Utils.Reflection;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Presentation
{
    [TestFixture, RequiresSTA]
    internal class MultiSelectionTest
    {
        private TestModel _model;

        private class TestModel : ViewModelBase<TestModel>
        {
            public static readonly string SelectionPropertyName =
                TypeExtensions<TestModel>.GetPropertyName(tm => tm.Selection);

            public static readonly string ListPropertyName = TypeExtensions<TestModel>.GetPropertyName(tm => tm.List);
            private ICollection<string> _selection;

            public TestModel(params string[] list)
            {
                List = new List<string>(list);
                Selection = new ObservableCollection<string>();
            }

            public IList<string> List { get; set; }

            public ICollection<string> Selection
            {
                get { return _selection; }
                set
                {
                    _selection = value;
                    RaisePropertyChanged(tm => tm.Selection);
                }
            }
        }

        [SetUp]
        public void SetUp()
        {
            _model = new TestModel();
        }

        [Test]
        public void ShouldInitializeListBoxWithItemsFromModel()
        {
            _model.List = new[] {"A", "B", "C"};

            var listBox = WhenListBoxIsCreated();

            CollectionAssert.AreEqual(_model.List, listBox.Items);
        }

        [Test]
        public void ShouldInitializeListBoxSelectionFromModel()
        {
            _model.List = new[] {"A", "B", "C"};
            _model.Selection.Add("A");

            var listBox = WhenListBoxIsCreated();
            
            CollectionAssert.AreEqual(_model.Selection, listBox.SelectedItems);
        }

        [Test]
        public void ShouldSynchronizeSelectionFromModelToListBox()
        {
            _model.List = new[] {"A", "B"};
            var listBox = WhenListBoxIsCreated();

            _model.Selection.Add("B");

            CollectionAssert.AreEqual(_model.Selection, listBox.SelectedItems);
        }

        [Test]
        public void ShouldSynchronizeSelectionFromListBoxToModel()
        {
            _model.List = new[] {"A", "B"};
            var listBox = WhenListBoxIsCreated();

            listBox.SelectedItems.Add("A");

            CollectionAssert.AreEqual(new[] {"A"}, _model.Selection);
        }

        [Test]
        public void ShouldSynchronizeSubsequentSelectionChanges()
        {
            _model.List = new[] {"A", "B", "C"};
            var listBox = WhenListBoxIsCreated();

            listBox.SelectedItems.Add("B");
            _model.Selection.Add("C");
            listBox.SelectedItems.Remove("C");

            CollectionAssert.AreEqual(new[] {"B"}, _model.Selection);
            CollectionAssert.AreEqual(_model.Selection, listBox.SelectedItems);
        }

        [Test]
        public void ShouldKeepSynchronizingIfModelSelectionIsSetToNewInstance()
        {
            _model.List = new[] {"A", "B"};
            var listBox = WhenListBoxIsCreated();

            _model.Selection = new ObservableCollection<string>(new[] {"B"});

            CollectionAssert.AreEqual(new[] {"B"}, listBox.SelectedItems);
        }

        [Test]
        public void ShouldForgetOldModelSelectionIfModelSelectionIsSetToNewInstance()
        {
            _model.List = new[] {"A", "B"};
            var listBox = WhenListBoxIsCreated();
            var oldSelection = _model.Selection;
            
            _model.Selection = new ObservableCollection<string>(new[] {"A"});
            oldSelection.Add("B");

            CollectionAssert.AreEqual(new[] {"A"}, listBox.SelectedItems);
        }

        [Test(Description = "Instead of only silently changing the selection instance.")]
        public void ShouldSetSelectionOnModel()
        {
            _model.List = new[] {"A"};
            var listBox = WhenListBoxIsCreated();
            var invoked = false;
            _model.OnPropertyChanged(m => m.Selection, selection => invoked = true);

            listBox.SelectedItems.Add("A");

            Assert.IsTrue(invoked);
        }

        private ListBox WhenListBoxIsCreated()
        {
            var listBox = new ListBox {SelectionMode = SelectionMode.Extended};
            var listBinding = new Binding(TestModel.ListPropertyName) {Source = _model};
            listBox.SetBinding(ItemsControl.ItemsSourceProperty, listBinding);
            var selectionBinding = new Binding(TestModel.SelectionPropertyName) {Source = _model};
            listBox.SetBinding(MultiSelection.SelectedItemsProperty, selectionBinding);
            return listBox;
        }
    }
}