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
using KaVE.VsFeedbackGenerator.Interactivity;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Interactivity
{
    [TestFixture]
    internal class InteractionRequestTest
    {
        private InteractionRequest<Notification> _uut;

        [SetUp]
        public void SetUpRequest()
        {
            _uut = new InteractionRequest<Notification>();
        }

        [Test]
        public void ShouldRaiseRequest()
        {
            var raised = false;
            _uut.Raised += (sender, args) => raised = true;

            _uut.Raise(new Notification());

            Assert.IsTrue(raised);
        }

        [Test]
        public void ShouldPassContextToHandler()
        {
            Notification actualContext = null;
            _uut.Raised += (sender, args) => actualContext = args.Notification;
            var expectedContext = new Notification();

            _uut.Raise(expectedContext);

            Assert.AreSame(expectedContext, actualContext);
        }

        [Test]
        public void ShouldDelegateToCallbackFromHandler()
        {
            var invoked = false;
            _uut.Raised += (sender, args) => args.Callback();

            _uut.Raise(new Notification(), n => invoked = true);

            Assert.IsTrue(invoked);
        }
    }
}