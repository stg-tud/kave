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
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;

namespace KaVE.Commons.Model.Names.CSharp
{
    internal static class ParametersUtils
    {
        public static bool HasParameters(this string nameWithParametersIdentifier)
        {
            return !nameWithParametersIdentifier.Contains("()");
        }

        public static IList<IParameterName> GetParameterNames(this string nameWithParametersIdentifier)
        {
            var parameters = new List<IParameterName>();
            if (nameWithParametersIdentifier.HasParameters())
            {
                var startOfParameterIdentifier = nameWithParametersIdentifier.IndexOf('(') + 1;
                var endOfParameterList = nameWithParametersIdentifier.IndexOf(')');
                do
                {
                    var endOfParameterType = nameWithParametersIdentifier.IndexOf(
                        "] ",
                        startOfParameterIdentifier,
                        StringComparison.Ordinal);
                    var endOfParameterIdentifier = nameWithParametersIdentifier.IndexOf(',', endOfParameterType);
                    if (endOfParameterIdentifier < 0 || endOfParameterIdentifier > endOfParameterList)
                    {
                        endOfParameterIdentifier = endOfParameterList;
                    }
                    var lengthOfParameterIdentifier = endOfParameterIdentifier - startOfParameterIdentifier;
                    var identifier =
                        nameWithParametersIdentifier.Substring(startOfParameterIdentifier, lengthOfParameterIdentifier)
                                                    .Trim();
                    parameters.Add(ParameterName.Get(identifier));
                    startOfParameterIdentifier = endOfParameterIdentifier + 1;
                } while (startOfParameterIdentifier < endOfParameterList);
            }
            return parameters;
        }
    }
}