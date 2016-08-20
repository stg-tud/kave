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
using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.Preprocessing.Filters;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing.Filters
{
    internal class NoTimeFilterTest
    {
        private NoTimeFilter _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new NoTimeFilter();
        }

        [Test]
        public void HasName()
        {
            Assert.AreEqual(typeof(NoTimeFilter).Name, _sut.Name);
        }

        [Test]
        public void KeepsEventsWithTime()
        {
            var e = new CommandEvent
            {
                TriggeredAt = DateTime.Now
            };
            Assert.True(_sut.Func(e));
        }

        [Test]
        public void RemovesEventsWithDefaultTime()
        {
            var e = new CommandEvent();
            e.TriggeredAt = null;
            Assert.False(_sut.Func(e));
        }
    }
}