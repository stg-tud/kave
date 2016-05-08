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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
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
        private static readonly string OldEventLogBasePath = Path.Combine(
            IDEEventLogFileManager.AppDataPath,
            IDEEventLogFileManager.ProjectName,
            "KaVE.VS.FeedbackGenerator");

        public OldEventLogAutoResubmitter([NotNull] IMessageBus messageBus,
            // Dependency on EventLogger is necessary to make sure it has already been
            // created and is subscribed to the message bus.
            [NotNull] EventLogger eventLogger,
            [NotNull] ILogger logger,
            [NotNull] VersionUtil versionUtil)
        {
            Task.Factory.StartNew(
                () =>
                    Execute(
                        Path.Combine(OldEventLogBasePath, versionUtil.GetCurrentVariant().ToString()),
                        messageBus,
                        logger));
        }

        private static void Execute(string oldEventLogPath, IMessageBus messageBus, ILogger logger)
        {
            if (Directory.Exists(oldEventLogPath))
            {
                var count = LogFileUtils.ResubmitLogs(new LogFileManager(oldEventLogPath), messageBus);
                logger.Info("Migrated {0} events from old to new event log path.", count);
                messageBus.Publish(new InfoEvent {Info = "wat?"});
                Directory.Delete(oldEventLogPath, true);
            }
        }
    }
}