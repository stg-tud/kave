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

using System.Globalization;
using KaVE.VS.FeedbackGenerator.UserControls.Export;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.Export
{
    internal class MailValidationRuleTest
    {
        private MailValidationRule _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new MailValidationRule();
        }

        [Test]
        public void ShouldIdentifyValidAddresses()
        {
            const string validAddress = "mail@web.com";

            var result = _uut.Validate(validAddress, CultureInfo.InvariantCulture);

            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void ShouldIdentifyInvalidAddresses()
        {
            const string invalidAddress = ".test@";

            var result = _uut.Validate(invalidAddress, CultureInfo.InvariantCulture);

            Assert.IsFalse(result.IsValid);
        }
    }
}