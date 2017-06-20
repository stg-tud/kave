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
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Processors
{
    internal class RedundantCommandFilterTest
    {
        private RedundantCommandFilter _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new RedundantCommandFilter();
        }

        [Test]
        public void ShouldDropRedundantCommandEvents()
        {
            var irrelevantEvent = new CommandEvent
            {
                CommandId = RedundantCommandFilter.RedundantCommands[0]
            };

            var actualSet = _uut.Map(irrelevantEvent);

            CollectionAssert.IsEmpty(actualSet);
        }

        [Test]
        public void ShouldNotDropAnyOtherCommandEvents()
        {
            var someOtherEvent = new CommandEvent();

            var actualSet = _uut.Map(someOtherEvent);

            CollectionAssert.AreEquivalent(Sets.NewHashSet<IDEEvent>(someOtherEvent), actualSet);
        }
    }
}