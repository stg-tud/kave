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

using KaVE.FeedbackProcessor.Intervals.Exporter;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Intervals.Exporter
{
    internal class WatchdogDataFormatTest
    {
        [Test]
        public void WatchdogWrappedValueFormatsCorrectly()
        {
            var wrap = new WatchdogWrappedValue {Wrapper = "ObjectId", Value = "abc"};

            Assert.AreEqual("ObjectId(\"abc\")", wrap.ToString());
        }

        [Test]
        public void WatchdogObjectFormatsCorrectly()
        {
            var obj = new WatchdogObject();

            obj.Properties.Add("a", new WatchdogIntValue {Value = 1});
            Assert.AreEqual("{\"a\":1}", obj.ToString());

            obj.Properties.Add("b", new WatchdogIntValue {Value = 2});
            Assert.AreEqual("{\"a\":1,\"b\":2}", obj.ToString());
        }
    }
}