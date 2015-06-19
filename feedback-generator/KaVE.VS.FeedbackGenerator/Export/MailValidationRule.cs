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
 * 
 * Contributors:
 *    - Mattis Manfred Kämmerer
 */

using System;
using System.Globalization;
using System.Net.Mail;
using System.Windows.Controls;

namespace KaVE.VS.FeedbackGenerator.Export
{
    public class MailValidationRule : ValidationRule
    {
        private const string ErrorMessage = "Please enter a valid mail address.";

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (value != null && !value.Equals(""))
                {
                    // ReSharper disable once ObjectCreationAsStatement
                    new MailAddress(value.ToString());
                }
                return new ValidationResult(true, null);
            }
            catch (FormatException)
            {
                return new ValidationResult(false, ErrorMessage);
            }
        }
    }
}