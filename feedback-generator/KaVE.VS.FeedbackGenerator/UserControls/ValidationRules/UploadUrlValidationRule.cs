﻿/*
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
using KaVE.VS.FeedbackGenerator.SessionManager.Presentation;

namespace KaVE.VS.FeedbackGenerator.UserControls.ValidationRules
{
    public class UploadUrlValidationRule : KaVEValidationRule
    {
        public UploadUrlValidationRule() : base(Properties.SessionManager.Options_Export_UploadUrl) {}

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return Fail(ValidationErrorMessages.ValueIsNull);
            }

            var url = value.ToString();
            if (url.Equals(""))
            {
                return Fail(ValidationErrorMessages.ValueIsEmptyString);
            }

            return UrlValidation.ValidateUrl(url)
                ? Success()
                : Fail(ValidationErrorMessages.GeneralOptions_InvalidUploadURL);
        }

        public static ValidationResult Validate(object value)
        {
            return new UploadUrlValidationRule().Validate(value, CultureInfo.InvariantCulture);
        }
    }
}