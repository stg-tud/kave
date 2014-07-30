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

        protected bool Equals(LinkNotification other)
        {
            return string.Equals(Message, other.Message) && string.Equals(Caption, other.Caption) &&
                   string.Equals(Link, other.Link);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Message != null ? Message.GetHashCode() : 0) * 397) ^
                       ((Caption != null ? Caption.GetHashCode() : 0) +
                        (Link != null ? Link.GetHashCode() : 0) * 17);
            }
        }

        public override string ToString()
        {
            return string.Format("[Caption: {0}, Message: {1}, Link: {2}]", Caption, Message, Link); 
        }
    }
}
