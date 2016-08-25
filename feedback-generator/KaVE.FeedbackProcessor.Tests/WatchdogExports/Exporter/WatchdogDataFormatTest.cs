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
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports.Exporter
{
    internal class WatchdogDataFormatTest
    {
        [Test]
        public void WatchdogStringValuesAreEscapedCorrectly()
        {
            var str = new WatchdogStringValue {Value = "\\ \""};

            Assert.AreEqual("\"\\\\ \\\"\"", str.ToString());
        }

        [Test]
        public void WatchdogDoubleValueFormatCorrectly()
        {
            var dbl = new WatchdogDoubleValue {Value = 0.000001};

            Assert.AreEqual("0.000001", dbl.ToString());
        }

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

        [Test]
        public void WatchdogArrayFormatsCorrectly()
        {
            var arr = new WatchdogArray();

            arr.Elements.Add(new WatchdogStringValue {Value = "a"});
            Assert.AreEqual("[\"a\"]", arr.ToString());

            arr.Elements.Add(new WatchdogObject {Properties = {{"k", new WatchdogIntValue {Value = 42}}}});
            Assert.AreEqual("[\"a\",{\"k\":42}]", arr.ToString());
        }
    }
}