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
using KaVE.Model.Events;
using KaVE.Model.Events.ReSharper;
using KaVE.Model.Events.VisualStudio;
using KaVE.Model.Names.VisualStudio;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Presentation
{
    [TestFixture]
    class IDEEventDetailsToJsonConverterTest
    {
        [Test]
        public void ShouldConvertActionEventDetailsToXaml()
        {
            var actionEvent = new WindowEvent{Window = WindowName.Get("MyWindow"), Action = WindowEvent.WindowAction.Create};
            const string expected = "    \"Window\": \"MyWindow\"\r\n" +
                                    "    \"Action\": \"Create\"";
            var actual = actionEvent.GetDetailsAsJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotIncludeIDEEventPropertiesInDetails()
        {
            var actionEvent = new ActionEvent
            {
                ActiveDocument = DocumentName.Get("Doc"),
                ActiveWindow = WindowName.Get("Window"),
                IDESessionUUID = "UUID",
                TerminatedAt = DateTime.Now,
                TriggeredAt = DateTime.Now,
                TriggeredBy = IDEEvent.Trigger.Click
            };
            const string expected = "";
            var actual = actionEvent.GetDetailsAsJson();
            Assert.AreEqual(expected, actual);
        }
    }
}
