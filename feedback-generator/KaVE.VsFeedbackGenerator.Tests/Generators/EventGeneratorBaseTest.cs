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
 * 
 * Contributors:
 *    - Sven Amann
 */

using JetBrains.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.VsFeedbackGenerator.Generators;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators
{
    [TestFixture]
    internal class EventGeneratorBaseTest : EventGeneratorTestBase
    {
        private TestEventGenerator _uut;

        private class TestEventGenerator : EventGeneratorBase
        {
            public TestEventGenerator([NotNull] IRSEnv env,
                [NotNull] IMessageBus messageBus,
                [NotNull] IDateUtils dateUtils) : base(env, messageBus, dateUtils) {}

            public void FireTestIDEEvent()
            {
                Fire(Create<TestIDEEvent>());
            }

            public void FireTestIDEEventNow()
            {
                FireNow(Create<TestIDEEvent>());
            }
        }

        [SetUp]
        public void SetUp()
        {
            _uut = new TestEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils);
        }

        [Test]
        public void ShouldSetExtensionVersion()
        {
            TestRSEnv.MockExtension.Setup(ex => ex.Version).Returns(SemanticVersion.Parse("1.0-test"));

            _uut.FireTestIDEEvent();

            var ideEvent = GetSinglePublished<TestIDEEvent>();
            Assert.AreEqual("1.0-test", ideEvent.KaVEVersion);
        }

        [Test]
        public void ShouldSetDateToNow()
        {
            _uut.FireTestIDEEventNow();

            var ideEvent = GetSinglePublished<TestIDEEvent>();
            Assert.AreEqual(TestDateUtils.Now, ideEvent.TriggeredAt);
        }
    }
}