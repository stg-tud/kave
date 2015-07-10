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

using System.Collections.Generic;
using System.Linq;

namespace KaVE.RS.Commons.Utils
{
    public static class ProposalProbabilityUtils
    {
        private const double InvalidProbability = double.NaN;

        /// <summary>
        ///     Can be invalid! (e.g. if probabilities is empty) Use the IsValidProbability extension to check for validity
        /// </summary>
        public static double GetPresentedProbability(this IEnumerable<double> probabilities)
        {
            try
            {
                return probabilities.Max();
            }
            catch
            {
                return InvalidProbability;
            }
        }

        public static bool IsValidProbability(this double probability)
        {
            return probability >= 0 && probability <= 100;
        }
    }
}