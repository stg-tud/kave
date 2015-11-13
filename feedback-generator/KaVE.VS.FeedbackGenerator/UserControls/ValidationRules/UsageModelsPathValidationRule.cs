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
using System.IO;
using System.Windows.Controls;

namespace KaVE.VS.FeedbackGenerator.UserControls.ValidationRules
{
    internal class UsageModelsPathValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var path = value as string;
            if (path == null || !Directory.Exists(path) || !File.Exists(Path.Combine(path, "index.json.gz")))
            {
                return new ValidationResult(false, null);
            }

            return new ValidationResult(true, null);
        }

        public static ValidationResult Validate(object value)
        {
            return new UsageModelsUriValidationRule().Validate(value, CultureInfo.InvariantCulture);
        }
    }
}