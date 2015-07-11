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

using System;
using JetBrains.Util;
using KaVE.Commons.Model.Events.VisualStudio;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.SessionManager.Anonymize
{
    [TestFixture]
    internal class BuildEventAnonymizerTest : IDEEventAnonymizerTestBase<BuildEvent>
    {
        private TimeSpan _testBuildTargetDuration;
        private DateTime _testBuildTargetStartTime;
        private const string TestTargetProjectName = "ProjectName";
        private const string TestTargetProjectNameHash = "0Wc0SWJ1Vy6bpzAFL2QHMg==";

        protected override BuildEvent CreateEventWithAllAnonymizablePropertiesSet()
        {
            return new BuildEvent
            {
                Scope = "Build All",
                Action = "Rebuild",
                Targets = new[]
                {
                    CreateBuildTarget(),
                    CreateBuildTarget(),
                    CreateBuildTarget()
                }
            };
        }

        protected BuildTarget CreateBuildTarget()
        {
            _testBuildTargetStartTime = DateTime.Now;
            _testBuildTargetDuration = TimeSpan.FromMinutes(42);
            return new BuildTarget
            {
                StartedAt = _testBuildTargetStartTime,
                Duration = _testBuildTargetDuration,
                Platform = "x86",
                Project = TestTargetProjectName,
                ProjectConfiguration = "Debug",
                SolutionConfiguration = "Release",
                Successful = true
            };
        }

        [Test]
        public void ShouldRemoveBuildTargetStartTimesWhenRemovingStartTimes()
        {
            ExportSettings.RemoveStartTimes = true;

            var actual = WhenEventIsAnonymized();

            actual.Targets.ForEach(target => Assert.IsNull(target.StartedAt));
        }

        [Test]
        public void ShouldRemoveBuildTargetDurationsWhenRemovingDurations()
        {
            ExportSettings.RemoveDurations = true;

            var actual = WhenEventIsAnonymized();

            actual.Targets.ForEach(target => Assert.IsNull(target.Duration));
        }

        [Test]
        public void ShouldAnonymizeProjectNameFromBuildTargetsWhenRemovingNames()
        {
            ExportSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            actual.Targets.ForEach(target => Assert.AreEqual(TestTargetProjectNameHash, target.Project));
        }

        protected override void AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(BuildEvent original,
            BuildEvent anonymized)
        {
            Assert.AreEqual(original.Scope, anonymized.Scope);
            Assert.AreEqual(original.Action, anonymized.Action);
            Assert.AreEqual(original.Targets.Count, anonymized.Targets.Count);
            AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(original.Targets[0], anonymized.Targets[0]);
            AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(original.Targets[1], anonymized.Targets[1]);
            AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(original.Targets[2], anonymized.Targets[2]);
        }

        private static void AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(BuildTarget original,
            BuildTarget anonymized)
        {
            Assert.AreEqual(original.Platform, anonymized.Platform);
            Assert.AreEqual(original.ProjectConfiguration, anonymized.ProjectConfiguration);
            Assert.AreEqual(original.SolutionConfiguration, anonymized.SolutionConfiguration);
            Assert.AreEqual(original.Successful, anonymized.Successful);
        }
    }
}