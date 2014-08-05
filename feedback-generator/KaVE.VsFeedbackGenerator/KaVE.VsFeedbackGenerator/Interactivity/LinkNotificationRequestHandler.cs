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

using System.Windows;
using MsgBox;

namespace KaVE.VsFeedbackGenerator.Interactivity
{
    internal class LinkNotificationRequestHandler
    {
        private readonly Window _window;

        public LinkNotificationRequestHandler(DependencyObject parent)
        {
            _window = Window.GetWindow(parent);
        }

        public void Handle(object sender, InteractionRequestedEventArgs<LinkNotification> args)
        {
            var notification = args.Notification;

            if (_window == null)
            {
                Msg.Show(
                    notification.Message,
                    notification.Caption,
                    MsgBoxButtons.OK,
                    MsgBoxImage.Default,
                    MsgBoxResult.OK,
                    notification.Link,
                    helpLinkLabel:notification.LinkDescription);
            }
            else
            {
                Msg.Show(
                    _window,
                    notification.Message,
                    notification.Caption,
                    MsgBoxResult.OK,
                    true,
                    image: MsgBoxImage.Default,
                    helpLink: notification.Link,
                    helpLinkLabel:notification.LinkDescription);
            }
        }
    }
}