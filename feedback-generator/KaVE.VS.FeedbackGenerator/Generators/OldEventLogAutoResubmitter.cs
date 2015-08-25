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

using System.IO;
using System.Threading.Tasks;
using JetBrains.Application;
using KaVE.Commons.Utils.Exceptions;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.FeedbackGenerator.Utils.Logging;

namespace KaVE.VS.FeedbackGenerator.Generators
{
    // TODO: Remove once obsolete.
    [ShellComponent]
    public class OldEventLogAutoResubmitter
    {
        private static readonly string OldEventLogPath = Path.Combine(
            IDEEventLogFileManager.AppDataPath,
            IDEEventLogFileManager.ProjectName,
            "KaVE.VsFeedbackGenerator");

        public OldEventLogAutoResubmitter([NotNull] IMessageBus messageBus, [NotNull] ILogger logger)
        {
            Task.Factory.StartNew(() => Execute(messageBus, logger));
        }

        private static void Execute(IMessageBus messageBus, ILogger logger)
        {
            if (Directory.Exists(OldEventLogPath))
            {
                var count = LogFileUtils.ResubmitLogs(new LogFileManager(OldEventLogPath), messageBus);
                logger.Info("Migrated {0} events from old to new event log path.", count);
                Directory.Delete(OldEventLogPath, true);
            }
        }
    }
}