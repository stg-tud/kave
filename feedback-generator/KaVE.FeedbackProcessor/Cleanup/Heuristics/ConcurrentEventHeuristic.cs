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
using System.Collections.Generic;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.DateTimes;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Cleanup.Heuristics
{
    internal class ConcurrentEventHeuristic
    {
        public static readonly TimeSpan EventTimeDifference = TimeSpan.FromMilliseconds(100);

        public static readonly List<string> IgnorableTextControlCommands = new List<string>
        {
            "TextControl.Left",
            "TextControl.Right",
            "TextControl.Up",
            "TextControl.Down",
            "TextControl.Backspace",
            "TextControl.Enter",
            "TextControl.Delete",
            "TextControl.Up.Selection",
            "TextControl.Down.Selection",
            "TextControl.Left.Selection",
            "TextControl.Right.Selection"
        };

        public static bool AreConcurrent(IDEEvent evt1, IDEEvent evt2)
        {
            return AreSimilar(evt1.GetTriggeredAt(), evt2.GetTriggeredAt());
        }

        public static bool AreSimilar(DateTime dateTime1, DateTime dateTime2)
        {
            return new SimilarDateTimeComparer(EventTimeDifference.Milliseconds).Equal(dateTime1, dateTime2);
        }

        public static bool IsIgnorableTextControlCommand(string commandId)
        {
            return IgnorableTextControlCommands.Contains(commandId);
        }
    }
}