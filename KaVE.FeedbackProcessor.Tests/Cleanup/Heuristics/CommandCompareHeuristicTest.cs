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

using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Heuristics
{
    [TestFixture]
    internal class CommandCompareHeuristicTest
    {
        public const string VisualStudioCommand = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:220:Project.AddNewItem";

        public const string ReSharperCommand = "TextControl.Backspace";

        public const string OtherCommand = "New Item...";

        [Test]
        public void ShouldIdentifyVisualStudioCommands()
        {
            Assert.IsTrue(CommandCompareHeuristic.IsVisualStudioCommand(VisualStudioCommand));
            Assert.IsFalse(CommandCompareHeuristic.IsVisualStudioCommand(ReSharperCommand));
            Assert.IsFalse(CommandCompareHeuristic.IsVisualStudioCommand(OtherCommand));
        }

        [Test]
        public void ShouldIdentifyReSharperCommand()
        {
            Assert.IsFalse(CommandCompareHeuristic.IsReSharperCommand(VisualStudioCommand));
            Assert.IsTrue(CommandCompareHeuristic.IsReSharperCommand(ReSharperCommand));
            Assert.IsFalse(CommandCompareHeuristic.IsReSharperCommand(OtherCommand));
        }

        [Test]
        public void ShouldIdentifyOtherCommands()
        {
            Assert.IsFalse(CommandCompareHeuristic.IsOtherCommand(VisualStudioCommand));
            Assert.IsFalse(CommandCompareHeuristic.IsOtherCommand(ReSharperCommand));
            Assert.IsTrue(CommandCompareHeuristic.IsOtherCommand(OtherCommand));
        }

        [TestCase(VisualStudioCommand, VisualStudioCommand, 0)]
        [TestCase(ReSharperCommand, ReSharperCommand, 0)]
        [TestCase(OtherCommand, OtherCommand, 0)]
        [TestCase(VisualStudioCommand, ReSharperCommand, -1)]
        [TestCase(VisualStudioCommand, OtherCommand, -1)]
        [TestCase(ReSharperCommand, OtherCommand, -1)]
        [TestCase(ReSharperCommand, VisualStudioCommand, 1)]
        [TestCase(OtherCommand, VisualStudioCommand, 1)]
        [TestCase(OtherCommand, ReSharperCommand, 1)]
        public void CompareCommandsTest(string command1, string command2, int expected)
        {
            Assert.AreEqual(CommandCompareHeuristic.CompareCommands(command1, command2), expected);
        }
    }
}