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
using KaVE.Commons.Utils;

namespace KaVE.Commons.Model.ObjectUsage
{
    public class CoReProposal
    {
        public CoReName Name { get; set; }
        public double Probability { get; set; }

        public CoReProposal(CoReName name, double probability)
        {
            Name = name;
            Probability = probability;
        }

        public string FormattedProbability
        {
            get { return string.Format("{0:n1}%", (int) (Probability*100)); }
        }

        private bool Equals(CoReProposal other)
        {
            return Equals(Name, other.Name) && Math.Abs(Probability - other.Probability) < 0.001;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ Probability.GetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format("Proposal: {0} ({1})", Name, FormattedProbability);
        }
    }
}