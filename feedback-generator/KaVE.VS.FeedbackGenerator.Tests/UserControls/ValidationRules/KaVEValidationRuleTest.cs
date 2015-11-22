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
using System.Windows.Controls;
using KaVE.VS.FeedbackGenerator.UserControls.ValidationRules;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.ValidationRules
{
    internal class KaVEValidationRuleTest
    {
        public const string TestRuleTarget = "Test";

        [Test]
        public void ShouldReturnValidResultOnSuccess()
        {
            KaVEValidationRuleTestImpl.ValidationSuccessful = true;
            Assert.IsTrue(new KaVEValidationRuleTestImpl().Validate(new object(), CultureInfo.InvariantCulture).IsValid);
        }

        [Test]
        public void SuccessfulResultShouldHaveEmptyMessage()
        {
            KaVEValidationRuleTestImpl.ValidationSuccessful = true;
            Assert.AreEqual(
                "",
                new KaVEValidationRuleTestImpl().Validate(new object(), CultureInfo.InvariantCulture).ErrorContent);
        }

        [Test]
        public void ShouldReturnInvalidResultOnFail()
        {
            KaVEValidationRuleTestImpl.ValidationSuccessful = false;
            Assert.IsFalse(
                new KaVEValidationRuleTestImpl().Validate(new object(), CultureInfo.InvariantCulture).IsValid);
        }

        [Test]
        public void ShouldAddGivenErrorMessageOnFail()
        {
            const string errorMessage = "SomeError";
            KaVEValidationRuleTestImpl.ValidationSuccessful = false;
            KaVEValidationRuleTestImpl.ErrorMessage = errorMessage;
            Assert.AreEqual(
                errorMessage,
                new KaVEValidationRuleTestImpl().Validate(new object(), CultureInfo.InvariantCulture).ErrorContent);
        }

        private class KaVEValidationRuleTestImpl : KaVEValidationRule
        {
            public static bool ValidationSuccessful;
            public static string ErrorMessage;

            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                return ValidationSuccessful ? Success() : Fail(ErrorMessage);
            }
        }
    }
}