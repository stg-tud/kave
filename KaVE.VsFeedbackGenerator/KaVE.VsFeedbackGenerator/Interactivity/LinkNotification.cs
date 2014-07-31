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
 *    - Uli Fahrer
 */

namespace KaVE.VsFeedbackGenerator.Interactivity
{
    public class LinkNotification : Notification
    {
        public string Link { get; set; }
        public string LinkDescription { get; set; }

        protected bool Equals(LinkNotification other)
        {
            return base.Equals(other) && string.Equals(Link, other.Link) && string.Equals(LinkDescription, other.LinkDescription);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (Link != null ? Link.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (LinkDescription != null ? LinkDescription.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, Link: {1}, LinkDescription: {2}", base.ToString(), Link, LinkDescription);
        }
    }
}
