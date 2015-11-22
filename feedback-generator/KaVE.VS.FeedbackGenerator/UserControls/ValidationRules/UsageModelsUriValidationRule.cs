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
using System.Globalization;
using System.Windows.Controls;

namespace KaVE.VS.FeedbackGenerator.UserControls.ValidationRules
{
    internal class UsageModelsUriValidationRule : KaVEValidationRule
    {
        public UsageModelsUriValidationRule() : base(Properties.SessionManager.Options_ModelUri) {}

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return Fail(ValidationErrorMessages.ValueIsNull);
            }

            var valueString = value.ToString();
            if (valueString.Equals(""))
            {
                return Success();
            }

            Uri uri;
            try
            {
                uri = new Uri(valueString);
            }
            catch (UriFormatException)
            {
                return Fail(ValidationErrorMessages.UsageModelsOptions_InvalidUri);
            }

            if (uri.Scheme == Uri.UriSchemeFile)
            {
                return new UsageModelsPathValidationRule().Validate(value, cultureInfo);
            }

            if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            {
                return Success();
            }

            return new ValidationResult(false, null);
        }

        public static ValidationResult Validate(object value)
        {
            return new UsageModelsUriValidationRule().Validate(value, CultureInfo.InvariantCulture);
        }
    }
}