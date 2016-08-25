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

namespace KaVE.FeedbackProcessor.WatchdogExports.Model
{
    public enum PerspectiveType
    {
        Production,
        Debug
    }

    public class PerspectiveInterval : Interval
    {
        public PerspectiveType Perspective { get; set; }

        protected bool Equals(PerspectiveInterval other)
        {
            return base.Equals(other) && Perspective == other.Perspective;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((PerspectiveInterval) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (int) Perspective;
            }
        }

        public override string ToString()
        {
            var str = base.ToString();
            switch (Perspective)
            {
                case PerspectiveType.Production:
                    str += " (Production)";
                    break;
                case PerspectiveType.Debug:
                    str += " (Debug)";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return str;
        }
    }
}