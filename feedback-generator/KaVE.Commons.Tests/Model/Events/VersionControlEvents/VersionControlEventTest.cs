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
using KaVE.Commons.Model.Events.VersionControlEvents;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events.VersionControlEvents
{
    internal class VersionControlEventTest
    {
        private static readonly SolutionName SomeSolution = SolutionName.Get("SomeSolution");
        private static readonly IKaVEList<VersionControlAction> SomeContent = Lists.NewList(SomeAction);

        private static VersionControlAction SomeAction
        {
            get { return new VersionControlAction {ExecutedAt = DateTime.Now, ActionType = VersionControlActionType.Checkout}; }
        }

        [Test]
        public void DefaultValues()
        {
            var actualEvent = new VersionControlEvent();
            Assert.AreEqual(Lists.NewList<VersionControlAction>(), actualEvent.Content);
            Assert.AreEqual(SolutionName.Get(""), actualEvent.Solution);
        }

        [Test]
        public void SettingValues()
        {
            var actualEvent = new VersionControlEvent
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
            var a = new VersionControlEvent();
            var b = new VersionControlEvent();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new VersionControlEvent {Solution = SomeSolution, Content = {SomeAction}};
            var b = new VersionControlEvent {Solution = SomeSolution, Content = {SomeAction}};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentSolutions()
        {
            var a = new VersionControlEvent {Solution = SomeSolution, Content = {SomeAction}};
            var b = new VersionControlEvent {Content = {SomeAction}};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new VersionControlEvent());
        }
    }
}