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
using System.IO;
using System.Security;
using JetBrains.ProjectModel;
using JetBrains.Util;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.JetBrains.Annotations;

namespace KaVE.VS.FeedbackGenerator.Generators.Git
{
    [SolutionComponent]
    internal class GitLogFileChangedRegistration : IDisposable
    {
        [CanBeNull]
        private readonly FileSystemWatcher _watcher;

        [NotNull]
        private readonly ISolution _solution;

        [NotNull]
        private readonly IGitEventGenerator _eventGenerator;

        public GitLogFileChangedRegistration([NotNull] ISolution solution,
            [NotNull] IGitEventGenerator gitEventGenerator)
        {
            _solution = solution;
            _eventGenerator = gitEventGenerator;

            var gitLogDirectory = GetGitLogPath(solution);
            if (gitLogDirectory.IsNullOrEmpty())
            {
                return;
            }

            CreateGitLogFile(gitLogDirectory);

            OnWatchStart(gitLogDirectory);

            _watcher = new FileSystemWatcher
            {
                Filter = "HEAD",
                Path = gitLogDirectory,
                EnableRaisingEvents = true
            };

            _watcher.Changed += OnGitHistoryFileChanged;
        }

        private void OnWatchStart(string directory)
        {
            _eventGenerator.OnGitHistoryFileChanged(
                this,
                new GitLogFileChangedEventArgs(directory, SolutionName.Get(_solution.Name)));
        }

        private void OnGitHistoryFileChanged(object sender, FileSystemEventArgs args)
        {
            var directoryName = Path.GetDirectoryName(args.FullPath);
            if (directoryName == null)
            {
                return;
            }

            _eventGenerator.OnGitHistoryFileChanged(
                this,
                new GitLogFileChangedEventArgs(directoryName, SolutionName.Get(_solution.Name)));
        }

        private static string GetGitLogPath(ISolution solution)
        {
            const string relativeGitLogPath = @".git\logs";
            var repositoryDirectory = FindRepositoryDirectory(solution.SolutionFilePath.FullPath);
            return repositoryDirectory.IsNullOrEmpty()
                ? string.Empty
                : Path.Combine(repositoryDirectory, relativeGitLogPath);
        }

        private static string FindRepositoryDirectory(string solutionPath)
        {
            var currentDirectory = new DirectoryInfo(solutionPath);

            while (currentDirectory != null && !ContainsGitFolder(currentDirectory.FullName))
            {
                try
                {
                    currentDirectory = currentDirectory.Parent;
                }
                catch (SecurityException)
                {
                    return string.Empty;
                }
            }

            return currentDirectory != null ? currentDirectory.FullName : string.Empty;
        }

        private static bool ContainsGitFolder(string currentDirectory)
        {
            return Directory.Exists(Path.Combine(currentDirectory, ".git"));
        }

        private static void CreateGitLogFile(string gitLogDirectory)
        {
            var gitLogFile = Path.Combine(gitLogDirectory, "HEAD");
            if (!Directory.Exists(gitLogDirectory))
            {
                Directory.CreateDirectory(gitLogDirectory);
            }
            if (!File.Exists(gitLogFile))
            {
                File.Create(gitLogFile).Dispose();
            }
        }

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
            }
        }
    }

    public class GitLogFileChangedEventArgs : FileSystemEventArgs
    {
        public SolutionName Solution { get; private set; }

        public GitLogFileChangedEventArgs([NotNull] string directory, [NotNull] SolutionName solution)
            : base(WatcherChangeTypes.Changed, directory, "HEAD")
        {
            Solution = solution;
        }
    }
}