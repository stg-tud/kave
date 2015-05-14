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

using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using KaVE.FeedbackProcessor.Utils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Utils
{
    [TestFixture]
    internal class SortedCommandPairTest
    {
        public const string GreaterCommand = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:220:Project.AddNewItem";

        public const string SmallerCommand = "New Item...";

        [SetUp]
        public void CheckForValidInputs()
        {
            Assert.AreEqual(
                1,
                CommandCompareHeuristic.CompareCommands(SmallerCommand, GreaterCommand),
                "Commands are not compared as expected!");
        }

        [Test]
        public void SwapsIfSecondIsSmallerThanFirst()
        {
            var sortedCommandPair = SortedCommandPair.NewSortedPair(GreaterCommand, SmallerCommand);

            Assert.AreEqual(SmallerCommand, sortedCommandPair.Item1);
            Assert.AreEqual(GreaterCommand, sortedCommandPair.Item2);
        }

        [Test]
        public void DoesNotSwapIfSecondIsGreaterThanFirst()
        {
            var sortedCommandPair = SortedCommandPair.NewSortedPair(SmallerCommand, GreaterCommand);

            Assert.AreEqual(SmallerCommand, sortedCommandPair.Item1);
            Assert.AreEqual(GreaterCommand, sortedCommandPair.Item2);
        }
    }
}