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
using KaVE.Utils;

namespace KaVE.VsFeedbackGenerator.Interactivity
{
    public class Notification
    {
        public string Caption { get; set; }
        public string Message { get; set; }

        protected bool Equals(Notification other)
        {
            return string.Equals(Message, other.Message) && string.Equals(Caption, other.Caption);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Message != null ? Message.GetHashCode() : 0)*397) ^
                       (Caption != null ? Caption.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("[Caption: {1}, Message: {0}]", Caption, Message);
        }
    }
}