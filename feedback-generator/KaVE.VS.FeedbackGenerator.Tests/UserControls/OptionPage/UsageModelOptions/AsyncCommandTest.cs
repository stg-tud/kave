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

using System.Threading;
using KaVE.Commons.Utils.Assertion;
using KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.OptionPage.UsageModelOptions
{
    internal class AsyncCommandTest
    {
        private bool _wasCalled;

        [SetUp]
        public void Setup()
        {
            _wasCalled = false;
        }

        [Test]
        public void ShouldUseCanExecute()
        {
            var executableCommand = new AsyncCommand<object>(o => { }, o => true);
            var nonExecutableCommand = new AsyncCommand<object>(o => { }, o => false);
            Assert.IsTrue(executableCommand.CanExecute(null));
            Assert.IsFalse(nonExecutableCommand.CanExecute(null));
        }

        [Test]
        public void ShouldFireOnCompleted()
        {
            var uut = new AsyncCommand<object>(o => { }, o => true);
            uut.ExecuteCompleted += delegate { _wasCalled = true; };
            uut.Execute(null);
            AssertWasCalled();
        }

        [Test]
        public void ShouldExecuteCommand()
        {
            var uut = new AsyncCommand<object>(
                o => _wasCalled = true,
                o => true);
            uut.Execute(null);
            AssertWasCalled();
        }

        [Test]
        public void ShouldRunOnAnotherThread()
        {
            new AsyncCommand<object>(o => { Assert.Fail(); }, o => true).Execute(null);
            Thread.Sleep(100);
        }

        [Test, Timeout(2000)]
        public void ShouldNotBeAbleToExecuteWhileExecuteIsNotFinished()
        {
            var uut = new AsyncCommand<object>(
                b => { while (true) {} },
                b => true);

            uut.Execute(null);

            Thread.Sleep(1000);
            Assert.IsFalse(uut.CanExecute(null));
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void ShouldThrowOnWrongExecuteInputType()
        {
            new AsyncCommand<int>(i => { }, i => true).Execute("");
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void ShouldThrowOnWrongCanExecuteInputType()
        {
            new AsyncCommand<int>(i => { }, i => true).CanExecute("");
        }

        private void AssertWasCalled()
        {
            Assert.That(() => _wasCalled, Is.True.After(1000, 100));
        }
    }
}