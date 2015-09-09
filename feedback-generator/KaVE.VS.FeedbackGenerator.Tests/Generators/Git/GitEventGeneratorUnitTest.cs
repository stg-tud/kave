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
using JetBrains.Util;
using KaVE.Commons.Model.Events.GitEvents;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.Generators.Git;
using KaVE.VS.FeedbackGenerator.MessageBus;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.Git
{
    internal class GitEventGeneratorUnitTest : EventGeneratorTestBase
    {
        private const string TestRepositoryDirectory = @"C:\Users\SomeUser\Documents\SomeRepository";
        private const string TestContent = "SomeTestContent";
        private const string RelativeGitLogPath = @".git\logs\";

        private TestGitEventGenerator _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new TestGitEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils);
        }

        [Test, Ignore("Not yet implemented")]
        public void ShouldSetContent()
        {
            ChangeFile(TestContent);

            var actualEvent = GetSinglePublished<GitEvent>();
            Assert.AreEqual(new[] {TestContent}, actualEvent.Content);
        }

        private void ChangeFile(string newLine)
        {
            var newContent = _uut.Content.AsList();
            newContent.Add(newLine);
            _uut.Content = newContent.ToArray();

            _uut.OnGitHistoryFileChanged(
                null,
                new FileSystemEventArgs(
                    WatcherChangeTypes.Changed,
                    Path.Combine(TestRepositoryDirectory, RelativeGitLogPath),
                    "HEAD"));
        }

        private class TestGitEventGenerator : GitEventGenerator
        {
            public string[] Content = {};

            public TestGitEventGenerator([NotNull] IRSEnv env,
                [NotNull] IMessageBus messageBus,
                [NotNull] IDateUtils dateUtils)
                : base(env, messageBus, dateUtils) {}

            protected override string[] ReadLogContent(string fullPath)
            {
                return Content;
            }
        }
    }
}