/*
 * Copyright 2017 Sebastian Proksch
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

using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.FeedbackProcessor.EditLocation
{
    public interface IEditLocationResults
    {
        [NotNull]
        string Zip { get; }

        int NumEvents { get; }
        int NumCompletionEvents { get; }
        int NumLocations { get; }

        [NotNull]
        IKaVEList<RelativeEditLocation> AppliedEditLocations { get; }

        [NotNull]
        IKaVEList<RelativeEditLocation> OtherEditLocations { get; }

        void Add(IEditLocationResults other);
    }

    public class EditLocationResults : IEditLocationResults
    {
        public string Zip { get; set; }
        public int NumEvents { get; set; }
        public int NumCompletionEvents { get; set; }
        public int NumLocations { get; set; }
        public IKaVEList<RelativeEditLocation> AppliedEditLocations { get; set; }
        public IKaVEList<RelativeEditLocation> OtherEditLocations { get; set; }

        public EditLocationResults()
        {
            Zip = "";
            AppliedEditLocations = Lists.NewList<RelativeEditLocation>();
            OtherEditLocations = Lists.NewList<RelativeEditLocation>();
        }

        public void Add(IEditLocationResults other)
        {
            NumEvents += other.NumEvents;
            NumCompletionEvents += other.NumCompletionEvents;
            NumLocations += other.NumLocations;
            AppliedEditLocations.AddAll(other.AppliedEditLocations);
            OtherEditLocations.AddAll(other.OtherEditLocations);
        }

        private bool Equals(EditLocationResults other)
        {
            return string.Equals(Zip, other.Zip) && NumEvents == other.NumEvents &&
                   NumCompletionEvents == other.NumCompletionEvents &&
                   NumLocations == other.NumLocations && AppliedEditLocations.Equals(other.AppliedEditLocations) &&
                   OtherEditLocations.Equals(other.OtherEditLocations);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 1928;
                hashCode = (hashCode * 397) ^ Zip.GetHashCode();
                hashCode = (hashCode * 397) ^ NumEvents;
                hashCode = (hashCode * 397) ^ NumCompletionEvents;
                hashCode = (hashCode * 397) ^ NumLocations;
                hashCode = (hashCode * 397) ^ AppliedEditLocations.GetHashCode();
                hashCode = (hashCode * 397) ^ OtherEditLocations.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}