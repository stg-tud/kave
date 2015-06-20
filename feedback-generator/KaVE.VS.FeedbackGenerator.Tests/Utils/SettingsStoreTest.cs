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
using JetBrains.Application.Settings;
using KaVE.VS.FeedbackGenerator.Utils;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Utils
{
    [SettingsKey(typeof (EnvironmentSettings), "Test Settings")]
    internal class TestSettings
    {
        [SettingsEntry("default string", "a string setting represented by a field, default value is a string")]
        public string FieldSetting;

        [SettingsEntry(true, "a boolean setting represented by a property, default value is a boolean")]
        public bool BooleanSetting { get; set; }

        [SettingsEntry("01/01/0001 00:00:00", "a date-time setting, default value is a string")]
        public DateTime DateTimeSetting;

        [SettingsEntry("00:00:42", "a time-span setting, default value is string")]
        public TimeSpan TimeSpanSetting;

        [SettingsEntry(null, "a setting with default value null")]
        public string DefaultNullSetting;
    }

    internal class SettingsStoreTest
    {
        [Test]
        public void ShouldCreateSettingsDefaultInstance()
        {
            var actual = SettingsStore.CreateDefaultInstance<TestSettings>();

            Assert.AreEqual("default string", actual.FieldSetting);
            Assert.IsTrue(actual.BooleanSetting);
            Assert.AreEqual(DateTime.MinValue, actual.DateTimeSetting);
            Assert.AreEqual(TimeSpan.FromSeconds(42), actual.TimeSpanSetting);
            Assert.IsNull(actual.DefaultNullSetting);
        }
    }
}