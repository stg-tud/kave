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
    internal class GitHistoryFileChangedRegistration
    {
        private const string RelativeGitLogPath = @".git\logs";

        public GitHistoryFileChangedRegistration([NotNull] ISolution solution,
            [NotNull] GitEventGenerator gitEventGenerator)
        {
            var repositoryDirectory = GetGitLogPath(solution);
            if (repositoryDirectory.IsNullOrEmpty())
            {
                // TODO: log failure at this point?
                return;
            }

            var watcher = new FileSystemWatcher
            {
                Path = repositoryDirectory,
                EnableRaisingEvents = true
            };
            watcher.Changed += gitEventGenerator.OnGitHistoryFileChanged;
        }

        private static string GetGitLogPath(ISolution solution)
        {
            var repositoryDirectory = FindRepositoryDirectory(solution.SolutionFilePath.FullPath);
            return repositoryDirectory.IsNullOrEmpty() ? string.Empty : Path.Combine(repositoryDirectory, RelativeGitLogPath);
        }

        private static string FindRepositoryDirectory(string solutionPath)
        {
            var currentDirectory = Directory.GetParent(solutionPath).FullName;
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
    }
}