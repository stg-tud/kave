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
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.SessionManager.Anonymize
{
    internal class WindowEventAnonymizerTest : IDEEventAnonymizerTestBase<WindowEvent>
    {
        protected override WindowEvent CreateEventWithAllAnonymizablePropertiesSet()
        {
            return new WindowEvent
            {
                Action = WindowEvent.WindowAction.Activate,
                Window = Names.Window("vsWindowType Solution Explorer")
            };
        }

        [Test]
        public void ShouldAnonymizeWindowNameIfRemoveNamesIsSetAndCaptionContainsFileName()
        {
            OriginalEvent.Window = Names.Window("vsSomeWindowType \\Contains\\File.Name");
            AnonymizationSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual("vsSomeWindowType H8aplQQfP3av08An9vt0KQ==", actual.Window.Identifier);
        }

        [Test]
        public void ShouldNotChangeWindowNameIfRemoveNamesIsSetButCaptionContainsNoFileName()
        {
            OriginalEvent.Window = Names.Window(
                "vsToolWindow Unit Test Sessions - DocumentEventAnonymizerTest");
            AnonymizationSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual("vsToolWindow Unit Test Sessions - DocumentEventAnonymizerTest", actual.Window.Identifier);
        }

        protected override void AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(WindowEvent original,
            WindowEvent anonymized)
        {
            Assert.AreEqual(original.Action, anonymized.Action);
        }
    }
}