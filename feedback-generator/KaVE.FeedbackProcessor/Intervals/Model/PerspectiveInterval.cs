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

namespace KaVE.FeedbackProcessor.Intervals.Model
{
    public enum PerspectiveType
    {
        Production,
        Debug
    }

    public class PerspectiveInterval : Interval
    {
        public PerspectiveType Perspective { get; set; }

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