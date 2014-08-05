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
using KaVE.VsFeedbackGenerator.Interactivity;

namespace KaVE.VsFeedbackGenerator.Tests.Interactivity
{
    /// <summary>
    ///     Captures a single interaction request.
    /// </summary>
    public class InteractionRequestTestHelper<T> where T : Notification
    {
        public bool IsRequestRaised { get; private set; }
        public T Context { get; private set; }
        public Action Callback { get; private set; }

        internal InteractionRequestTestHelper(IInteractionRequest<T> request)
        {
            request.Raised += (s, e) =>
            {
                IsRequestRaised = true;
                Context = e.Notification;
                Callback = e.Callback;
            };
        }
    }

    public static class InteractionRequestTestHelperExtensions
    {
        public static InteractionRequestTestHelper<TContext> NewTestHelper<TContext>(this IInteractionRequest<TContext> request)
            where TContext : Notification
        {
            return new InteractionRequestTestHelper<TContext>(request);
        }
    }
}