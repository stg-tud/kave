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
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.VS.FeedbackGenerator.Properties;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.PropertiesTests
{
    internal class EnumLocalizationTest
    {
        [Test]
        public void LocalizationOfEducation()
        {
            CheckEnumLocalization<Educations>();
        }

        [Test]
        public void LocalizationOfPosition()
        {
            CheckEnumLocalization<Positions>();
        }

        private static void CheckEnumLocalization<T>()
        {
            var enumType = typeof (T);
            var localizationType = typeof (EnumLocalization);

            var prefix = string.Format("{0}_", enumType.Name);

            // ReSharper disable once SuspiciousTypeConversion.Global
            var expecteds =
                ((IEnumerable<T>) Enum.GetValues(enumType)).Select(v => string.Format("{0}{1}", prefix, v)).ToList();
            var actuals = localizationType.GetProperties().Select(p => p.Name).Where(p => p.StartsWith(prefix)).ToList();
            CollectionAssert.AreEquivalent(expecteds, actuals);
        }
    }
}