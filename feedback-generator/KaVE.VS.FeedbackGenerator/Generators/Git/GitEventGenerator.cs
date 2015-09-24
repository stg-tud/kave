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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.ProjectModel;
using JetBrains.Util.Extension;
using KaVE.Commons.Model.Events.VersionControlEvents;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.MessageBus;
using NuGet;

namespace KaVE.VS.FeedbackGenerator.Generators.Git
{
    public interface IGitEventGenerator
    {
        void OnGitHistoryFileChanged(object sender, GitLogFileChangedEventArgs args);
    }

    [SolutionComponent]
    internal class GitEventGenerator : EventGeneratorBase, IGitEventGenerator
    {
        private readonly IKaVEList<VersionControlAction> _oldActions;

        public GitEventGenerator([NotNull] IRSEnv env, [NotNull] IMessageBus messageBus, [NotNull] IDateUtils dateUtils)
            : base(env, messageBus, dateUtils)
        {
            _oldActions = Lists.NewList<VersionControlAction>();
        }

        public void OnGitHistoryFileChanged(object sender, GitLogFileChangedEventArgs args)
        {
            var eventContent = ReadNewGitActionsFrom(ReadLogContent(args.FullPath));
            if (!eventContent.IsEmpty())
            {
                Fire(eventContent, args.Solution);
            }
        }

        private void Fire(IKaVEList<VersionControlAction> content, SolutionName solutionName)
        {
            var gitEvent = Create<VersionControlEvent>();
            gitEvent.Solution = solutionName;
            gitEvent.Actions = content;
            FireNow(gitEvent);
        }

        private IKaVEList<VersionControlAction> ReadNewGitActionsFrom(IEnumerable<string> logContent)
        {
            var gitActions = Lists.NewList<VersionControlAction>();

            foreach (
                var newAction in
                    logContent.Select(
                        logEntry =>
                            new VersionControlAction
                            {
                                ExecutedAt = ExtractExecutedAtFrom(logEntry),
                                ActionType = ExtractActionTypeFrom(logEntry)
                            })
                              .Where(gitAction => !_oldActions.Contains(gitAction)))
            {
                _oldActions.Add(newAction);
                gitActions.Add(newAction);
            }

            return gitActions;
        }

        private static VersionControlActionType ExtractActionTypeFrom([NotNull] string entry)
        {
            var substring = new Regex("\t.*: ").Match(entry).Value.SubstringAfter("\t").SubstringBefore(": ");
            return substring.ToVersionControlActionType();
        }

        private static DateTime? ExtractExecutedAtFrom([NotNull] string entry)
        {
            string unixTimeStamp;
            try
            {
                unixTimeStamp = entry.Split(' ')[4];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
            
            // Unix timestamp is seconds since 1970-01-01T00:00:00Z
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(int.Parse(unixTimeStamp)).ToLocalTime();
            return dateTime;
        }

        [Pure]
        protected virtual IEnumerable<string> ReadLogContent(string fullPath)
        {
            try
            {
                return File.ReadAllLines(fullPath);
            }
            catch (IOException)
            {
                return new string[0];
            }
        }
    }
}