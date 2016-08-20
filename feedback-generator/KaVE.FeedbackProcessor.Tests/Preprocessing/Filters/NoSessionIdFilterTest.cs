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

using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.Preprocessing.Filters;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing.Filters
{
    internal class NoSessionIdFilterTest
    {
        private NoSessionIdFilter _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new NoSessionIdFilter();
        }

        [Test]
        public void ShouldPreserveEventsWithNonEmptySid()
        {
            Assert.True(
                _sut.Func2(
                    new CommandEvent
                    {
                        IDESessionUUID = "x"
                    }));
        }

        [Test]
        public void ShouldRemoveEventsWithoutSid_Empty()
        {
            Assert.False(
                _sut.Func2(
                    new CommandEvent
                    {
                        IDESessionUUID = ""
                    }));
        }

        [Test]
        public void ShouldRemoveEventsWithoutSid_Null()
        {
            Assert.False(
                _sut.Func2(
                    new CommandEvent
                    {
                        IDESessionUUID = null
                    }));
        }
    }
}