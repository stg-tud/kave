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
using JetBrains.ProjectModel;
using KaVE.Commons.Model.Events.GitEvents;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.Git
{
    [SolutionComponent]
    internal class GitEventGenerator : EventGeneratorBase
    {
        public GitEventGenerator([NotNull] IRSEnv env, [NotNull] IMessageBus messageBus, [NotNull] IDateUtils dateUtils)
            : base(env, messageBus, dateUtils) {}

        public void OnGitHistoryFileChanged(object sender, FileSystemEventArgs args)
        {
            var content = ReadLogContent(args.FullPath);
            var repositoryDirectory = GetRepositoryDirectory(args.FullPath);

            Fire(content, repositoryDirectory);
        }

        public void Fire(string[] logContent, string repositoryDirectory)
        {
            var gitEvent = Create<GitEvent>();

            // TODO: generate event content from logContent 

            Fire(gitEvent);
        }

        [Pure]
        private static string GetRepositoryDirectory(string logPath)
        {
            // logPath is at <repository>\.git\log\HEAD
            return Directory.GetParent(Directory.GetParent(Directory.GetParent(logPath).FullName).FullName).FullName;
        }

        [Pure]
        protected virtual string[] ReadLogContent(string fullPath)
        {
            return File.ReadAllLines(fullPath);
        }
    }
}