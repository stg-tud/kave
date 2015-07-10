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

using KaVE.RS.Commons.Utils;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Unit.Utils
{
    internal class ProposalProbabilityUtilsTest
    {
        [Test]
        public void PresentedProbabilityShouldBeMaximum()
        {
            const double expected = 100d;
            var testProbabilities = new[] {0.0, 10.5, expected, 25.95};
            Assert.AreEqual(expected, testProbabilities.GetPresentedProbability());
        }

        [Test]
        public void PresentedProbabilityShouldBeInvalidForEmptyEnumerable()
        {
            double[] emptyArray = {};
            Assert.IsFalse(emptyArray.GetPresentedProbability().IsValidProbability());
        }

        [Test]
        public void NegativeProbabilityShouldBeInvalid()
        {
            const double negativeProbability = -1d;
            Assert.IsFalse(negativeProbability.IsValidProbability());
        }

        [Test]
        public void ProbabilityHigherThanOneHundredShouldBeInvalid()
        {
            const double probabilityHigherThanOneHundred = 100.01;
            Assert.IsFalse(probabilityHigherThanOneHundred.IsValidProbability());
        }

        [Test]
        public void ValidProbabilitiesShouldBeIdentifiedAsValid()
        {
            const double validProbability = 19.84;
            Assert.IsTrue(validProbability.IsValidProbability());
        }
    }
}