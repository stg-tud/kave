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
 *    - Andreas Bauer
 */

using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.VsFeedbackGenerator.SessionManager;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager
{
    [TestFixture]
    internal class EventViewTest
    {
        [Test]
        public void ShouldDisplayFormattedEventDetails()
        {
            var @event = new CommandEvent {CommandId = "test.command"};
            const string expected =
                "    <Bold>\"CommandId\":</Bold> \"test.command\"";

            var view = new EventViewModel(@event);
            var actual = view.Details;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatsCompletionEventSST()
        {
            var completionEvent = new CompletionEvent
            {
                Context2 = {SST = new SST {EnclosingType = TypeName.Get("TestClass,TestProject")}}
            };

            var view = new EventViewModel(completionEvent);
            Assert.IsNotNullOrEmpty(view.XamlContextRepresentation);
        }

        [Test]
        public void DoesntFormatOtherEvents()
        {
            var someEvent = IDEEventTestFactory.SomeEvent();

            var view = new EventViewModel(someEvent);
            Assert.IsNull(view.XamlContextRepresentation);
        }
    }
}