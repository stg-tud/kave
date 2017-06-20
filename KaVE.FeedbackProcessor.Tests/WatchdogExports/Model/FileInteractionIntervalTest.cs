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

using KaVE.FeedbackProcessor.WatchdogExports.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports.Model
{
    internal class FileInteractionIntervalTest
    {
        [Test]
        public void Equality_Default()
        {
            var i1 = new FileInteractionInterval();
            var i2 = new FileInteractionInterval();

            Assert.AreEqual(i1, i2);
        }

        [Test]
        public void Equality_FileInteractionType()
        {
            var i1 = new FileInteractionInterval {Type = FileInteractionType.Reading};
            var i2 = new FileInteractionInterval {Type = FileInteractionType.Reading};
            var i3 = new FileInteractionInterval {Type = FileInteractionType.Typing};

            Assert.AreEqual(i1, i2);
            Assert.AreNotEqual(i2, i3);
        }
    }
}