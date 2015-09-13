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
using JetBrains.ProjectModel;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;

namespace KaVE.VS.FeedbackGenerator.Generators.Git
{
    [SolutionComponent]
    internal class GitHistoryFileChangedRegistration : IDisposable
    {
        [CanBeNull]
        private readonly FileSystemWatcher _watcher;
        
        public GitHistoryFileChangedRegistration([NotNull] ISolution solution,
            [NotNull] GitEventGenerator gitEventGenerator)
        {
            var gitLogDirectory = GetGitLogPath(solution);
            if (gitLogDirectory.IsNullOrEmpty())
            {
                return;
            }

            _watcher = new FileSystemWatcher
            {
                Filter = "HEAD",
                Path = gitLogDirectory,
                EnableRaisingEvents = true
            };

            _watcher.Changed +=
                (sender, args) =>
                {
                    var directoryName = Path.GetDirectoryName(args.FullPath);
                    if (directoryName == null) return;

                    gitEventGenerator.OnGitHistoryFileChanged(
                        this,
                        new GitHistoryFileChangedEventArgs(directoryName, solution));
                };
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
            var currentDirectory = Path.GetDirectoryName(solutionPath);
            if (currentDirectory == null) return string.Empty;

            while (!ContainsGitFolder(currentDirectory))
            {
                try
                {
                    currentDirectory = Directory.GetParent(currentDirectory).FullName;
                }
                catch (Exception e)
                {
                    if (e is UnauthorizedAccessException || e is DirectoryNotFoundException)
                    {
                        currentDirectory = string.Empty;
                        break;
                    }

                    throw;
                }
            }
            return currentDirectory;
        }

        private static bool ContainsGitFolder(string currentDirectory)
        {
            return Directory.Exists(Path.Combine(currentDirectory, ".git"));
        }

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
            }
        }
    }

    public class GitHistoryFileChangedEventArgs : FileSystemEventArgs
    {
        public ISolution Solution { get; private set; }

        public GitHistoryFileChangedEventArgs([NotNull] string directory, [NotNull] ISolution solution)
            : base(WatcherChangeTypes.Changed, directory, "HEAD")
        {
            Solution = solution;
        }
    }
}