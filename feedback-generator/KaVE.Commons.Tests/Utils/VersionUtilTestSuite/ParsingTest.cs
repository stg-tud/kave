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
using KaVE.Commons.Utils.Assertion;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.VersionUtilTestSuite
{
    internal class ParsingTest
    {
        [Test, ExpectedException(typeof(AssertException))]
        public void ParameterValidation_null()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            VersionUtil.Parse(null);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ParameterValidation_empty()
        {
            VersionUtil.Parse("");
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ParameterValidation_invalid()
        {
            VersionUtil.Parse("abc");
        }

        [Test]
        public void ParsingRoundtrip()
        {
            var actual = VersionUtil.Parse("0.123-Development");
            var expected = new KaVEVersion
            {
                KaVEVersionNumber = 123,
                Variant = Variant.Development
            };
            Assert.AreEqual(expected, actual);
        }
    }
}