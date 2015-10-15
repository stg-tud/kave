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
using KaVE.Commons.Utils.Csv;
using KaVE.Commons.Utils.DateTime;
using KaVE.FeedbackProcessor.Activities.Intervals;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.VsWindows
{
    internal class WindowIntervalProcessor : IntervalProcessor<string>
    {
        public const string OutsideIDEIntervalId = ":outside of ide:";

        protected override void HandleWithCurrentInterval(ActivityEvent @event)
        {
            if (@event.Activity == Activity.EnterIDE)
            {
                if (CurrentInterval.Id.Equals(OutsideIDEIntervalId))
                {
                    CurrentInterval.End = @event.GetTriggeredAt();
                }
                else
                {
                    StartInterval(CurrentInterval.End, OutsideIDEIntervalId, @event.GetTriggeredAt());
                }
            }

            var previousInterval = CurrentInterval;

            if (previousInterval.Id.Equals(GetIntervalId(@event)))
            {
                previousInterval.End = GetEnd(@event);
            }
            else
            {
                StartInterval(@event);
                previousInterval.End = CurrentInterval.Start;
            }
        }

        protected override string GetIntervalId(ActivityEvent @event)
        {
            return @event.Activity == Activity.LeaveIDE ? OutsideIDEIntervalId : @event.ActiveWindow.Identifier;
        }

        public string IntervalsToVsWindowUsageStatisticCsv()
        {
            var builder = new CsvBuilder();
            foreach (var intervalStream in Intervals)
            {
                var usage = new Dictionary<string, TimeSpan>();
                foreach (var interval in intervalStream.Value)
                {
                    if (!usage.ContainsKey(interval.Id))
                    {
                        usage[interval.Id] = TimeSpan.Zero;
                    }
                    usage[interval.Id] += interval.Duration;
                }

                builder.StartRow();
                builder["developer"] = intervalStream.Key.Id;
                foreach (var windowUsage in usage)
                {
                    builder[windowUsage.Key] = windowUsage.Value.RoundedTotalSeconds();
                }
            }
            return builder.Build();
        }
    }
}