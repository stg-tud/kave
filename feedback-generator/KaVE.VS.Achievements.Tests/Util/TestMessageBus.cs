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

using System;
using System.Collections.Generic;
using JetBrains.Util;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.Achievements.Tests.Util
{
    public class TestMessageBus : IMessageBus
    {
        private readonly IList<Action<object>> _receivers = new List<Action<object>>();

        public void Publish<TMessage>(TMessage evt) where TMessage : class
        {
            _receivers.ForEach(r => r.Invoke(evt));
        }

        public void Subscribe<TMessage>(Action<TMessage> action, Func<TMessage, bool> filter = null)
            where TMessage : class
        {
            _receivers.Add(o => action((TMessage) o));
        }
    }
}