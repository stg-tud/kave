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

using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Naming;
using KaVE.FeedbackProcessor.Activities;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities
{
    internal class SolutionEventMapperTest : BaseToActivityMapperTest
    {
        public override BaseToActivityMapper Sut
        {
            get { return new SolutionEventToActivityMapper(); }
        }

        [Test]
        public void ShouldAlwaysDropBuildEvents()
        {
            var @event = new SolutionEvent
            {
                Target = Names.Document("d d"),
                Action = SolutionAction.AddProject
            };
            // TODO I'm not sure about the correct category...
            AssertDrop(@event);
        }
    }
}