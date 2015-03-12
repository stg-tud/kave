﻿/*
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

using System.Runtime.Serialization;
using KaVE.Model.Names.VisualStudio;
using KaVE.Utils;

namespace KaVE.Model.Events.VisualStudio
{
    [DataContract]
    public class WindowEvent : IDEEvent
    {
        public enum WindowAction
        {
            Create,
            Activate,
            Move,
            Close,
            Deactivate
        }

        [DataMember]
        public WindowName Window { get; set; }

        [DataMember]
        public WindowAction Action { get; set; }

        protected bool Equals(WindowEvent other)
        {
            return base.Equals(other) && Equals(Window, other.Window) && Action == other.Action;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (Window != null ? Window.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) Action;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, Window: {1}, Action: {2}", base.ToString(), Window, Action);
        }
    }
}