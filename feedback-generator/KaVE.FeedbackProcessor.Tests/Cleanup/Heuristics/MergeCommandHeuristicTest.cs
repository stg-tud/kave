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
 *    - Mattis Manfred Kämmerer
 *    - Markus Zimmermann
 */

using System;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Heuristics
{
    [TestFixture]
    internal class MergeCommandHeuristicTest
    {
        private const string SomeIdeSessionUUID = "5fbfae0b-f787-4fef-a355-ba2fb8ab913a";
        private const string SomeId = "SomeId";
        private const string SomeKaVeVersion = "0.768";

        public readonly CommandEvent VisualStudioCommandEvent = new CommandEvent
        {
            ActiveDocument =
                DocumentName.Get(
                    "VisualStudio.DocumentName:CSharp \\KaVE.FeedbackProcessor.Tests\\Cleanup\\Heuristics\\MergeCommandHeuristicTest.cs"),
            ActiveWindow = WindowName.Get("VisualStudio.WindowName:vsWindowTypeSolutionExplorer Solution Explorer"),
            CommandId = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:220:Project.AddNewItem",
            Id = SomeId,
            IDESessionUUID = SomeIdeSessionUUID,
            KaVEVersion = SomeKaVeVersion,
            TriggeredBy = IDEEvent.Trigger.Unknown
        };

        public readonly CommandEvent ClickCommandEvent = new CommandEvent
        {
            ActiveDocument =
                DocumentName.Get(
                    "VisualStudio.DocumentName:CSharp \\KaVE.FeedbackProcessor.Tests\\Cleanup\\Heuristics\\MergeCommandHeuristicTest.cs"),
            ActiveWindow = WindowName.Get("VisualStudio.WindowName:vsWindowTypeSolutionExplorer Solution Explorer"),
            CommandId = "New Item...",
            Id = SomeId,
            IDESessionUUID = SomeIdeSessionUUID,
            KaVEVersion = SomeKaVeVersion,
            TriggeredBy = IDEEvent.Trigger.Click
        };
        
        [Test]
        public void ShouldMergeEventsCorrectly()
        {
            var now = DateTime.Now;

            var expectedDuration = TimeSpan.FromSeconds(2);            
            ClickCommandEvent.TriggeredAt = now;
            ClickCommandEvent.TerminatedAt = now.AddSeconds(1);
            VisualStudioCommandEvent.TriggeredAt = now.AddSeconds(1);
            VisualStudioCommandEvent.TerminatedAt = now.Add(expectedDuration);
            
            var mergedEvent = MergeCommandHeuristic.MergeCommandEvents(
                VisualStudioCommandEvent, ClickCommandEvent);

            Assert.AreEqual(
                VisualStudioCommandEvent.ActiveDocument,
                mergedEvent.ActiveDocument,
                "ActiveDocument was not merged correctly");

            Assert.AreEqual(
                VisualStudioCommandEvent.ActiveWindow,
                mergedEvent.ActiveWindow,
                "ActiveWindow was not merged correctly");

            Assert.AreEqual(
                VisualStudioCommandEvent.CommandId,
                mergedEvent.CommandId,
                "CommandId was not merged correctly");

            Assert.AreEqual(
                expectedDuration,
                mergedEvent.Duration,
                "Duration was not merged correctly");

            Assert.AreEqual(
                SomeId,
                mergedEvent.Id,
                "Id was not merged correctly");

            Assert.AreEqual(
                SomeIdeSessionUUID,
                mergedEvent.IDESessionUUID,
                "Duration was not merged correctly");

            Assert.AreEqual(
                SomeKaVeVersion,
                mergedEvent.KaVEVersion,
                "KaVEVersion was not merged correctly");

            Assert.AreEqual(
                VisualStudioCommandEvent.TerminatedAt,
                mergedEvent.TerminatedAt,
                "TerminatedAt was not merged correctly");

            Assert.AreEqual(
                ClickCommandEvent.TriggeredAt,
                mergedEvent.TriggeredAt,
                "TriggeredAt was not merged correctly");

            Assert.AreEqual(
                IDEEvent.Trigger.Click,
                mergedEvent.TriggeredBy,
                "TriggeredBy was not merged correctly");
        }
    }
}