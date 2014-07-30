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

using System;
using KaVE.Utils.Assertion;

namespace KaVE.VsFeedbackGenerator.Interactivity
{
    internal class InteractionRequest<TNotification> : IInteractionRequest<TNotification> where TNotification : Notification
    {
        public event EventHandler<InteractionRequestedEventArgs<TNotification>> Raised = null;

        public void Raise(TNotification notification)
        {
            Raise(notification, n => { });
        }
        public void Raise(TNotification notification, Action<TNotification> callback)
        {
            Asserts.NotNull(Raised, "there is no handler registered for this interaction request");
            Raised(
                this,
                new InteractionRequestedEventArgs<TNotification>
                {
                    Notification = notification,
                    Callback = () => callback(notification)
                });
        }

        public void Delegate(InteractionRequestedEventArgs<TNotification> args)
        {
            Raised(this, args);
        }
    }
}