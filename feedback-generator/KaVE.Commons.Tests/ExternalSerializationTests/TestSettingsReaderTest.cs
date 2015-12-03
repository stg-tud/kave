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

using System.IO;
using NUnit.Framework;

namespace KaVE.Commons.Tests.ExternalSerializationTests
{
    internal class TestSettingsReaderTest
    {
        private const string TestSection = "SomeSection";
        private const string TestSerializedType = "SomeSerializedType";

        private string _settingsFile;

        [SetUp]
        public void Setup()
        {
            _settingsFile = Path.Combine(Path.GetTempPath(), "ReaderTestSettings.ini");

            var settingsFileContent = new[]
            {
                "[this should not be read]",
                string.Format("{0}={1}", ExternalTestSetting.SerializedType, "this should not be read"),
                string.Format("[{0}]", TestSection),
                string.Format("{0}={1}", ExternalTestSetting.SerializedType, TestSerializedType),
                "[this should not be read]",
                string.Format("{0}={1}", ExternalTestSetting.SerializedType, "this should not be read")
            };

            File.WriteAllLines(_settingsFile, settingsFileContent);
        }

        [TearDown]
        public void Teardown()
        {
            if (File.Exists(_settingsFile))
            {
                File.Delete(_settingsFile);
            }
        }

        [Test]
        public void ShouldReadSerializedTypeFromSection()
        {
            Assert.AreEqual(
                TestSerializedType,
                TestSettingsReader.ReadSection(_settingsFile, TestSection)[ExternalTestSetting.SerializedType]);
        }
    }
}