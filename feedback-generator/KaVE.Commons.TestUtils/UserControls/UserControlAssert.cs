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

using System.Windows;
using System.Windows.Controls;
using NUnit.Framework;

namespace KaVE.Commons.TestUtils.UserControls
{
    public class UserControlAssert
    {
        public static void IsChecked(CheckBox checkBox)
        {
            var isChecked = checkBox.IsChecked;
            if (isChecked.HasValue)
            {
                Assert.True(isChecked.Value);
            }
            else
            {
                Assert.Fail();
            }
        }

        public static void IsNotChecked(CheckBox checkBox)
        {
            var isChecked = checkBox.IsChecked;
            if (isChecked.HasValue)
            {
                Assert.False(isChecked.Value);
            }
            else
            {
                Assert.Fail();
            }
        }

        public static void IsVisible(UIElement element)
        {
            Assert.AreEqual(Visibility.Visible, element.Visibility);
        }

        public static void IsNotVisible(UIElement element, Visibility expectedvisibility = Visibility.Collapsed)
        {
            Assert.AreEqual(expectedvisibility, element.Visibility);
        }

        public static void IsDisabled(UIElement element)
        {
            Assert.False(element.IsEnabled);
        }

        public static void IsEnabled(UIElement element)
        {
            Assert.False(element.IsEnabled);
        }
    }
}