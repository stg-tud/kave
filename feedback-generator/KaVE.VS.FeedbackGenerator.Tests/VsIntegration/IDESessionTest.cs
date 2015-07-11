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

using EnvDTE;
using KaVE.VS.FeedbackGenerator.VsIntegration;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.VsIntegration
{
    [TestFixture]
    public class IDESessionTest
    {
        private IDESession _uut;
        private DTE _dte;

        [SetUp]
        public void MockEnvironment()
        {
            _dte = new Mock<DTE>().Object;

            _uut = new IDESession(_dte);
        }

        [Test]
        public void ReturnsSessionUUID()
        {
            var sessionId = _uut.UUID;

            Assert.NotNull(sessionId);
        }

        [Test]
        public void ReturnsNewIdForNewSession()
        {
            var newSession = new IDESession(_dte);

            Assert.AreNotEqual(_uut.UUID, newSession.UUID);
        }

        [Test]
        public void ReturnsConsistentIdForSession()
        {
            var expected = _uut.UUID;
            var actual = _uut.UUID;

            Assert.AreEqual(expected, actual);
        }
    }
}