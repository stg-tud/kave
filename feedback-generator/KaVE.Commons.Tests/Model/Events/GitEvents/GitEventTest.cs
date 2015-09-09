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

using KaVE.Commons.Model.Events.GitEvents;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events.GitEvents
{
    internal class GitEventTest
    {
        private static readonly SolutionName SomeSolution = SolutionName.Get("SomeSolution");
        private static readonly IKaVEList<GitAction> SomeContent = Lists.NewList(new GitAction());

        [Test]
        public void DefaultValues()
        {
            var actualEvent = new GitEvent();
            Assert.AreEqual(Lists.NewList<GitAction>(), actualEvent.Content);
            Assert.AreEqual(SolutionName.Get(""), actualEvent.Solution);
        }

        [Test]
        public void SettingValues()
        {
            var actualEvent = new GitEvent
            {
                Solution = SomeSolution,
                Content = SomeContent
            };
            Assert.AreEqual(SomeSolution, actualEvent.Solution);
            Assert.AreEqual(SomeContent, actualEvent.Content);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new GitEvent();
            var b = new GitEvent();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new GitEvent {Solution = SomeSolution, Content = SomeContent};
            var b = new GitEvent {Solution = SomeSolution, Content = SomeContent};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentSolutions()
        {
            var a = new GitEvent {Solution = SomeSolution, Content = SomeContent};
            var b = new GitEvent {Content = SomeContent};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentContents()
        {
            var a = new GitEvent {Solution = SomeSolution, Content = SomeContent};
            var b = new GitEvent {Solution = SomeSolution};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}