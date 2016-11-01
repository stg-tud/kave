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

using KaVE.Commons.Model;
using KaVE.Commons.Utils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.VersionUtilTestSuite
{
    // TODO @seb: create abstract super class for the three different product version tests
    internal class DevelopmentTest
    {
        [Test]
        public void CurrentVersion()
        {
            var actual = new VersionUtil().GetCurrentVersion().ToString();
            Assert.True(actual.StartsWith("0."));
            Assert.True(actual.EndsWith(".0.0"));
        }

        [Test]
        public void CurrentInformalVersion()
        {
            var actual = new VersionUtil().GetCurrentInformalVersion();
            Assert.True(actual.StartsWith("0."));
            Assert.True(actual.EndsWith("-Development"));
        }

        [Test]
        public void CurrentVariant()
        {
            var actual = new VersionUtil().GetCurrentVariant();
            var expected = Variant.Development;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AlsoWorksForOverwrittenClasses()
        {
            var actual = new ExtendedVersionUtilInAssemblyWithoutVersion().GetCurrentInformalVersion();
            Assert.True(actual.StartsWith("0."));
            Assert.True(actual.EndsWith("-Development"));
        }

        private class ExtendedVersionUtilInAssemblyWithoutVersion : VersionUtil {}
    }
}