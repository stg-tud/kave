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
using System.Net.Mail;
using System.Windows.Controls;

namespace KaVE.VS.FeedbackGenerator.UserControls.ValidationRules
{
    public class MailValidationRule : KaVEValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return Fail(ValidationErrorMessages.ValueIsNull);
            }

            var mail = value.ToString();
            if (mail.Equals(""))
            {
                return Success();
            }

            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new MailAddress(mail);

                return Success();
            }
            catch (FormatException)
            {
                return Fail(ValidationErrorMessages.UserProfile_InvalidMail);
            }
        }
    }
}