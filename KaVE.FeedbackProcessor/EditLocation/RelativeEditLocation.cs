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

using KaVE.Commons.Utils;

namespace KaVE.FeedbackProcessor.EditLocation
{
    public class RelativeEditLocation
    {
        public int Size { get; set; }
        public int Location { get; set; }

        public bool HasEditLocation
        {
            get { return Location != 0; }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(RelativeEditLocation other)
        {
            return Size == other.Size && Location == other.Location;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 397 + (Size * 397) ^ Location;
            }
        }

        public override string ToString()
        {
            return "{0}({1}/{2})".FormatEx(typeof(RelativeEditLocation).Name, Location, Size);
        }
    }
}