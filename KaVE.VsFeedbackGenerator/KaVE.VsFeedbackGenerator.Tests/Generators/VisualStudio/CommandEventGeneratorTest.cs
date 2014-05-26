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

using System.Collections.Generic;
using EnvDTE;
using KaVE.VsFeedbackGenerator.Generators.VisualStudio;
using Microsoft.VisualStudio.CommandBars;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators.VisualStudio
{
    [TestFixture]
    internal class CommandEventGeneratorTest : VisualStudioEventGeneratorTestBase
    {
        private Mock<CommandEvents> _mockCommandEvents;

        protected override void MockEvents(Mock<Events> mockEvents)
        {
            _mockCommandEvents = new Mock<CommandEvents>();
            // ReSharper disable once UseIndexedProperty
            mockEvents.Setup(events => events.get_CommandEvents("{00000000-0000-0000-0000-000000000000}", 0)).Returns(_mockCommandEvents.Object);
        }

        [SetUp]
        public void SetUp()
        {
            var mockCommandBars = new Mock<CommandBars>();
            mockCommandBars.Setup(cb => cb.GetEnumerator()).Returns(new List<CommandBar>().GetEnumerator());
            TestIDESession.MockDTE.Setup(dte => dte.CommandBars).Returns(mockCommandBars.Object);
            // ReSharper disable once ObjectCreationAsStatement
            new CommandEventGenerator(TestIDESession, TestMessageBus);
        }

        [Test, Ignore]
        public void ShouldNotFireNamelessVsCoreEvent()
        {
            _mockCommandEvents.Raise(ce => ce.BeforeExecute += null, "guid", 0, null, null, true);
        }
    }
}