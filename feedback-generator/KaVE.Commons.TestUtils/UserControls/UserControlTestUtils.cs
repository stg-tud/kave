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
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace KaVE.Commons.TestUtils.UserControls
{
    public static class UserControlTestUtils
    {
        public static void Toggle(this CheckBox checkBox)
        {
            var peer = new CheckBoxAutomationPeer(checkBox);
            var tpattern = peer.GetPattern(PatternInterface.Toggle) as IToggleProvider;
            if (tpattern != null)
            {
                tpattern.Toggle();
            }
        }

        public static void Click(ButtonBase button)
        {
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        public static void Click(Hyperlink hyperlink)
        {
            hyperlink.RaiseEvent(new RoutedEventArgs(Hyperlink.ClickEvent));
        }
    }
}