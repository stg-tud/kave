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

using KaVE.FeedbackProcessor.WatchdogExports.Exporter;
using KaVE.FeedbackProcessor.WatchdogExports.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports.Exporter
{
    internal class WatchdogExporterTest
    {
        [Test]
        public void CreatesIntervalObjectForEachInterval()
        {
            var i1 = new VisualStudioOpenedInterval {UserId = "a"};
            var i2 = new VisualStudioOpenedInterval {UserId = "a"};
            var i3 = new VisualStudioOpenedInterval {UserId = "b"};

            var data = WatchdogExporter.Convert(new Interval[] {i1, i2, i3});
            Assert.AreEqual(3, data.Intervals.Count);
        }

        [Test]
        public void CreatesUserObjectForEachUserId()
        {
            var i1 = new VisualStudioOpenedInterval {UserId = "a"};
            var i2 = new VisualStudioOpenedInterval {UserId = "a"};
            var i3 = new VisualStudioOpenedInterval {UserId = "b"};

            var data = WatchdogExporter.Convert(new Interval[] {i1, i2, i3});
            Assert.AreEqual(2, data.Users.Count);
        }

        [Test]
        public void CreatesProjectObjectForEachProjectId()
        {
            var i1 = new VisualStudioOpenedInterval {Project = "a"};
            var i2 = new VisualStudioOpenedInterval {Project = "a"};
            var i3 = new VisualStudioOpenedInterval {Project = "b"};

            var data = WatchdogExporter.Convert(new Interval[] {i1, i2, i3});
            Assert.AreEqual(2, data.Projects.Count);
        }
    }
}