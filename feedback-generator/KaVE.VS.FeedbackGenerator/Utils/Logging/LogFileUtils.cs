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

using System.Linq;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Utils.Logging
{
    public static class LogFileUtils
    {
        public static int ResubmitLogs(ILogManager sourceLogManager, IMessageBus targetBus)
        {
            var logs = sourceLogManager.Logs;

            var events = logs.SelectMany(log => log.ReadAll()).ToArray();
            foreach (var @event in events)
            {
                targetBus.Publish(@event);
            }

            return events.Length;
        }
    }
}
