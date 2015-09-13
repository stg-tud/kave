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
using JetBrains.ProjectModel;
using KaVE.Commons.Model.Events.GitEvents;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.Generators.Git;
using KaVE.VS.FeedbackGenerator.MessageBus;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.Git
{
    internal class GitEventGeneratorUnitTest : EventGeneratorTestBase
    {
        private const string TestContent =
            "de75df3fd4322ec96e02c078e90228f121b6b53c 6f2eaaff6079e41af242a41a09b5f9510214d014 TestUsername <TestMail@domain.de> 1441217745 +0200	commit: Test commit";

        private TestGitEventGenerator _uut;
        private Mock<ISolution> _solutionMock;

        [SetUp]
        public void Setup()
        {
            _uut = new TestGitEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils);

            _solutionMock = new Mock<ISolution>();
            _solutionMock.Setup(solution => solution.Name).Returns("SomeSolution");
        }

        [Test]
        public void ShouldSetContent()
        {
            WriteFile(TestContent);

            var actualEvent = GetSinglePublished<GitEvent>();
            var expectedGitAction = new GitAction
            {
                ActionType = GitActionType.Commit,
                ExecutedAt = new DateTime(2015, 9, 2, 20, 15, 45)
            };
            CollectionAssert.AreEqual(Lists.NewList(expectedGitAction), actualEvent.Content);
        }

        private void WriteFile(string newLine)
        {
            _uut.Content = new[] {newLine};
            _uut.OnGitHistoryFileChanged(
                null,
                new GitHistoryFileChangedEventArgs("C:\\", _solutionMock.Object));
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